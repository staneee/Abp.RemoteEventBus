using RemoteEventBus.Attributes;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using EasyNetQ;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using RabbitMQ.Client.Events;

namespace RemoteEventBus.Impl
{
    public class RabbitMQRemoteEventBus : IRemoteEventBus
    {
        // 公用
        protected readonly IRabbitMQSetting _rabbitMQSetting;
        protected readonly ISerializer _serializer;

        // 发布者使用
        protected readonly IBus _bus;
        private readonly ConcurrentDictionary<string, IModel> _publisherDictionary;
        private readonly List<IConnection> _publisherConnectionsAcquired;
        //private readonly IObjectPool<IConnection> _connectionPool;

        // 订阅者使用
        private readonly ConcurrentDictionary<string, IModel> _dictionary;
        private readonly List<IConnection> _connectionsAcquired;
        private readonly IRabbitMQConnectionFactory _factory;


        private bool _disposed;

        public RabbitMQRemoteEventBus(
            IRabbitMQSetting rabbitMQSetting,
            ISerializer serializer,
            IRabbitMQConnectionFactory factory
            )
        {
            // 公用
            _rabbitMQSetting = rabbitMQSetting;
            _serializer = serializer;

            if (_rabbitMQSetting.UseEasyNetQ)
            {
                _bus = RabbitHutch.CreateBus(_rabbitMQSetting.ConnectionString, (register) =>
                {
                    // 替换默认的序列化器
                    register.Register<ISerializer>((a) =>
                    {
                        return _serializer;
                    });
                });
            }
            else
            {
                _publisherDictionary = new ConcurrentDictionary<string, IModel>();
                _publisherConnectionsAcquired = new List<IConnection>();
                // 订阅使用
                _dictionary = new ConcurrentDictionary<string, IModel>();
                _connectionsAcquired = new List<IConnection>();
                _factory = factory;
            }
        }


        public void Publish<TEventData>(TEventData eventData, string topic = null)
            where TEventData : class, new()

        {
            if (_rabbitMQSetting.UseEasyNetQ)
            {
                EasyNetQPublish<TEventData>(eventData);
                return;
            }

            RabbitMQClientPublish<TEventData>(eventData, topic);
        }

        public Task PublishAsync<TEventData>(TEventData eventData, string topic = null)
            where TEventData : class, new()
        {
            return Task.Factory.StartNew(() =>
            {
                Publish<TEventData>(eventData, topic);
            });
        }

        public void Subscribe<TEventData>(Action<TEventData> invoke, string topic = null)
           where TEventData : class, new()
        {
            if (_rabbitMQSetting.UseEasyNetQ)
            {
                EasyNetQSubscribe<TEventData>(invoke, topic);
                return;
            }

            RabbitMQClientSubscribe<TEventData>(invoke, topic);
        }

        public Task SubscribeAsync<TEventData>(Action<TEventData> invoke, string topic = null)
            where TEventData : class, new()
        {
            return Task.Factory.StartNew(() =>
            {
                Subscribe<TEventData>(invoke, topic);
            });
        }

        #region RabbitMQClient

        /// <summary>
        /// RabbitMQ发布
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventData"></param>
        protected virtual void RabbitMQClientPublish<TEventData>(TEventData eventData, string topic)
           where TEventData : class, new()

        {
            var keyType = typeof(TEventData);
            var buffer = _serializer.MessageToBytes(typeof(TEventData), eventData);

            var topicAttr = keyType.GetCustomAttributes(typeof(TopicAttribute), true)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();

            // topic特性不为空
            if (topicAttr != null)
            {
                RabbitMQClientPublish(CreateKey(topic ?? keyType.Name), buffer);
                return;
            }


            // 负载均衡模式
            var loadBalancingAttr = keyType.GetCustomAttributes(typeof(ConnectionLoadBalancingAttribute), true)
                  .Select(o => o as ConnectionLoadBalancingAttribute)
                .FirstOrDefault();
            if (loadBalancingAttr != null)
            {
                var loadBalancingInfo = _rabbitMQSetting.LoadBalancings
                    .Find(o => o.HandlerType == topic);
                if (loadBalancingInfo != null)
                {
                    if (string.IsNullOrWhiteSpace(topic))
                    {
                        RabbitMQClientPublish(CreateKey(loadBalancingInfo.NextKey()), buffer);
                    }
                    else
                    {
                        RabbitMQClientPublish(
                            CreateKey($"{topic}_{ loadBalancingInfo.NextIndex()}"),
                            buffer
                            );
                    }
                    return;
                }
            }

            // 普通的工作队列模式
            RabbitMQClientPublish(CreateKey(topic ?? keyType.Name), buffer);
        }

