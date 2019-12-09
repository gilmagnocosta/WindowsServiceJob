using Autofac;
using Autofac.Extras.Quartz;
using log4net.Config;
using Topshelf;
using System.Collections.Specialized;
using System.Configuration;
using WindowsServiceJob.Jobs;
using WindowsServiceJob.Log;

namespace WindowsServiceJob
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<Service>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterModule(new LoggingModule());

            containerBuilder.RegisterModule(new QuartzAutofacFactoryModule
            {
                ConfigurationProvider = context =>
                    (NameValueCollection) ConfigurationManager.GetSection("quartz")
            });

            containerBuilder.RegisterModule(new QuartzAutofacJobsModule(typeof(MyJob).Assembly));

            IContainer container = containerBuilder.Build();

            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.SetServiceName(typeof(MyJob).Assembly.GetName().Name);
                hostConfigurator.SetDisplayName("Service Name");
                hostConfigurator.SetDescription("Service description");

                hostConfigurator.RunAsLocalSystem();
                hostConfigurator.UseLog4Net();

                hostConfigurator.Service<Service>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(() => container.Resolve<Service>());

                    serviceConfigurator.WhenStarted(service => service.OnStart());
                    serviceConfigurator.WhenStopped(service => service.OnStop());
                });
            });
        }
    }
}
 