using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Interface
{
    /// <summary>
    /// RabbitMQ连接创建工厂
    /// </summary>
    public interface IRabbitMQConnectionFactory : IDisposable
    {
        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <returns></returns>
        IConnection Create();

        /// <summary>
        /// 释放一个连接
        /// </summary>
        /// <param name="obj"></param>
        void Destroy(IConnection obj);
    }
}
