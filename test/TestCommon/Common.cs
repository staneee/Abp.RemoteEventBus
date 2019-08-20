using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemoteEventBus;
using RemoteEventBus.Impl;

namespace TestCommon
{
    public static class Common
    {
        /// <summary>
        /// Ioc容器
        /// </summary>
        public static IServiceProvider IoContainer { get; private set; }




        /// <summary>
        /// 创建服务注册器
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        /// <summary>
        /// 生成Ioc容器
        /// </summary>
        /// <param name="services"></param>
        public static void BuildIoContainer(this IServiceCollection services)
        {
            IoContainer = services.BuildServiceProvider();
        }

        public static void Init()
        {
            var services = CreateServiceCollection();
            services.AddRemoteEventBusRabbitMQ(() =>
            {
                var setting = new RabbitMQSetting()
                {
                    Url = "amqp://root:ms877350@10.3.2.154:5672/"
                };

                // 配置负载均衡
                setting.LoadBalancings.Add(new RabbitMQConnectionLoadBalancing()
                {
                    HandlerType = typeof(MyHandler),
                    MaxSize = 30,
                });

                return setting;
            });
            BuildIoContainer(services);
        }


        public static void PrintLine(string msg, bool hasDateTime = true)
        {
            if (hasDateTime)
            {
                Console.WriteLine($"{DateTime.Now} {msg}");
                return;
            }
            Console.WriteLine(msg);
        }

        public static void Wait(string msg, bool hasDateTime = false)
        {
            PrintLine(msg, hasDateTime);
            Console.ReadKey();
        }
    }
}
