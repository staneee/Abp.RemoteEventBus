using Microsoft.Extensions.DependencyInjection;
using System;
using RemoteEventBus;
using RemoteEventBus.Impl;
using RemoteEventBus.Interface;
using TestCommon;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Common.Init();

            var eventBus = Common.IoContainer.GetService<IRemoteEventBus>();
            eventBus.Subscribe<MyHandler, MyEntity>(new MyHandler("a"));
            //eventBus.Subscribe<MyHandler, MyEntity>(new MyHandler("b"));
            //eventBus.Subscribe<MyHandler, MyEntity>(new MyHandler("c"));
            Common.Wait("客户端已启动...");


        }
    }
}
