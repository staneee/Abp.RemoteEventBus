using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemoteEventBus.Impl;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus
{
    public static class RemoteEventBusConfigurationExtension
    {
        /// <summary>
        /// 注册添加远程事件总线
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRemoteEventBus(this IServiceCollection services)
        {
            services.TryAddSingleton<IRemoteEventBus, NullRemoteEventBus>();
            return services;
        }
    }
}
