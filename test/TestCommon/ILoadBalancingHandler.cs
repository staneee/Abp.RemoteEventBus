using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RemoteEventBus.Attributes;
using RemoteEventBus.Interface;

namespace TestCommon
{
    [ConnectionLoadBalancing]
    public interface ILoadBalancingHandler : IRemoteEventHandler<MyEntity>
    {
    }
}
