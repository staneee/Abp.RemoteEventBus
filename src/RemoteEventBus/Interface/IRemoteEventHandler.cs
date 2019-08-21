using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEventBus.Interface
{
    /// <summary>
    /// 事件处理器接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRemoteEventHandler<T>
    {
        void HandleEvent(T data);
    }
}
