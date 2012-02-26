using System.Abstract;
using NServiceBus;
using NServiceBus.ObjectBuilder.Common.Config;
namespace Contoso.Abstract.NServiceBus
{
    public static class Extensions
    {
        public static Configure AbstractServiceBuilder(this Configure configuration) { return AbstractServiceBuilder(configuration, ServiceLocatorManager.Current); }
        public static Configure AbstractServiceBuilder(this Configure configuration, IServiceLocator locator)
        {
            ConfigureCommon.With(configuration, new ServiceLocatorObjectBuilder(locator));
            return configuration;
        }
    }
}