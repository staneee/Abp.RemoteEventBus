using RemoteEventBus.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class MyWorkQueueEntity
    {
        public string Content { get; set; }

        public DateTime CreationTime { get; set; }

        public byte[] Buffer { get; set; }

    }
}
