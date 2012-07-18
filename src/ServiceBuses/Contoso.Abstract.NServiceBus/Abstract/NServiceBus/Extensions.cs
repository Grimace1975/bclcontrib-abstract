using System.Abstract;
using NServiceBus;
using NServiceBus.ObjectBuilder.Common.Config;
namespace Contoso.Abstract.NServiceBus
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Abstracts the service builder.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Configure AbstractServiceBuilder(this Configure configuration) { return AbstractServiceBuilder(configuration, ServiceLocatorManager.Current); }
        /// <summary>
        /// Abstracts the service builder.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Configure AbstractServiceBuilder(this Configure configuration, IServiceLocator locator)
        {
            ConfigureCommon.With(configuration, new ServiceLocatorObjectBuilder(locator));
            return configuration;
        }
    }
}