        protected virtual void RabbitMQClientPublish(string topic, byte[] buffer)
        {
            IConnection connection = null;
            if (!_publisherDictionary.TryGetValue(topic, out IModel channel))
            {
                connection = _factory.Create();
                _publisherConnectionsAcquired.Add(connection);
                channel = connection.CreateModel();
            }


            var topicName = topic;
            var body = buffer;


            channel.ExchangeDeclare(exchange: topicName,
                type: ExchangeType.Fanout,
                durable: false,
                autoDelete: false,
                arguments: null);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = _rabbitMQSetting.Persistent;

            // topic和通道关联
            _publisherDictionary[topicName] = channel;

            channel.BasicPublish(
                exchange: topicName,
                routingKey: topicName,
                basicProperties: properties,
                body: body);

        }


        /// <summary>
        /// RabbitMQ订阅
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="invoke"></param>
        protected virtual void RabbitMQClientSubscribe<TEventData>(Action<TEventData> invoke, string topic = null)
                 where TEventData : class, new()
        {
            var keyType = typeof(TEventData);

            var key = CreateKey(topic ?? keyType.Name);

            // topic模式
            var topicAttr = keyType.GetCustomAttributes(typeof(TopicAttribute), true)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();
            if (topicAttr != null)
            {
                RabbitMQClientSubscribe(key, (buffer) =>
                  {
                      invoke(
                            (TEventData)_serializer.BytesToMessage(typeof(TEventData), buffer)
                        );
                  }, false);
                return;
            }

            // 普通的工作队列模式
            RabbitMQClientSubscribe(key, (buffer) =>
            {
                invoke(
                        (TEventData)_serializer.BytesToMessage(typeof(TEventData), buffer)
                    );
            }, true);
        }


        protected virtual void RabbitMQClientSubscribe(string topic, Action<byte[]> invoke, bool isWorkQueue = true)
        {
            var existsTopics = _dictionary.ContainsKey(topic);
            if (existsTopics)
            {
                throw new Exception(string.Format("the topics {0} have subscribed already", string.Join(",", existsTopics)));
            }
            var connection = _factory.Create();
            _connectionsAcquired.Add(connection);
            try
            {
                var topicName = topic;
                var queueName = topicName;

                var channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: topicName,
                    type: ExchangeType.Fanout,
                    durable: false,
                    autoDelete: false,
                    arguments: null);


                if (isWorkQueue)// 工作队列,指定队列名称
                {
                    channel.QueueDeclare(queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    channel.BasicQos(0, 1, false);
                }
                else// 非工作队列,随机队列名称
                {
                    queueName = channel.QueueDeclare(durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null).QueueName;
                }

                channel.QueueBind(queue: queueName,
                       exchange: topicName,
                       routingKey: topicName);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (ch, ea) =>
                {
                    invoke(ea.Body);
                    //handler(ea.RoutingKey, ea.Body);
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(queue: queueName,
                    autoAck: false,
                    consumer: consumer);

                // topic和通道关联
                _dictionary[topicName] = channel;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _connectionsAcquired.Remove(connection);
            }
        }



        #endregion


        #region EASYNETQClient

        /// <summary>
        /// EasyNetQ发布
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventData"></param>
        protected virtual void EasyNetQPublish<TEventData>(TEventData eventData)
            where TEventData : class, new()

        {
            var keyType = typeof(TEventData);

            var topicAttr = keyType.GetCustomAttributes(typeof(TopicAttribute), true)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();

            // topic特性不为空
            if (topicAttr != null)
            {
                _bus.Publish(eventData);
                return;
            }


            // 负载均衡模式
            var loadBalancingAttr = keyType.GetCustomAttributes(typeof(ConnectionLoadBalancingAttribute), true)
                  .Select(o => o as ConnectionLoadBalancingAttribute)
                .FirstOrDefault();
            if (loadBalancingAttr != null)
            {
                var loadBalancingInfo = _rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType == keyType.Name);
                if (loadBalancingInfo != null)
                {
                    _bus.Send(loadBalancingInfo.NextKey(), eventData);
                    return;
                }
            }

            // 普通的工作队列模式
            _bus.Send(keyType.Name, eventData);

        }

