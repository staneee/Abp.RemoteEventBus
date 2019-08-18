using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus
{
    public interface IRemoteEventSubscriber : IDisposable
    {
        void Subscribe(IEnumerable<string> topics, Action<string, byte[]> handler);

        Task SubscribeAsync(IEnumerable<string> topics, Action<string, byte[]> handler);

        void Unsubscribe(IEnumerable<string> topics);

        Task UnsubscribeAsync(IEnumerable<string> topics);

        void UnsubscribeAll();

        Task UnsubscribeAllAsync();
    }
}
