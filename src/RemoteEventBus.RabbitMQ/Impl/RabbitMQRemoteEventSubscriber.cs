﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemoteEventBus.Interface;

namespace RemoteEventBus.Impl
{
    public class RabbitMQRemoteEventSubscriber : IRemoteEventSubscriber
    {
        private readonly ConcurrentDictionary<string, IModel> _dictionary;
        private readonly List<IConnection> _connectionsAcquired;
        private readonly RabbitMQConnectionPooledObjectFactory _factory;

        protected readonly IRabbitMQSetting _rabbitMQSetting;

        private bool _disposed;

        public RabbitMQRemoteEventSubscriber(IRabbitMQSetting rabbitMQSetting)
        {
            _factory = new RabbitMQConnectionPooledObjectFactory(rabbitMQSetting);
            _dictionary = new ConcurrentDictionary<string, IModel>();
            _connectionsAcquired = new List<IConnection>();
            _rabbitMQSetting = rabbitMQSetting;
        }

        public virtual void Subscribe(IEnumerable<string> topics, Action<string, byte[]> handler)
        {
            var existsTopics = topics.ToList().Where(p => _dictionary.ContainsKey(p));
            if (existsTopics.Any())
            {
                throw new Exception(string.Format("the topics {0} have subscribed already", string.Join(",", existsTopics)));
            }

            foreach (var topic in topics)
            {
                var connection = _factory.Create();
                _connectionsAcquired.Add(connection);
                try
                {
                    var topicName = $"{_rabbitMQSetting.TopicPrefix}_{topic}";
                    var queueName = $"{topic}_{_rabbitMQSetting.QueuePrefix}_queue";

                    var channel = connection.CreateModel();
                    channel.ExchangeDeclare(exchange: topicName,
                        type: ExchangeType.Fanout,
                        durable: false,
                        autoDelete: false,
                        arguments: null);
                    channel.QueueDeclare(queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.QueueBind(queue: queueName,
                        exchange: topicName,
                        routingKey: topicName);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (ch, ea) =>
                    {
                        handler(ea.RoutingKey, ea.Body);


                        channel.BasicAck(ea.DeliveryTag, false);
                    };
                    channel.BasicConsume(queue: queueName,
                        autoAck: false,
                        consumer: consumer);

                    // topic和通道关联
                    _dictionary[topic] = channel;
                }
                finally
                {
                    _connectionsAcquired.Remove(connection);
                }
            }
        }

        public virtual async Task SubscribeAsync(IEnumerable<string> topics, Action<string, byte[]> handler)
        {

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

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                UnsubscribeAll();
                foreach (var connection in _connectionsAcquired)
                {
                    _factory.Destroy(connection);
                }

                _disposed = true;
            }
        }

    }
}
