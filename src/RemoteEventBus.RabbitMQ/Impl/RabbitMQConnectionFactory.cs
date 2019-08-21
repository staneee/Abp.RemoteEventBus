using RabbitMQ.Client;
using RemoteEventBus.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RemoteEventBus.Impl
{
    public class RabbitMQConnectionFactory : IRabbitMQConnectionFactory
    {
        /// <summary>
        /// 连接创建工厂
        /// </summary>
        protected readonly ConnectionFactory _connectionFactory;
        /// <summary>
        /// 所有创建的连接集合
        /// </summary>
        protected List<IConnection> Connections { get; set; }

        public RabbitMQConnectionFactory(IRabbitMQSetting rabbitMQSetting)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMQSetting.ConnectionString),
                AutomaticRecoveryEnabled = rabbitMQSetting.AutomaticRecoveryEnabled
            };

            Connections = new List<IConnection>();
        }

        public virtual IConnection Create()
        {
            var connection = _connectionFactory.CreateConnection();
            Connections.Add(connection);
            return connection;
        }

        public virtual void Destroy(IConnection obj)
        {
            obj?.Dispose();
        }

        public virtual void Dispose()
        {
            // 释放所有连接
            foreach (var item in Connections)
            {
                item?.Dispose();
            }
            Connections.Clear();
        }
    }
}
