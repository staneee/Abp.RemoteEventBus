using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Attributes
{
    /// <summary>
    /// 负载均衡特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ConnectionLoadBalancingAttribute : Attribute
    {

    }
}