        /// <summary>
        /// EasyNetQ订阅
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="instance"></param>
        protected virtual void EasyNetQSubscribe<TEventData>(Action<TEventData> invoke, string topic)
                 where TEventData : class, new()
        {
            var keyType = typeof(TEventData);

            // topic模式
            var topicAttr = keyType.GetCustomAttributes(typeof(TopicAttribute), true)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();
            if (topicAttr != null)
            {
                //TEntity
                _bus.Subscribe(Guid.NewGuid().ToString(), invoke);
                return;
            }

            // 普通的工作队列模式
            _bus.Receive<TEventData>(topic ?? keyType.Name, (data) =>
            {
                invoke(data);
            });
        }

        #endregion


        #region 取消订阅

        public virtual void Unsubscribe<TEventData>(string topic = null)
             where TEventData : class, new()
        {
            var keyType = typeof(TEventData);

            // 负载均衡模式
            var loadBalancingAttr = keyType.GetCustomAttributes(typeof(ConnectionLoadBalancingAttribute), true)
                  .Select(o => o as ConnectionLoadBalancingAttribute)
                .FirstOrDefault();

            if (loadBalancingAttr != null)
            {
                var loadBalancingInfo = _rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType == keyType.Name);
                if (loadBalancingInfo != null)
                {
                    Unsubscribe(loadBalancingInfo.GetAll());
                    return;
                }
            }

            var key = CreateKey(topic ?? keyType.Name);
            _dictionary[key].Close();
            _dictionary[key].Dispose();
        }

        public virtual Task UnsubscribeAsync<TEventData>(string topic = null)
             where TEventData : class, new()
        {
            return Task.Factory.StartNew(() =>
            {
                Unsubscribe<TEventData>(topic);
            });
        }

        public virtual void Unsubscribe(string topic)
        {
            topic = CreateKey(topic);
            if (_dictionary.ContainsKey(topic))
            {
                _dictionary[topic].Close();
                _dictionary[topic].Dispose();
            }

        }

        public virtual Task UnsubscribeAsync(string topic)
        {
            return Task.Factory.StartNew(() =>
            {
                Unsubscribe(topic);
            });
        }

        public virtual void Unsubscribe(IEnumerable<string> topics)
        {
            topics = topics.Select(o => CreateKey(o));
            foreach (var topic in topics)
            {
                if (_dictionary.ContainsKey(topic))
                {
                    _dictionary[topic].Close();
                    _dictionary[topic].Dispose();
                }
            }
        }

        public virtual Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return Task.Factory.StartNew(() => Unsubscribe(topics));
        }

        public virtual void UnsubscribeAll()
        {
            Unsubscribe(_dictionary.Select(p => p.Key));
        }

        public virtual Task UnsubscribeAllAsync()
        {
            return Task.Factory.StartNew(UnsubscribeAll);
        }

        #endregion


        /// <summary>
        /// 创建队列键值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected virtual string CreateKey(string str)
        {
            if (string.IsNullOrWhiteSpace(this._rabbitMQSetting.Prefix))
            {
                return str;
            }

            return $"{this._rabbitMQSetting.Prefix}_{str}";
        }

        public virtual void Dispose()
        {
            if (!_disposed)
            {

                #region 释放发布者通道

                foreach (var item in _publisherDictionary)
                {
                    item.Value.Close();
                    item.Value.Dispose();
                }

                #endregion


                #region 释放订阅通道

                UnsubscribeAll();

                #endregion


                // 释放factory创建的所有资源
                _factory?.Dispose();


                // 释放bus资源
                _bus?.Dispose();


                _disposed = true;
            }
        }
    }
}
