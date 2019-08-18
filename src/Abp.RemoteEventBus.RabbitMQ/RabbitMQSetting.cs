using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;

namespace Abp.RemoteEventBus.RabbitMQ
{
    public class RabbitMQSetting : IRabbitMQSetting, ITransientDependency
    {
        public virtual string Url { get; set; }

        public virtual bool AutomaticRecoveryEnabled { get; set; }

        public virtual int InitialSize { get; set; }

        public virtual int MaxSize { get; set; }

        public virtual string TopicPrefix { get; set; }

        public virtual string QueuePrefix { get; set; }

        public RabbitMQSetting()
        {
            AutomaticRecoveryEnabled = true;
            InitialSize = 0;
            MaxSize = 10;
            TopicPrefix = string.Empty;
            QueuePrefix = string.Empty;
        }
    }
}
