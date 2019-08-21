using RemoteEventBus.Attributes;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    [Topic]
    public class TopicHandler : IRemoteEventHandler<MyEntity>
    {
        private readonly string _name;

        public TopicHandler(string name)
        {
            this._name = name;
        }

        public void HandleEvent(MyEntity data)
        {

        }
    }
}
