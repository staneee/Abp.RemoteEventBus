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


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Publish<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()
        {
            throw new NotImplementedException();
        }

        public void Subscribe<THandler, TEntity>(THandler instance)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()
        {
            throw new NotImplementedException();
        }

        public void Subscribe<THandler, TEntity>(THandler instance, string topic)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAsync<THandler, TEntity>(THandler instance, string topic = null)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<THandler>(string topic = null) where THandler : class, new()
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync<THandler>(string topic = null) where THandler : class, new()
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
    }
}
