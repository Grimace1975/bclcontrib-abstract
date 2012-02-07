using System;
using System.Abstract;
using Contoso.Abstract;
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
        }
    }
}
