using Rhino.ServiceBus.Impl;
using System.Abstract;
namespace Contoso.Abstract
{
    internal static class ServiceBusConfigurationExtensions
    {
        public static AbstractRhinoServiceBusConfiguration UseServiceLocator(this AbstractRhinoServiceBusConfiguration configuration) { return UseServiceLocator(configuration, ServiceLocatorManager.Current); }
        public static AbstractRhinoServiceBusConfiguration UseServiceLocator(this AbstractRhinoServiceBusConfiguration configuration, IServiceLocator locator)
        {
            new ServiceBusBuilder(locator, configuration);
            return configuration;
        }
    }
}