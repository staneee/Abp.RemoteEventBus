using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Impl
{
    public class NullRemoteEventBus : IRemoteEventBus
    {
        public static NullRemoteEventBus Instance { get { return SingletonInstance; } }
        private static readonly NullRemoteEventBus SingletonInstance = new NullRemoteEventBus();

        private NullRemoteEventBus()
        {

        }

        public void Publish<TEventData>(string topic, TEventData eventData) where TEventData : class
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync<TEventData>(string topic, TEventData eventData) where TEventData : class
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEventData>(string topic, Action<TEventData> invoke) where TEventData : class
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAsync<TEventData>(string topic, Action<TEventData> invoke) where TEventData : class
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(string topic)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync(string topic)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll()
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAllAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
