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

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            var services = CreateServiceCollection();
            services.AddRemoteEventBusRabbitMQ(() =>
            {
                var setting = new RabbitMQSetting()
                {
                    //ConnectionString = "host=10.3.2.154;username=root;password=ms877350"
                    ConnectionString = "amqp://root:ms877350@10.3.2.154:5672/"
                };

                // 配置负载均衡
                setting.LoadBalancings.Add(new RabbitMQConnectionLoadBalancing(TopicConsts.MyLoadBalancing)
                {
                    MaxSize = 30,
                });
                // 负载均衡初始化
                setting.LoadBalancings.ForEach(item =>
                {
                    item.Initialize();
                });

                return setting;
            });
            BuildIoContainer(services);
        }


        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="hasDateTime"></param>
        public static void PrintLine(string msg, bool hasDateTime = true)
        {
            if (hasDateTime)
            {
                Console.WriteLine($"{DateTime.Now} {msg}");
                return;
            }
            Console.WriteLine(msg);
        }

        /// <summary>
        /// 打印并等待
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="hasDateTime"></param>
        public static void Wait(string msg, bool hasDateTime = false)
        {
            PrintLine(msg, hasDateTime);
            Console.ReadKey();
        }
    }
}
