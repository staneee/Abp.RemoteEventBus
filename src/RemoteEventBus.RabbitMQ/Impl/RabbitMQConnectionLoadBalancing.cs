using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteEventBus.Impl
{
    /// <summary>
    /// 负载均衡配置器
    /// </summary>
    public class RabbitMQConnectionLoadBalancing : IRabbitMQConnectionLoadBalancing
    {
        /// <summary>
        /// 当前负载均衡的索引
        /// </summary>
        protected int _currentIndex;
        /// <summary>
        /// 所有的负载均衡配置
        /// </summary>
        protected virtual ServerConfig[] ServerConfigs { get; set; }

        private string _handlerType;
        /// <summary>
        /// 负载均衡的事件处理器类型
        /// </summary>
        public virtual string HandlerType
        {
            get => this._handlerType;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("The parameter handlerType cannot be empty!");
                }
                _handlerType = value;
            }
        }
        /// <summary>
        /// 最大数量
        /// </summary>
        public virtual int MaxSize { get; set; }
        /// <summary>
        /// 是否已初始化成功
        /// </summary>
        public bool Initialized { get; protected set; }

        public RabbitMQConnectionLoadBalancing(string handlerType)
        {
            HandlerType = handlerType;
        }

        public string NextKey()
        {
            if (Initialized == false)
            {
                this.Initialize();
            }

            this._currentIndex = this.NextIndex();
            return ServerConfigs[this._currentIndex].Name;
        }

        public int NextIndex()
        {
            if (Initialized == false)
            {
                this.Initialize();
            }

            return NextServerIndex(ServerConfigs);
        }


        public void Initialize()
        {
            ServerConfigs = new ServerConfig[this.MaxSize];
            for (int i = 0; i < this.MaxSize; i++)
            {
                ServerConfigs[i] = new ServerConfig()
                {
                    Weight = 1,
                    Name = $"{HandlerType}_{i}"
                };
            }
            Initialized = true;
        }

        /// <summary>
        /// 获取所有负载均衡的(topic/队列)名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetAll()
        {
            return ServerConfigs.Select(o => o.Name).ToList();
        }

        /// <summary>
        /// 下一个负载的索引
        /// </summary>
        /// <param name="serverConfigArray"></param>
        /// <returns></returns>
        public static int NextServerIndex(ServerConfig[] serverConfigArray)
        {
            int index = -1;
            int total = 0;
            int size = serverConfigArray.Count();
            for (int i = 0; i < size; i++)
            {
                serverConfigArray[i].Current += serverConfigArray[i].Weight;
                total += serverConfigArray[i].Weight;
                if (index == -1 || serverConfigArray[index].Current < serverConfigArray[i].Current)
                {
                    index = i;
                }
            }
            serverConfigArray[index].Current -= total;
            return index;
        }


    }


    /// <summary>
    /// 负载的配置
    /// </summary>
    public struct ServerConfig
    {
        //初始权重
        public int Weight { get; set; }

        //当前权重
        public int Current { get; set; }

        //服务名称
        public string Name { get; set; }
    }

}
