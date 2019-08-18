using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus
{
    /// <summary>
    /// 订阅者接口
    /// </summary>
    public interface IRemoteEventSubscriber : IDisposable
    {
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="handler"></param>
        void Subscribe(IEnumerable<string> topics, Action<string, byte[]> handler);

        /// <summary>
        /// 订阅 (异步)
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        Task SubscribeAsync(IEnumerable<string> topics, Action<string, byte[]> handler);

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="topics"></param>
        void Unsubscribe(IEnumerable<string> topics);

        /// <summary>
        /// 解绑 (异步)
        /// </summary>
        /// <param name="topics"></param>
        /// <returns></returns>
        Task UnsubscribeAsync(IEnumerable<string> topics);

        /// <summary>
        /// 解绑所有
        /// </summary>
        void UnsubscribeAll();

        /// <summary>
        /// 解绑所有(异步)
        /// </summary>
        /// <returns></returns>
        Task UnsubscribeAllAsync();
    }
}
