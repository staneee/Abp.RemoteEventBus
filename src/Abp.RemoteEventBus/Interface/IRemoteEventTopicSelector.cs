using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus
{
    public interface IRemoteEventTopicSelector
    {
        /// <summary>
        /// 选择Topic
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        string SelectTopic(IRemoteEventData eventData);

        /// <summary>
        /// 设置映射类型和Topic的映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topic"></param>
        void SetMapping<T>(string topic)
            where T : IRemoteEventData;
    }
}
