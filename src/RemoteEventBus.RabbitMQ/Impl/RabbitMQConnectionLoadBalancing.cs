using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteEventBus.Impl
{
    public class RabbitMQConnectionLoadBalancing : IRabbitMQConnectionLoadBalancing
    {
        protected Type _handlerType;

        public virtual Type HandlerType
        {
            get => _handlerType;
            set
            {
                _handlerType = value;
            }
        }
        protected int _maxSize;
        public virtual int MaxSize
        {
            get => _maxSize;
            set
            {
                _maxSize = value;
            }
        }
        /// <summary>
        /// 当前负载均衡的索引
        /// </summary>
        protected int _currentIndex;

        /// <summary>
        /// 所有的负载均衡配置
        /// </summary>
        protected virtual ServerConfig[] ServerConfigs { get; set; }


        /// <summary>
        /// 是否已初始化成功
        /// </summary>
        public bool Initialized
        { get; protected set; }


        public string Start()
        {
            if (Initialized == false)
            {
                this.Initialize();
            }

            this._currentIndex = NextServerIndex(ServerConfigs);
            return ServerConfigs[this._currentIndex].Name;
        }

        public void Initialize()
        {
            ServerConfigs = new ServerConfig[this.MaxSize];
            for (int i = 0; i < this.MaxSize; i++)
            {
                ServerConfigs[i] = new ServerConfig()
                {
                    Weight = 1,
                    Name = $"{HandlerType.FullName}_{i}"
                };
            }
            Initialized = true;
        }

        public List<string> GetAll()
        {
            return ServerConfigs.Select(o => o.Name).ToList();
        }

        public struct ServerConfig
        {
            //初始权重
            public int Weight { get; set; }

            //当前权重
            public int Current { get; set; }

            //服务名称
            public string Name { get; set; }
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
}
