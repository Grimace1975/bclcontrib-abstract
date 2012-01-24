using System;
using System.Abstract;
using Contoso.Abstract;
namespace Example
{
    class Program
    {
        public class Message : IServiceMessage
        {
            public string Name { get; set; }
        }

        static void Main(string[] args)
        {
            ServiceLocatorManager.SetProvider(() => new UnityServiceLocator());
            ServiceBusManager.SetProvider(() => new RhinoServiceBusAbstractor());

            var serviceBus = ServiceBusManager.Current;
            serviceBus.Send<Message>(x =>
            {
                x.Name = "George";
            });
        }
    }
}
