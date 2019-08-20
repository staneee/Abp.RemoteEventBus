using Abp.Dependency;
using Castle.Core.Logging;

namespace Abp.RemoteEventBus.RabbitMQ.Test
{
    [RemoteEventHandler(ForType = "Type_Test1", ForTopic = "Topic_Test1")]
    public class TestEventHandler : IRemoteEventHandler, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public TestEventHandler()
        {
            Logger = NullLogger.Instance;
        }

        public void HandleEvent(RemoteEventArgs eventArgs)
        {
            Logger.Info("receive " + eventArgs.EventData.Data["playload"]);
        }
    }
}