using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Interface
{
    public interface IRabbitMQConnectionLoadBalancing
    {
        /// <summary>
        /// 处理器的类型
        /// </summary>
        Type HandlerType { get; set; }

        /// <summary>
        /// 处理器负载最大数量
        /// </summary>
        int MaxSize { get; set; }

        /// <summary>
        /// 获取负载均衡值
        /// </summary>
        /// <returns></returns>
        string Start();

        /// <summary>
        /// 初始化负载均衡配置
        /// </summary>
        void Initialize();

        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <returns></returns>
        List<string> GetAll();
    }
}
