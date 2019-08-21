using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Impl
{
    public class RabbitMQSetting : IRabbitMQSetting
    {
        public bool AutomaticRecoveryEnabled { get; set; }

        public bool Persistent { get; set; }

        public string ConnectionString { get; set; }

        public string TopicPrefix { get; set; }

        public string QueuePrefix { get; set; }

        public List<IRabbitMQConnectionLoadBalancing> LoadBalancings { get; set; }

        public bool UseEasyNetQ { get; set; }

        public RabbitMQSetting()
        {
            AutomaticRecoveryEnabled = true;
            Persistent = true;
            TopicPrefix = string.Empty;
            QueuePrefix = string.Empty;
            LoadBalancings = new List<IRabbitMQConnectionLoadBalancing>();
        }
    }
}
