using System;
using System.Threading.Tasks;
using Abp.Castle.Logging.Log4Net;
using Castle.Facilities.Logging;

namespace Abp.RemoteEventBus.RabbitMQ.TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrapper = AbpBootstrapper.Create<RabbitMQTestModule>();

            bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f =>
                f.UseAbpLog4Net().WithConfig("log4net.config"));

            bootstrapper.Initialize();

            var remoteEventBus = bootstrapper.IocManager.Resolve<IRemoteEventBus>();
            var topicSelector = bootstrapper.IocManager.Resolve<IRemoteEventTopicSelector>();


            //remoteEventBus.Subscribe()

            //topicSelector.SetMapping()

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    const string type = "Type_Test1";
                    //const string topic = "Topic_Test1";
                    var eventDate = new RemoteEventData(type)
                    {
                        Data = { ["playload"] = DateTime.Now }
                    };
                    var topic = topicSelector.SelectTopic(eventDate);
                    //topicSelector.SetMapping()

                    remoteEventBus.Publish(topic, eventDate);

                    Console.ReadKey();
                }
            });

            Console.WriteLine("Any key exit");
            Console.ReadKey();
        }
    }
}