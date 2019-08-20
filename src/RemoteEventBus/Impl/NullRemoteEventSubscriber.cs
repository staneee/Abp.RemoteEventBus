using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Impl
{
    public class NullRemoteEventSubscriber : IRemoteEventSubscriber
    {
        public static NullRemoteEventSubscriber Instance { get { return SingletonInstance; } }
        private static readonly NullRemoteEventSubscriber SingletonInstance = new NullRemoteEventSubscriber();
        private NullRemoteEventSubscriber()
        {

        }

        public void Subscribe(IEnumerable<string> topics, Action<string, byte[]> handler)
        {

        }

        public Task SubscribeAsync(IEnumerable<string> topics, Action<string, byte[]> handler)
        {
            return Task.FromResult(0);
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {

        }

        public void UnsubscribeAll()
        {

        }

        public Task UnsubscribeAllAsync()
        {
            return Task.FromResult(0);
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return Task.FromResult(0);
        }

        public void Dispose()
        {

        }
    }
}
