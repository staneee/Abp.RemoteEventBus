namespace Abp.RemoteEventBus.RabbitMQ
{
    public interface IRabbitMQSetting
    {
        /// <summary>
        /// 
        /// </summary>
        bool AutomaticRecoveryEnabled { get; set; }

        /// <summary>
        /// RabbitMQ链接字符串
        /// </summary>
        string Url { get; set; }

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
    }
}