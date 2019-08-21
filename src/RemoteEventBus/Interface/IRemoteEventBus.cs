using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Interface
{
    /// <summary>
    /// 远程事件总线 (单例)
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
        /// 发布消息到订阅的处理器 同步
        /// </summary>
        /// <typeparam name="THandler">处理器类型</typeparam>
        /// <param name="eventData">消息实例</param>
        Task PublishAsync<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new();

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T">订阅处理器类型</typeparam>
        /// <typeparam name="THandler">订阅处理器类型</typeparam>
        /// <typeparam name="TEntity">响应数据类型</typeparam>
        /// <param name="instance">处理器实例</param>
        /// <param name="topic">topic</param>
        void Subscribe<THandler, TEntity>(THandler instance, string topic = null)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new();

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T">订阅处理器类型</typeparam>
        /// <typeparam name="THandler">订阅处理器类型</typeparam>
        /// <typeparam name="TEntity">响应数据类型</typeparam>
        /// <param name="instance">处理器实例</param>
        /// <param name="topic">topic</param>
        Task SubscribeAsync<THandler, TEntity>(THandler instance, string topic = null)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new();

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="THandler">订阅处理器类型</typeparam>
        /// <param name="topic">topic</param>
        void Unsubscribe<THandler>(string topic = null)
                where THandler : class, new();

        /// <summary>
        /// 取消订阅 异步
        /// </summary>
        /// <typeparam name="THandler">订阅处理器类型</typeparam>
        /// <param name="topic">topic</param>
        Task UnsubscribeAsync<THandler>(string topic = null)
             where THandler : class, new();


        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="topic">topic</param>
        void Unsubscribe(string topic);

        /// <summary>
        /// 取消订阅 异步
        /// </summary>
        /// <param name="topic">topic</param>
        Task UnsubscribeAsync(string topic);

        /// <summary>
        /// 取消一批订阅
        /// </summary>
        /// <param name="topics"></param>
        void Unsubscribe(IEnumerable<string> topics);

        /// <summary>
        /// 取消一批订阅 异步
        /// </summary>
        /// <param name="topics"></param>
        /// <returns></returns>
        Task UnsubscribeAsync(IEnumerable<string> topics);

        /// <summary>
        /// 取消所有订阅
        /// </summary>
        void UnsubscribeAll();

        /// <summary>
        /// 取消所有订阅 异步
        /// </summary>
        /// <returns></returns>
        Task UnsubscribeAllAsync();
    }
}
