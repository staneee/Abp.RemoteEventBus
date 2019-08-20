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
        public static IServiceCollection AddRemoteEventBus(this IServiceCollection services)
        {
            services.TryAddSingleton(typeof(IRemoteEventSerializer), typeof(JsonRemoteEventSerializer));
            services.TryAddSingleton(typeof(IRemoteEventBus), typeof(NullRemoteEventBus));
            services.TryAddSingleton(typeof(IRemoteEventPublisher), typeof(NullRemoteEventPublisher));
            services.TryAddSingleton(typeof(IRemoteEventSubscriber), typeof(NullRemoteEventSubscriber));

            return services;
        }
    }
}
