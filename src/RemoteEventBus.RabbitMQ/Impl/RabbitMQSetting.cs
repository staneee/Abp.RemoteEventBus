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

        public string Prefix { get; set; }

        public List<IRabbitMQConnectionLoadBalancing> LoadBalancings { get; set; }

        public bool UseEasyNetQ { get; set; }

        public RabbitMQSetting()
        {
            AutomaticRecoveryEnabled = true;
            Persistent = true;
            Prefix = string.Empty;
            LoadBalancings = new List<IRabbitMQConnectionLoadBalancing>();
        }
    }
}
