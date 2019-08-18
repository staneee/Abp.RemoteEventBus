using Commons.Pool;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus.RabbitMQ
{
    public class RabbitMQRemoteEventPublisher : IRemoteEventPublisher
    {
        private const string _exchangeTopic = "RemoteEventBus.Exchange.Topic";

        private readonly IObjectPool<IConnection> _connectionPool;

        private readonly IRemoteEventSerializer _remoteEventSerializer;
        private readonly IRabbitMQSetting _rabbitMQSetting;

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
        }

        public void Publish(string topic, IRemoteEventData remoteEventData)
        {
            var connection = _connectionPool.Acquire();
            try
            {
                var body = _remoteEventSerializer.Serialize(remoteEventData);

                var channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: topic,
                    type: ExchangeType.Fanout,
                    durable: false,
                    autoDelete: false,
                    arguments: null);
                channel.QueueDeclare(queue: $"{topic}_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                channel.BasicPublish(
                    exchange: topic,
                    routingKey: topic,
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
