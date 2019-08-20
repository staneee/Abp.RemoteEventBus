using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Interface
{
    public interface IRemoteEventSerializer
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] value);

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        byte[] Serialize(object value);
    }
}
