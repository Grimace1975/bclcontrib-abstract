using Rhino.ServiceBus.Impl;
using System.Abstract;
namespace Contoso.Abstract.RhinoServiceBus
{
    public static class Extensions
    {
        public static AbstractRhinoServiceBusConfiguration UseAbstractServiceLocator(this AbstractRhinoServiceBusConfiguration configuration) { return UseAbstractServiceLocator(configuration, ServiceLocatorManager.Current); }
        public static AbstractRhinoServiceBusConfiguration UseAbstractServiceLocator(this AbstractRhinoServiceBusConfiguration configuration, IServiceLocator locator)
        {
            new ServiceLocatorBuilder(locator, configuration);
            return configuration;
        }
    }
}