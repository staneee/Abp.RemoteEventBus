using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RemoteEventBus.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteEventBus.Impl
{
    public class JsonRemoteEventSerializer : IRemoteEventSerializer
    {
        private readonly JsonSerializerSettings settings;

        public JsonRemoteEventSerializer()
        {
            settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public virtual T Deserialize<T>(byte[] value)
        {
            var valueString = Encoding.UTF8.GetString(value);

            return JsonConvert.DeserializeObject<T>(valueString, settings);
        }

        public virtual byte[] Serialize(object value)
        {
            var resultString = JsonConvert.SerializeObject(value, settings);
            return Encoding.UTF8.GetBytes(resultString);
        }
    }
}
