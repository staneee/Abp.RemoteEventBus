using EasyNetQ;
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
        /// <summary>
        /// 添加注册RabbitMQ实现的远程事件总线
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rabbitMQSetting"></param>
        /// <returns></returns>
        public static IServiceCollection AddRemoteEventBusRabbitMQ(
            this IServiceCollection services,
            Func<RabbitMQSetting> rabbitMQSetting)
        {

            services.TryAddSingleton(typeof(IRabbitMQSetting), (serviceProvider) =>
            {
                return rabbitMQSetting.Invoke();
            });

            services.TryAddSingleton<IRemoteEventBus, RabbitMQRemoteEventBus>();
            services.TryAddSingleton<ISerializer, JsonSerializer>();
            services.TryAddSingleton<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();

            return services.AddRemoteEventBus();
        }
    }
}
