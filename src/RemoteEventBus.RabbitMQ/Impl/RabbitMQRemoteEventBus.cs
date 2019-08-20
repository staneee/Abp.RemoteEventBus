using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Impl
{
    public class RabbitMQRemoteEventBus : IRemoteEventBus
    {
        protected readonly IRemoteEventSerializer _remoteEventSerializer;
        protected readonly IRemoteEventPublisher _remoteEventPublisher;
        protected readonly IRemoteEventSubscriber _remoteEventSubscriber;

        public RabbitMQRemoteEventBus(
            IRemoteEventSerializer remoteEventSerializer,
            IRemoteEventPublisher remoteEventPublisher,
            IRemoteEventSubscriber remoteEventSubscriber
            )
        {
            _remoteEventSerializer = remoteEventSerializer;
            _remoteEventPublisher = remoteEventPublisher;
            _remoteEventSubscriber = remoteEventSubscriber;
        }

        public void Publish<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
        {
            var handlerType = typeof(THandler);
            var eventDataBuffer = _remoteEventSerializer.Serialize(eventData);
            _remoteEventPublisher.Publish(handlerType.FullName, eventDataBuffer);
        }

        public void Subscribe<THandler, TEntity>(THandler instance)
            where THandler : IRemoteEventHandler<TEntity>
        {
            var handlerType = typeof(THandler);
            _remoteEventSubscriber.Subscribe(new string[] { handlerType.FullName }, (topic, buffer) =>
            {
                var entity = _remoteEventSerializer.Deserialize<TEntity>(buffer);
                instance.HandleEvent(topic, entity);
            });
        }

        public void Dispose()
        {

        }
    }
}
