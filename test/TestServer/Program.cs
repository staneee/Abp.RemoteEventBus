using Microsoft.Extensions.DependencyInjection;
using System;
using RemoteEventBus;
using RemoteEventBus.Impl;
using RemoteEventBus.Interface;
using TestCommon;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                //foreach (var item in list)
                //{
                //    eventBus.Publish<MyHandler002, MyEntity>(item);
                //}

                Parallel.ForEach(list, (item) =>
                {
                    eventBus.Publish<MyHandler002, MyEntity>(item);
                });

                Common.PrintLine("发送完成");
            }
        }
    }
}
