using System;
using System.Collections.Generic;
using System.Text;
using RemoteEventBus.Interface;

namespace TestCommon
{
    public interface IWorkQueueHandler : IRemoteEventHandler<MyEntity>
    {
    }
}
