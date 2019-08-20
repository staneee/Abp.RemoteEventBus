using Microsoft.Extensions.DependencyInjection;
using System;
using RemoteEventBus;
using RemoteEventBus.Impl;
using RemoteEventBus.Interface;
using TestCommon;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Common.Init();

            var eventBus = Common.IoContainer.GetService<IRemoteEventBus>();
            Common.PrintLine("输入 quit 退出程序,按回车发送消息", false);
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "quit")
                {
                    break;
                }
                eventBus.Publish<MyHandler, MyEntity>(new MyEntity
                {
                    Content = input,
                    CreationTime = DateTime.Now
                });
            }
        }
    }
}
