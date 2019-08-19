using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.RemoteEventBus.RabbitMQ.Test
{
    [DependsOn(typeof(AbpRemoteEventBusRabbitMQModule))]
    public class RabbitMQTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RabbitMQTestModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            Configuration.Modules.RemoteEventBus().UseRabbitMQ().Configure(setting =>
            {
                // TODO: 链接字符串
                //setting.Url = "amqp://root:ms877350@10.3.2.154:15672/";
                setting.Url = "amqp://root:ms877350@10.3.2.154:5672/";
            });

            Configuration.Modules.RemoteEventBus().AutoSubscribe();
        }
    }
}