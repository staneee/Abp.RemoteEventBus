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

        public void Publish<THandler, TEntity>(TEntity eventData) where THandler : IRemoteEventHandler<TEntity>
        {
            throw new NotImplementedException();
        }

        public void Subscribe<THandler, TEntity>(THandler instance) where THandler : IRemoteEventHandler<TEntity>
        {
            throw new NotImplementedException();
        }
    }
}
