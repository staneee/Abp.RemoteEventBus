using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Interface
{
    /// <summary>
    /// RabbitMQ 处理器负载均衡配置
    /// </summary>
    public interface IRabbitMQConnectionLoadBalancing
    {
        /// <summary>
        /// 负载均衡主键
        /// </summary>
        string PrimaryKey { get; set; }

        /// <summary>
        /// 处理器负载最大数量
        /// </summary>
        int MaxSize { get; set; }

        /// <summary>
        /// 获取负载均衡键值
        /// </summary>
        /// <returns></returns>
        string NextKey();

        /// <summary>
        /// 获取负载均衡索引
        /// </summary>
        /// <returns></returns>
        int NextIndex();

        /// <summary>
        /// 初始化负载均衡配置
        /// </summary>
        void Initialize();

        /// <summary>
        /// 获取所有负载均衡值
        /// </summary>
        /// <returns></returns>
        List<string> GetAll();
    }
}
