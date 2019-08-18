using Abp.Dependency;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Abp.RemoteEventBus.Impl
{
    public class JsonRemoteEventSerializer : IRemoteEventSerializer, ISingletonDependency
    {
        private readonly JsonSerializerSettings settings;

        public JsonRemoteEventSerializer()
        {
            settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public T Deserialize<T>(byte[] value)
        {
            var valueString = Encoding.UTF8.GetString(value);

            return JsonConvert.DeserializeObject<T>(valueString, settings);
        }

        public byte[] Serialize(object value)
        {
            var resultString = JsonConvert.SerializeObject(value, settings);
            return Encoding.UTF8.GetBytes(resultString);
        }
    }
}