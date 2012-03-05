namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //var serviceManager = ServiceCacheManager.MakeByProvider(() => new StaticServiceCache())
            //    .LoadFromConfiguration(null);
            //var actions = ServiceCacheManager.GetSetupDescriptor(serviceManager);
            //actions.Do(x => Console.Write("Here"));
            ////
            //var cache = serviceManager.Value;

            //var nparams = Nparams.Parse(new { controller = "sample" });
            //Console.Write(nparams.ContainsKey("controller"));


            //ServiceLocatorManager.SetProvider(() => new UnityServiceLocator());
            //var service = new Lazy<IServiceCache>(() => new StaticServiceCache());
            //var actions = ServiceCacheManager.GetSetupDescriptor(service);
            //actions.Do(x => Console.WriteLine("Here"));
            //actions.Do(x => Console.WriteLine("Here"));
            //actions.Do(x => Console.WriteLine("Here"));
            //var cache = service.Value;

            //var castleWinsorService = new Lazy<IServiceLocator>(() => new CastleWindsorServiceLocator());
            //var locator = castleWinsorService.Value;
            //var r = locator.Registrar;
            //var registrar = locator.Registrar;
            //registrar.Register<object, object>();

            //new RhinoServiceBusConfiguration()
            //    .UseUnity(serviceLocator.Container)
            //    .Configure();
            //serviceLocator.Resolve<IStartableServiceBus>().Start();
        }
    }
}
