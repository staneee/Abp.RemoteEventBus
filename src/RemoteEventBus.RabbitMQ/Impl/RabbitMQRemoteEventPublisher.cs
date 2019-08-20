using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Commons.Pool;
using RabbitMQ.Client;
using RemoteEventBus.Interface;

namespace RemoteEventBus.Impl
{
    public class RabbitMQRemoteEventPublisher : IRemoteEventPublisher
    {
        protected readonly IObjectPool<IConnection> _connectionPool;
        protected readonly IRabbitMQSetting _rabbitMQSetting;


        public RabbitMQRemoteEventPublisher(
            IPoolManager poolManager,
            IRemoteEventSerializer remoteEventSerializer,
            IRabbitMQSetting rabbitMQSetting
            )
        {
            _connectionPool = poolManager.NewPool<IConnection>()
                                    .InitialSize(rabbitMQSetting.InitialSize)
                                    .MaxSize(rabbitMQSetting.MaxSize)
                                    .WithFactory(new RabbitMQConnectionPooledObjectFactory(rabbitMQSetting))
                                    .Instance();
            _rabbitMQSetting = rabbitMQSetting;

        }

        public virtual void Publish(string topic, byte[] remoteEventData)
        {
            var connection = _connectionPool.Acquire();
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

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(
                    exchange: topicName,
                    routingKey: topicName,
                    basicProperties: properties,
                    body: remoteEventData);
            }
            finally
            {
                _connectionPool.Return(connection);
            }
        }

        public virtual Task PublishAsync(string topic, byte[] remoteEventData)
        {
            return Task.Factory.StartNew(() =>
            {
                Publish(topic, remoteEventData);
            });
        }

        public virtual void Dispose()
        {

        }
    }
}
