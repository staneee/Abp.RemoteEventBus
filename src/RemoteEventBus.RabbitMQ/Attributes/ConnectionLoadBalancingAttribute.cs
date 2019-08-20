using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Attributes
{
    /// <summary>
    /// 连接负载均衡特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ConnectionLoadBalancingAttribute : Attribute
    {

    }
}
