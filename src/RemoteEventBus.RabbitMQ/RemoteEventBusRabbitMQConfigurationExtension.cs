using Commons.Pool;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemoteEventBus.Impl;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus
{
    /// <summary>
    /// 远程事件总线 - RabbitMQ实现
    /// </summary>
    public static class RemoteEventBusRabbitMQConfigurationExtension
    {
        public static IServiceCollection AddRemoteEventBusRabbitMQ(
            this IServiceCollection services,
            Func<RabbitMQSetting> rabbitMQSetting)
        {
            services.TryAddSingleton<IPoolManager, PoolManager>();

            services.TryAddSingleton(typeof(IRabbitMQSetting), (serviceProvider) =>
            {
                return rabbitMQSetting.Invoke();
            });

            services.TryAddSingleton(typeof(IRemoteEventBus), typeof(RabbitMQRemoteEventBus));
            services.TryAddSingleton(typeof(IRemoteEventPublisher), typeof(RabbitMQRemoteEventPublisher));
            services.TryAddSingleton(typeof(IRemoteEventSubscriber), typeof(RabbitMQRemoteEventSubscriber));


            return services.AddRemoteEventBus();
        }
    }
}
