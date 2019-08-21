using System;
using System.Collections.Generic;
using System.Text;
using RemoteEventBus.Interface;

namespace TestCommon
{
    public class WorkQueueHandler : IWorkQueueHandler
    {
        private readonly string _name;
        public WorkQueueHandler(string name)
        {
            this._name = name;
        }

        public void HandleEvent(MyEntity data)
        {
            Common.PrintLine($"工作队列 {_name} 收到消息：{data.Content} 消息创建时间：{data.CreationTime}");
        }
    }
}
