using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Interface
{
    public interface IRemoteEventHandler<T>
    {
        void HandleEvent(string topic, T data);
    }
}
