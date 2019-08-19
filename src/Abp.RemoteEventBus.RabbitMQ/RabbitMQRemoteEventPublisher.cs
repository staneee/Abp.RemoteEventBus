using Commons.Pool;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus.RabbitMQ
{
    public class RabbitMQRemoteEventPublisher : IRemoteEventPublisher
    {
        private readonly IObjectPool<IConnection> _connectionPool;

        private readonly IRemoteEventSerializer _remoteEventSerializer;
        private readonly IRabbitMQSetting _rabbitMQSetting;

        private readonly string _topicPrefix = "";
        private readonly string _queuePrefix = "";

        private bool _disposed;

        public RabbitMQRemoteEventPublisher(
            IPoolManager poolManager,
            IRabbitMQSetting rabbitMQSetting,
            IRemoteEventSerializer remoteEventSerializer
            )
        {
            _remoteEventSerializer = remoteEventSerializer;

            _connectionPool = poolManager.NewPool<IConnection>()
                                    .InitialSize(rabbitMQSetting.InitialSize)
                                    .MaxSize(rabbitMQSetting.MaxSize)
                                    .WithFactory(new RabbitMQConnectionPooledObjectFactory(rabbitMQSetting))
                                    .Instance();

            _rabbitMQSetting = rabbitMQSetting;

            _topicPrefix = rabbitMQSetting.TopicPrefix;
            _queuePrefix = rabbitMQSetting.QueuePrefix;

        }

        public void Publish(string topic, IRemoteEventData remoteEventData)
        {
            var connection = _connectionPool.Acquire();
            try
            {
                var topicName = $"{_topicPrefix}_{topic}";
                var queueName = $"{topic}_{_queuePrefix}_queue";
                var body = _remoteEventSerializer.Serialize(remoteEventData);


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
                    body: body);
            }
            finally
            {
                _connectionPool.Return(connection);
            }
        }

        public Task PublishAsync(string topic, IRemoteEventData remoteEventData)
        {
            return Task.Factory.StartNew(() =>
            {
                Publish(topic, remoteEventData);
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _connectionPool.Dispose();

                _disposed = true;
            }
        }
    }
}
