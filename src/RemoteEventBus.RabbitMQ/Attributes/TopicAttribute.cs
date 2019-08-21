using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Attributes
{
    /// <summary>
    /// 主题队列特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class TopicAttribute : Attribute
    {

    }
}
