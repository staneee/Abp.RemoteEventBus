using RemoteEventBus.Attributes;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    [Topic]
    public interface ITopicHandler : IRemoteEventHandler<MyEntity>
    {
    }
}
