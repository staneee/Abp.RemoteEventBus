using RemoteEventBus.Attributes;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using EasyNetQ;

namespace RemoteEventBus.Impl
{
    public class RabbitMQRemoteEventBus : IRemoteEventBus
    {
        protected readonly IBus _bus;
        protected readonly IRabbitMQSetting _rabbitMQSetting;


        public RabbitMQRemoteEventBus(
            IRabbitMQSetting rabbitMQSetting
            )
        {
            _rabbitMQSetting = rabbitMQSetting;
            _bus = RabbitHutch.CreateBus(_rabbitMQSetting.ConnectionString);
            //_bus = RabbitHutch.CreateBus(_rabbitMQSetting.ConnectionString, (register) =>
            //{
            //    // 替换默认的序列化器
            //    register.Register<ISerializer>((a) =>
            //    {
            //        return null;
            //    });
            //});
        }

        public void Publish<THandler, TEntity>(TEntity eventData)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()

        {
            var handlerType = typeof(THandler);

            var topicAttr = handlerType.GetCustomAttributes(typeof(TopicAttribute), false)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();

            // topic特性不为空
            if (topicAttr != null)
            {
                _bus.Publish(eventData);
                return;
            }


            // 负载均衡模式
            var loadBalancingAttr = handlerType.GetCustomAttributes(typeof(ConnectionLoadBalancingAttribute), false)
                  .Select(o => o as ConnectionLoadBalancingAttribute)
                .FirstOrDefault();
            if (loadBalancingAttr != null)
            {
                var loadBalancingInfo = _rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType.FullName == handlerType.FullName);
                if (loadBalancingInfo != null)
                {
                    _bus.Send(loadBalancingInfo.Start(), eventData);
                    return;
                }
            }

            // 普通的工作队列模式
            _bus.Send(handlerType.FullName, eventData);

        }

        public void Subscribe<THandler, TEntity>(THandler instance)
            where THandler : IRemoteEventHandler<TEntity>
            where TEntity : class, new()
        {
            var handlerType = typeof(THandler);

            // topic模式
            var topicAttr = handlerType.GetCustomAttributes(typeof(TopicAttribute), false)
                .Select(o => o as TopicAttribute)
                .FirstOrDefault();
            if (topicAttr != null)
            {
                _bus.Subscribe<TEntity>(Guid.NewGuid().ToString(), instance.HandleEvent);
                return;
            }

            // 负载均衡模式
            var loadBalancingAttr = handlerType.GetCustomAttributes(typeof(ConnectionLoadBalancingAttribute), false)
                  .Select(o => o as ConnectionLoadBalancingAttribute)
                .FirstOrDefault();
            if (loadBalancingAttr != null)
            {
                var loadBalancingInfo = _rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType.FullName == handlerType.FullName);
                if (loadBalancingInfo != null)
                {
                    _bus.Receive<TEntity>(loadBalancingInfo.Start(), (data) =>
                    {
                        instance.HandleEvent(data);
                    });
                    return;
                }
            }

            // 普通的工作队列模式
            _bus.Receive<TEntity>(handlerType.FullName, (data) =>
            {
                instance.HandleEvent(data);
            });
        }


        public void Dispose()
        {

        }
    }
}
