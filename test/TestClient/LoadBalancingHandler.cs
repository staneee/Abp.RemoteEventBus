using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RemoteEventBus.Attributes;
using RemoteEventBus.Interface;

namespace TestCommon
{
    public class LoadBalancingHandler : IRemoteEventHandler<MyEntity>
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

        public LoadBalancingHandler(string name)
        {
            this.Name = name;
            Count = 0;
        }

        public void HandleEvent(MyEntity data)
        {
            if (!Start.HasValue)
            {
                Start = DateTime.Now;
            }

            Stop = DateTime.Now;
            Count++;
        }
    }
}
