using Abp.Dependency;
using Commons.Pool;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus.RabbitMQ
{
    /// <summary>
    /// RabbitMQ 连接池
    /// </summary>
    public class RabbitMQConnectionPooledObjectFactory : IPooledObjectFactory<IConnection>
    {
        private ConnectionFactory _connectionFactory;

        public RabbitMQConnectionPooledObjectFactory(IRabbitMQSetting rabbitMQSetting)
        {
            Check.NotNullOrWhiteSpace(rabbitMQSetting.Url, "Url");
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMQSetting.Url),
                AutomaticRecoveryEnabled = true
            };
        }

        public IConnection Create()
        {
            return _connectionFactory.CreateConnection();
        }

        public void Destroy(IConnection obj)
        {
            obj.Dispose();
        }
    }
}
