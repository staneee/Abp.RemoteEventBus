using System;
using System.Collections.Generic;
using System.Text;
using RemoteEventBus.Interface;

namespace TestCommon
{
    public class MyHandler : IRemoteEventHandler<MyEntity>
    {
        private readonly string _name;
        public MyHandler(string name)
        {
            this._name = name;
        }

        public void HandleEvent(MyEntity data)
        {
            //Common.PrintLine($"订阅者 {_name} 收到消息：{data.Content}消息创建时间：{data.CreationTime}");
        }
    }
}
