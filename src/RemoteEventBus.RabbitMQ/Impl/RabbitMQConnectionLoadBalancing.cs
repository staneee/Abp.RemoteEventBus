using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Impl
{
    public class RabbitMQConnectionLoadBalancing : IRabbitMQConnectionLoadBalancing
    {
        public Type HandlerType { get; set; }
        public int MaxSize { get; set; }
    }
}
