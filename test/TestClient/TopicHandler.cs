using RemoteEventBus.Attributes;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TopicHandler : ITopicHandler
    {
        private readonly string _name;

        public TopicHandler(string name)
        {
            this._name = name;
        }

        public void HandleEvent(MyEntity data)
        {
            Common.PrintLine($"主题队列 {_name} 收到消息：{data.Content} 消息创建时间：{data.CreationTime}");
        }
    }
}
