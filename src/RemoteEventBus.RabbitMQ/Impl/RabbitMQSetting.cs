using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Impl
{
    public class RabbitMQSetting : IRabbitMQSetting
    {
        public bool AutomaticRecoveryEnabled { get; set; }

        public string ConnectionString { get; set; }

        public int InitialSize { get; set; }

        public int MaxSize { get; set; }

        public string TopicPrefix { get; set; }

        public string QueuePrefix { get; set; }

        public List<IRabbitMQConnectionLoadBalancing> LoadBalancings { get; set; }

        public RabbitMQSetting()
        {
            AutomaticRecoveryEnabled = true;
            InitialSize = 0;
            MaxSize = 30;
            TopicPrefix = string.Empty;
            QueuePrefix = string.Empty;
            LoadBalancings = new List<IRabbitMQConnectionLoadBalancing>();
        }
    }
}
