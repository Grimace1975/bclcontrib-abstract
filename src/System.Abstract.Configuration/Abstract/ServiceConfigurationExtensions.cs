#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System.Abstract.Configuration;
using System.Abstract.Configuration.ServiceBus;
namespace System.Abstract
{
    /// <summary>
    /// ServiceConfigurationExtensions
    /// </summary>
    public static class ServiceConfigurationExtensions
    {
        #region Lazy Setup

        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Lazy<IEventSource> LoadFromConfiguration(this Lazy<IEventSource> service, EventSourceConfiguration configuration) { EventSourceManager.GetSetupDescriptor(service).LoadFromConfiguration(service, configuration); return service; }
        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> LoadFromConfiguration(this Lazy<IServiceBus> service, ServiceBusConfiguration configuration) { ServiceBusManager.GetSetupDescriptor(service).LoadFromConfiguration(service, configuration); return service; }
        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> LoadFromConfiguration(this Lazy<IServiceCache> service, ServiceCacheConfiguration configuration) { ServiceCacheManager.GetSetupDescriptor(service).LoadFromConfiguration(service, configuration); return service; }
        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> LoadFromConfiguration(this Lazy<IServiceLocator> service, ServiceLocatorConfiguration configuration) { ServiceLocatorManager.GetSetupDescriptor(service).LoadFromConfiguration(service, configuration); return service; }
        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> LoadFromConfiguration(this Lazy<IServiceLog> service, ServiceLogConfiguration configuration) { ServiceLogManager.GetSetupDescriptor(service).LoadFromConfiguration(service, configuration); return service; }

        #endregion

        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        public static void LoadFromConfiguration(this EventSourceManager.ISetupDescriptor descriptor, Lazy<IEventSource> service, EventSourceConfiguration configuration)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (configuration == null)
                throw new ArgumentNullException("configuration");
        }

        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        public static void LoadFromConfiguration(this ServiceBusManager.ISetupDescriptor descriptor, Lazy<IServiceBus> service, ServiceBusConfiguration configuration)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            var endpoints = configuration.Endpoints;
            foreach (EndpointElement endpoint in endpoints)
                service.AddEndpoint(endpoint.Endpoint);
        }

        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        public static void LoadFromConfiguration(this ServiceCacheManager.ISetupDescriptor descriptor, Lazy<IServiceCache> service, ServiceCacheConfiguration configuration)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (configuration == null)
                throw new ArgumentNullException("configuration");
        }

        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        public static void LoadFromConfiguration(this ServiceLocatorManager.ISetupDescriptor descriptor, Lazy<IServiceLocator> service, ServiceLocatorConfiguration configuration)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (configuration == null)
                throw new ArgumentNullException("configuration");
        }

        /// <summary>
        /// Loads from configuration.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="service">The service.</param>
        /// <param name="configuration">The configuration.</param>
        public static void LoadFromConfiguration(this ServiceLogManager.ISetupDescriptor descriptor, Lazy<IServiceLog> service, ServiceLogConfiguration configuration)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (configuration == null)
                throw new ArgumentNullException("configuration");
        }
    }
}
