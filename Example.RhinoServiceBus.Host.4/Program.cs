using System;
using System.Abstract;
using Contoso.Abstract;
using Contoso.Abstract.RhinoServiceBus;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Impl;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceLocatorManager.SetProvider(() => new UnityServiceLocator());
                //.RegisterByNamingConvention();
            //var service = new Lazy<IServiceCache>(() => new StaticServiceCache());
            //var actions = ServiceCacheManager.GetSetupDescriptor(service);
            //actions.Do(x => Console.WriteLine("Here"));
            //actions.Do(x => Console.WriteLine("Here"));
            //actions.Do(x => Console.WriteLine("Here"));
            //var cache = service.Value;

            //var castleWinsorService = new Lazy<IServiceBus>(() => new CastleWindsorServiceLocator());
            //var locator = castleWinsorService.Value;
            //var registrar = locator.Registrar;
            //registrar.Register<object, object>();

            var serviceLocator = ServiceLocatorManager.Current;
            new RhinoServiceBusConfiguration()
                .UseAbstractServiceLocator(serviceLocator)
                .Configure();
                
            serviceLocator.Resolve<IStartableServiceBus>().Start();
        }
    }
}
