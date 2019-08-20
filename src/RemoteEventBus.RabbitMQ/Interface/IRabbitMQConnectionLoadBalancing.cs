using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Interface
{
    public interface IRabbitMQConnectionLoadBalancing
    {
        /// <summary>
        /// 处理器的类型
        /// </summary>
        Type HandlerType { get; set; }

        /// <summary>
        /// 处理器负载最大数量
        /// </summary>
        int MaxSize { get; set; }
    }
}
