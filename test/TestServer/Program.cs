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

                var list = new List<int>();
                for (int i = 0; i < tryCount; i++)
                {
                    list.Add(i);
                }

                Common.Wait("按回车开始发送数据");

                foreach (var item in list)
                {
                    eventBus.Publish<MyHandler002, MyEntity>(new MyEntity
                    {
                        Buffer = Data.Msg001,
                        CreationTime = DateTime.Now
                    });
                }

                //Parallel.ForEach(list, (item) =>
                //{
                //    eventBus.Publish<MyHandler002, MyEntity>(new MyEntity
                //    {
                //        Buffer = Data.Msg001,
                //        CreationTime = DateTime.Now
                //    });
                //});

                Common.PrintLine("发送完成", false);
            }
        }
    }
}
