using Microsoft.Extensions.DependencyInjection;
using System;
using RemoteEventBus;
using RemoteEventBus.Impl;
using RemoteEventBus.Interface;
using TestCommon;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TestServer
{
    class Program
    {
        static IRemoteEventBus eventBus;

        static void Main(string[] args)
        {
            Common.Init();
            eventBus = Common.IoContainer.GetService<IRemoteEventBus>();
            Common.PrintLine("输入选项开始运行程序", false);
            Common.PrintLine("1: 负载均衡发布模式", false);
            Common.PrintLine("2: 主题发布模式", false);
            Common.PrintLine("3: 工作队列发布模式", false);
            var input = Console.ReadLine();
            switch (input.Trim())
            {
                case "1":
                    LoadBalancingsPublisher();
                    break;
                case "2":
                    TopicPublisher();
                    break;
                case "3":
                    WorkQueuePublisher();
                    break;
                default:
                    break;
            }
            Common.Wait("按任意键结束程序...");
            eventBus.Dispose();
        }


        /// <summary>
        /// 负载均衡发布
        /// </summary>
        static void LoadBalancingsPublisher()
        {
            while (true)
            {
                Common.PrintLine("已进入负载均衡发布者模式\r\n请输入发布数量(必须是数字不能为负数,不能为小数,必须大于0)\r\n输入quit按回车退出");
                var input = Console.ReadLine();
                if (input == "quit")
                {
                    break;
                }

                var tryCount = Convert.ToInt32(input);

                var list = new List<MyEntity>();
                for (int i = 0; i < tryCount; i++)
                {
                    list.Add(new MyEntity
                    {
                        Buffer = Data.Msg001,
                        CreationTime = DateTime.Now
                    });
                }

                Common.Wait("按回车开始发送数据");
                Common.PrintLine("开始发送");

                Parallel.ForEach(list, (item) =>
                {
                    eventBus.Publish(item, TopicConsts.MyLoadBalancing);
                });

                Common.PrintLine("发送完成");
            }
        }

        /// <summary>
        /// 主题发布
        /// </summary>
        static void TopicPublisher()
        {
            Common.PrintLine("已进入主题发布模式");
            while (true)
            {
                Common.PrintLine("输入任意值按回车发布\r\n输入quit按回车退出");
                var input = Console.ReadLine();
                if (input == "quit")
                {
                    break;
                }

                eventBus.Publish<MyEntity>(new MyEntity()
                {
                    Content = input,
                    CreationTime = DateTime.Now,
                }, TopicConsts.MyTopic);

                Common.PrintLine("发送完成");
            }
        }

        /// <summary>
        /// 工作者发布
        /// </summary>
        static void WorkQueuePublisher()
        {
            Common.PrintLine("已进入工作队列发布模式");
            while (true)
            {
                Common.PrintLine("输入任意值按回车发布\r\n输入quit按回车退出");
                var input = Console.ReadLine();
                if (input == "quit")
                {
                    break;
                }

                eventBus.Publish<MyEntity>(new MyEntity()
                {
                    Content = input,
                    CreationTime = DateTime.Now,
                }, TopicConsts.MyWorkQueue);

                Common.PrintLine("发送完成");
            }
        }
    }
}
