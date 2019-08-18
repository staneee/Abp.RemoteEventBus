namespace Abp.RemoteEventBus
{
    public interface IRemoteEventSerializer
    {
        T Deserialize<T>(byte[] value);

        byte[] Serialize(object value);
    }
}