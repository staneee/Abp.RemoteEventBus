using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TopicAttribute : Attribute
    {

    }
}
