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

        public void Publish<TEventData>(TEventData eventData, string topic = null) where TEventData : class, new()
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync<TEventData>(TEventData eventData, string topic = null) where TEventData : class, new()
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEventData>(Action<TEventData> invoke, string topic = null) where TEventData : class, new()
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAsync<TEventData>(Action<TEventData> invoke, string topic = null) where TEventData : class, new()
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEventData>(string topic = null) where TEventData : class, new()
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync<TEventData>(string topic = null) where TEventData : class, new()
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
