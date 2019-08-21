using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RemoteEventBus.Attributes;
using RemoteEventBus.Interface;

namespace TestCommon
{
    [ConnectionLoadBalancing]
    public class MyHandler002 : IRemoteEventHandler<MyEntity>
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

        public MyHandler002(string name)
        {
            this.Name = name;
            Count = 0;
        }

        public void HandleEvent(MyEntity data)
        {
            //Common.PrintLine($"订阅者 {_name} 收到消息：{data.Content}消息创建时间：{data.CreationTime}");
            if (!Start.HasValue)
            {
                Start = DateTime.Now;
            }

            Stop = DateTime.Now;
            Count++;
            //Thread.Sleep(1000);
        }
    }
}
