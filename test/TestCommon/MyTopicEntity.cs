using RemoteEventBus.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    /// <summary>
    /// Topic消息Dto
    /// </summary>
    [Topic]
    public class MyTopicEntity
    {
        public string Content { get; set; }

        public DateTime CreationTime { get; set; }

        public byte[] Buffer { get; set; }

    }
}
