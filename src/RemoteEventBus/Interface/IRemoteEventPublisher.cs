using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Interface
{
    /// <summary>
    /// 发布者
    /// </summary>
    public interface IRemoteEventPublisher : IDisposable
    {
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="remoteEventData"></param>
        void Publish(string topic, byte[] remoteEventData);

        /// <summary>
        /// 发布(异步)
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="remoteEventData"></param>
        /// <returns></returns>
        Task PublishAsync(string topic, byte[] remoteEventData);
    }
}
