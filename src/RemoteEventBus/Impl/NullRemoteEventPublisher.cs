using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Impl
{
    public class NullRemoteEventPublisher : IRemoteEventPublisher
    {
        public static NullRemoteEventPublisher Instance { get { return SingletonInstance; } }
        private static readonly NullRemoteEventPublisher SingletonInstance = new NullRemoteEventPublisher();
        private NullRemoteEventPublisher()
        {

        }

        public virtual void Publish(string topic, byte[] remoteEventData)
        {

        }

        public virtual Task PublishAsync(string topic, byte[] remoteEventData)
        {
            return Task.FromResult(0);
        }


        public void Dispose()
        {

        }

    }
}
