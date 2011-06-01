using System;
using System.Abstract;
using Contoso.Abstract;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new LazyEx<IServiceCache>(() => new StaticServiceCache(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
            var actions = ServiceCacheManager.GetSetupDescriptor(service);
            //actions.Do(x => Console.Write("Here"));
            //
            var cache = service.Value;
        }
    }
}
