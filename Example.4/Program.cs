using System;
using System.Abstract;
using Contoso.Abstract;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //ServiceLocatorManager.SetProvider(() => new UnityServiceLocator());
            //var service = new Lazy<IServiceCache>(() => new StaticServiceCache());
            //var actions = ServiceCacheManager.GetSetupDescriptor(service);
            //actions.Do(x => Console.WriteLine("Here"));
            //actions.Do(x => Console.WriteLine("Here"));
            //actions.Do(x => Console.WriteLine("Here"));
            //var cache = service.Value;

            var castleWinsorService = new Lazy<IServiceLocator>(() => new CastleWindsorServiceLocator());
            var locator = castleWinsorService.Value;
            var r = locator.Registrar;
            //var registrar = locator.Registrar;
            //registrar.Register<object, object>();

            //new RhinoServiceBusConfiguration()
            //    .UseUnity(serviceLocator.Container)
            //    .Configure();
            //serviceLocator.Resolve<IStartableServiceBus>().Start();
        }
    }
}
