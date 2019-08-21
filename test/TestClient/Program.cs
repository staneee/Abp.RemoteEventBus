using Microsoft.Extensions.DependencyInjection;
using System;
using RemoteEventBus;
using RemoteEventBus.Impl;
using RemoteEventBus.Interface;
using TestCommon;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TestClient
{
    class Program
    {
        static IRemoteEventBus eventBus;

        static void Main(string[] args)
        {
            Common.Init();
            eventBus = Common.IoContainer.GetService<IRemoteEventBus>();
            Common.PrintLine("输入选项开始运行程序", false);
            Common.PrintLine("1: 负载均衡订阅", false);
            Common.PrintLine("2: 主题订阅", false);
            Common.PrintLine("3: 工作队列订阅", false);
            var input = Console.ReadLine();
            switch (input.Trim())
            {
                case "1":
                    LoadBalancingsSubscriber();
                    break;
                case "2":
                    TopicSubscriber();
                    break;
                case "3":
                    WorkQueueSubscriber();
                    break;
                default:
                    break;
            }
            Common.Wait("按任意键结束程序...");
            eventBus.Dispose();
        }

        /// <summary>
        /// 负载均衡订阅
        /// </summary>
        static void LoadBalancingsSubscriber()
        {
            var handelrType = typeof(LoadBalancingHandler);

            var rabbitMQSetting = Common.IoContainer.GetService<IRabbitMQSetting>();
            var loadBalancing = rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType == handelrType.Name);
            var allKey = loadBalancing.GetAll();


            var handlerList = new List<LoadBalancingHandler>();
            for (int i = 0; i < loadBalancing.MaxSize; i++)
            {
                var handler = new LoadBalancingHandler(i.ToString());
                handlerList.Add(handler);
            }

            Parallel.ForEach(handlerList, (item, state, index) =>
            {
                eventBus.Subscribe<LoadBalancingHandler, MyEntity>(item, allKey[int.Parse(index.ToString())]);
            });

            Common.Wait($"{loadBalancing.MaxSize}个消费者已启动...");
            foreach (var handler in handlerList)
            {
                if (!handler.Start.HasValue || !handler.Stop.HasValue)
                {
                    continue;
                }
                Common.PrintLine($"消费者:{handler.Name} 消费数量{handler.Count} 开始时间:{handler.Start}  结束时间:{handler.Stop}  总用时:{handler.Stop - handler.Start}");
            }
        }

        /// <summary>
        /// 主题订阅
        /// </summary>
        static void TopicSubscriber()
        {
            var pid = Process.GetCurrentProcess().Id;
            eventBus.Subscribe<TopicHandler, MyEntity>(new TopicHandler(pid.ToString()));
            Common.Wait("主题消费者已启动...");
        }

        /// <summary>
        /// 工作者订阅
        /// </summary>
        static void WorkQueueSubscriber()
        {
            var pid = Process.GetCurrentProcess().Id;
            eventBus.Subscribe<WorkQueueHandler, MyEntity>(new WorkQueueHandler(pid.ToString()));
            Common.Wait("工作队列消费者已启动...");
        }
    }
}
