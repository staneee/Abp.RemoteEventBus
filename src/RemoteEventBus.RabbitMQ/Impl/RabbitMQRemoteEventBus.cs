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


        public void Publish<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()

        {
            if (_rabbitMQSetting.UseEasyNetQ)
            {
                EasyNetQPublish<THandler, TEntity>(eventData);
                return;
            }

            RabbitMQClientPublish<THandler, TEntity>(eventData);
        }

        public Task PublishAsync<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()
        {
            return Task.Factory.StartNew(() =>
            {
                Publish<THandler, TEntity>(eventData);
            });
        }

        public void Subscribe<THandler, TEntity>(THandler instance, string topic = null)
           where THandler : IRemoteEventHandler<TEntity>
           where TEntity : class, new()
        {
            if (_rabbitMQSetting.UseEasyNetQ)
            {
                EasyNetQSubscribe<THandler, TEntity>(instance, topic);
                return;
            }

            RabbitMQClientSubscribe<THandler, TEntity>(instance, topic);
        }

        public Task SubscribeAsync<THandler, TEntity>(THandler instance, string topic = null)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()
        {
            return Task.Factory.StartNew(() =>
            {
                Subscribe<THandler, TEntity>(instance, topic);
            });
        }

        #region RabbitMQClient

        /// <summary>
        /// RabbitMQ发布
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="eventData"></param>
        protected virtual void RabbitMQClientPublish<THandler, TEntity>(TEntity eventData)
           where THandler : IRemoteEventHandler<TEntity>
           where TEntity : class, new()

        {
            var handlerType = typeof(THandler);
            var buffer = _serializer.MessageToBytes(typeof(TEntity), eventData);

            var topicAttr = handlerType.GetCustomAttributes(typeof(TopicAttribute), false)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();

            // topic特性不为空
            if (topicAttr != null)
            {
                RabbitMQClientPublish(handlerType.FullName, buffer);
                return;
            }


            // 负载均衡模式
            var loadBalancingAttr = handlerType.GetCustomAttributes(typeof(ConnectionLoadBalancingAttribute), false)
                  .Select(o => o as ConnectionLoadBalancingAttribute)
                .FirstOrDefault();
            if (loadBalancingAttr != null)
            {
                var loadBalancingInfo = _rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType.FullName == handlerType.FullName);
                if (loadBalancingInfo != null)
                {
                    RabbitMQClientPublish(loadBalancingInfo.Start(), buffer);
                    return;
                }
            }

            // 普通的工作队列模式
            RabbitMQClientPublish(handlerType.FullName, buffer);
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

            try
            {
                var topicName = topic;
                var body = buffer;


                channel.ExchangeDeclare(exchange: topicName,
                    type: ExchangeType.Fanout,
                    durable: false,
                    autoDelete: false,
                    arguments: null);

                var properties = channel.CreateBasicProperties();
                //properties.Persistent = true;

                // topic和通道关联
                _publisherDictionary[topicName] = channel;

                channel.BasicPublish(
                    exchange: topicName,
                    routingKey: topicName,
                    basicProperties: properties,
                    body: body);
            }
            finally
            {
                //if (connection != null)
                //{
                //    _publisherConnectionsAcquired.Remove(connection);
                //}
            }
        }


        /// <summary>
        /// RabbitMQ订阅
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="instance"></param>
        protected virtual void RabbitMQClientSubscribe<THandler, TEntity>(THandler instance, string topic = null)
                 where THandler : IRemoteEventHandler<TEntity>
                 where TEntity : class, new()
        {
            var handlerType = typeof(THandler);

            // topic模式
            var topicAttr = handlerType.GetCustomAttributes(typeof(TopicAttribute), false)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();
            if (topicAttr != null)
            {
                RabbitMQClientSubscribe(topic ?? handlerType.FullName, (buffer) =>
                  {
                      var data = (TEntity)_serializer.BytesToMessage(typeof(TEntity), buffer);
                      instance.HandleEvent(data);
                  }, false);
                return;
            }

            // 普通的工作队列模式
            RabbitMQClientSubscribe(topic ?? handlerType.FullName, (buffer) =>
            {
                var data = (TEntity)_serializer.BytesToMessage(typeof(TEntity), buffer);
                instance.HandleEvent(data);
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
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="eventData"></param>
        protected virtual void EasyNetQPublish<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()

        {
            var handlerType = typeof(THandler);

            var topicAttr = handlerType.GetCustomAttributes(typeof(TopicAttribute), false)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();

            // topic特性不为空
            if (topicAttr != null)
            {
                _bus.Publish(eventData);
                return;
            }


            // 负载均衡模式
            var loadBalancingAttr = handlerType.GetCustomAttributes(typeof(ConnectionLoadBalancingAttribute), false)
                  .Select(o => o as ConnectionLoadBalancingAttribute)
                .FirstOrDefault();
            if (loadBalancingAttr != null)
            {
                var loadBalancingInfo = _rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType.FullName == handlerType.FullName);
                if (loadBalancingInfo != null)
                {
                    _bus.Send(loadBalancingInfo.Start(), eventData);
                    return;
                }
            }

            // 普通的工作队列模式
            _bus.Send(handlerType.FullName, eventData);

        }

        /// <summary>
        /// EasyNetQ订阅
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="instance"></param>
        protected virtual void EasyNetQSubscribe<THandler, TEntity>(THandler instance, string topic)
                 where THandler : IRemoteEventHandler<TEntity>
                 where TEntity : class, new()
        {
            var handlerType = typeof(THandler);

            // topic模式
            var topicAttr = handlerType.GetCustomAttributes(typeof(TopicAttribute), false)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();
            if (topicAttr != null)
            {
                _bus.Subscribe<TEntity>(Guid.NewGuid().ToString(), instance.HandleEvent);
                return;
            }

            // 普通的工作队列模式
            _bus.Receive<TEntity>(topic ?? handlerType.FullName, (data) =>
              {
                  instance.HandleEvent(data);
              });
        }

        #endregion


        #region 取消订阅

        public virtual void Unsubscribe<THandler>(string topic = null)
             where THandler : class, new()
        {
            var handlerType = typeof(THandler);

            // 负载均衡模式
            var loadBalancingAttr = handlerType.GetCustomAttributes(typeof(ConnectionLoadBalancingAttribute), false)
                  .Select(o => o as ConnectionLoadBalancingAttribute)
                .FirstOrDefault();

            if (loadBalancingAttr != null)
            {
                var loadBalancingInfo = _rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType.FullName == handlerType.FullName);
                if (loadBalancingInfo != null)
                {
                    Unsubscribe(loadBalancingInfo.GetAll());
                    return;
                }
            }

            _dictionary[topic ?? handlerType.FullName].Close();
            _dictionary[topic ?? handlerType.FullName].Dispose();
        }

        public virtual Task UnsubscribeAsync<THandler>(string topic = null)
             where THandler : class, new()
        {
            return Task.Factory.StartNew(() =>
            {
                Unsubscribe<THandler>(topic);
            });
        }

        public virtual void Unsubscribe(string topic)
        {
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
