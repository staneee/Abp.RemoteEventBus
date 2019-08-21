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
        /// 自动重连,默认为true
        /// </summary>
        bool AutomaticRecoveryEnabled { get; set; }

        /// <summary>
        /// RabbitMQ/EasyNetQ 链接字符串
        /// </summary>
        string ConnectionString { get; set; }

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

        /// <summary>
        /// 使用easynetq,默认使用rabbitmq原生sdk
        /// </summary>
        bool UseEasyNetQ { get; set; }
    }
}