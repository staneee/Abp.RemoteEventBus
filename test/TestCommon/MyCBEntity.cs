using RemoteEventBus.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    [ConnectionLoadBalancing]
    public class MyCBEntity
    {
        public string Content { get; set; }

        public DateTime CreationTime { get; set; }

        public byte[] Buffer { get; set; }

    }
}
