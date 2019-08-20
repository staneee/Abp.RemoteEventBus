using Commons.Pool;
using RabbitMQ.Client;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus
{
    public class RabbitMQConnectionPooledObjectFactory : IPooledObjectFactory<IConnection>
    {
        private ConnectionFactory _connectionFactory;

        public RabbitMQConnectionPooledObjectFactory(IRabbitMQSetting rabbitMQSetting)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMQSetting.Url),
                AutomaticRecoveryEnabled = true
            };
        }

        public virtual IConnection Create()
        {
            return _connectionFactory.CreateConnection();
        }

        public virtual void Destroy(IConnection obj)
        {
            obj.Dispose();
        }
    }
}