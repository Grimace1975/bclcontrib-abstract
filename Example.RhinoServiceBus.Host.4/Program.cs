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
            var cacheManager = ServiceCacheManager.MakeByProvider(() => new StaticServiceCache());
            //var actions = ServiceCacheManager.GetSetupDescriptor(service);
            //actions.Do(x => Console.WriteLine("Here"));
            //actions.Do(x => Console.WriteLine("Here"));
            //actions.Do(x => Console.WriteLine("Here"));
            var cache = cacheManager.Value;

            //var castleWinsorService = new Lazy<IServiceBus>(() => new CastleWindsorServiceLocator());
            //var locator = castleWinsorService.Value;
            //var registrar = locator.Registrar;
            //registrar.Register<object, object>();

            var serviceLocator = ServiceLocatorManager.Current;
            //var serviceLocatorMgr = ServiceLocatorManager.MakeByProvider(() => new UnityServiceLocator());
            new RhinoServiceBusConfiguration()
                .UseAbstractServiceLocator(serviceLocator)
                .Configure();
                
            serviceLocator.Resolve<IStartableServiceBus>().Start();
            //using (var x = serviceLocator.Resolve<ServiceLocatorBootStrapper>())
            //{
            ////    x.InitializeContainer();
            ////    System.Threading.Thread.Sleep(5000);
            //}
        }
    }
}
