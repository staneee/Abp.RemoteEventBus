using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Interface
{
    /// <summary>
    /// RabbitMQ配置
    /// </summary>
    public interface IRabbitMQSetting
    {
        /// <summary>
        /// 
        /// </summary>
        bool AutomaticRecoveryEnabled { get; set; }

        /// <summary>
        /// RabbitMQ链接字符串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 连接池初始化大小
        /// </summary>
        int InitialSize { get; set; }

        /// <summary>
        /// 连接池最大数量
        /// </summary>
        int MaxSize { get; set; }

        /// <summary>
        /// 主题前缀，默认空
        /// </summary>
        string TopicPrefix { get; set; }

        /// <summary>
        /// 队列前缀，默认空
        /// </summary>
        string QueuePrefix { get; set; }

        /// <summary>
        /// 连接负载均衡配置
        /// </summary>
        List<IRabbitMQConnectionLoadBalancing> LoadBalancings { get; set; }
    }
}