using RemoteEventBus.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    /// <summary>
    /// 普通工作队列Dto
    /// </summary>
    public class MyWorkQueueEntity
    {
        public string Content { get; set; }

        public DateTime CreationTime { get; set; }

        public byte[] Buffer { get; set; }

    }
}
