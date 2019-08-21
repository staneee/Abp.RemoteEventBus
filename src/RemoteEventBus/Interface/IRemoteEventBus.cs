using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Interface
{
    /// <summary>
    /// 远程事件总线
    /// </summary>
    public interface IRemoteEventBus : IDisposable
    {
        /// <summary>
        /// 发布消息到订阅的处理器 同步
        /// </summary>
        /// <typeparam name="THandler">处理器类型</typeparam>
        /// <param name="eventData">消息实例</param>
        void Publish<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new();

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T">订阅处理器类型</typeparam>
        void Subscribe<THandler, TEntity>(THandler instance)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new();
    }
}
