using Microsoft.Extensions.DependencyInjection;
using System;
using RemoteEventBus;
using RemoteEventBus.Impl;
using RemoteEventBus.Interface;
using TestCommon;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Common.Init();

            var handelrType = typeof(MyHandler002);

            var eventBus = Common.IoContainer.GetService<IRemoteEventBus>();



            var rabbitMQSetting = Common.IoContainer.GetService<IRabbitMQSetting>();
            var loadBalancing = rabbitMQSetting.LoadBalancings.Find(o => o.HandlerType.FullName == handelrType.FullName);
            var allKey = loadBalancing.GetAll();


            var handlerList = new List<MyHandler002>();
            for (int i = 0; i < loadBalancing.MaxSize; i++)
            {
                var handler = new MyHandler002(i.ToString());
                handlerList.Add(handler);
            }

            Parallel.ForEach(handlerList, (item, state, index) =>
            {
                eventBus.Subscribe<MyHandler002, MyEntity>(item, allKey[int.Parse(index.ToString())]);
            });

            Common.Wait("客户端已启动...");
            foreach (var handler in handlerList)
            {
                if (!handler.Start.HasValue || !handler.Stop.HasValue)
                {
                    continue;
                }
                Common.PrintLine($"消费者:{handler.Name} 消费数量{handler.Count} 开始时间:{handler.Start}  结束时间:{handler.Stop}  总用时:{handler.Stop - handler.Start}");
            }
            Common.Wait("按任意键结束程序...");

        }
    }
}
