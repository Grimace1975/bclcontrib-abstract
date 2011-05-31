using System;
using System.Abstract;
using Contoso.Abstract;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new Lazy<IServiceCache>(() => new StaticServiceCache());
            var actions = ServiceCacheManager.GetSetupDescriptor(service);
            actions.Do(x => Console.Write("Here"));
            //
            var cache = service.Value;

        }
    }
}
