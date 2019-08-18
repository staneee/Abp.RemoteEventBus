namespace Abp.RemoteEventBus
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRemoteEventHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        void HandleEvent(RemoteEventArgs eventArgs);
    }
}
