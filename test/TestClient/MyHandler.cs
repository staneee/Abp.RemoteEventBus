using System;
using System.Collections.Generic;
using System.Text;
using RemoteEventBus.Interface;
using TestCommon;

namespace TestClient
{
    public class MyHandler : IRemoteEventHandler<MyCBEntity>,
        IRemoteEventHandler<MyTopicEntity>,
        IRemoteEventHandler<MyWorkQueueEntity>
    {
        public string Name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? Stop { get; set; }

        public long Count { get; set; }

        public MyHandler(string name)
        {
            this.Name = name;
        }

        public void HandleEvent(MyCBEntity data)
        {
            if (!Start.HasValue)
            {
                Start = DateTime.Now;
            }

            Stop = DateTime.Now;
            Count++;
        }

        public void HandleEvent(MyTopicEntity data)
        {
            Common.PrintLine($"主题队列 {Name} 收到消息：{data.Content} 消息创建时间：{data.CreationTime}");
        }

        public void HandleEvent(MyWorkQueueEntity data)
        {
            Common.PrintLine($"工作队列 {Name} 收到消息：{data.Content} 消息创建时间：{data.CreationTime}");
        }
    }
}
