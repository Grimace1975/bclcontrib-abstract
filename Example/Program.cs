using System;
using System.Abstract;
using Contoso.Abstract;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceLocatorManager.SetLocatorProvider(() => new UnityServiceLocator())
                .RegisterByIServiceRegistration(typeof(Program).Assembly)
                .RegisterByNamingConvention(typeof(Program).Assembly);
            //
            ServiceBusManager.SetBusProvider(() => new ApplicationServiceBus());
        }
    }
}
