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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.SharePoint.Common;
using Microsoft.Practices.SharePoint.Common.Configuration;
using Microsoft.Practices.SharePoint.Common.Logging;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
namespace Contoso.Abstract.SPG2010
{
    /// <summary>
    /// SharePointServiceLocator
    /// </summary>
    public class SharePointServiceLocator
    {
        private const int defaultSiteCacheIntervalInSeconds = 60;

        private static IServiceLocator _serviceLocatorInstance;
        private static Dictionary<Guid, SiteLocatorEntry> _siteLocators = new Dictionary<Guid, SiteLocatorEntry>();
        private static object _syncRoot = new object();
        static SharePointServiceLocator _sharePointLocatorInstance = null;
        static IServiceLocatorConfig _farmServiceLocatorConfig;

        private class SiteLocatorEntry
        {
            public List<TypeMapping> SiteMappings { get; set; }
            public DateTime LoadTime { get; set; }
            public IServiceLocator locator;
        }

        static int _siteCacheInterval = -1;

        /// <summary>
        /// Gets or sets the site caching timeout in seconds.
        /// </summary>
        /// <value>
        /// The site caching timeout in seconds.
        /// </value>
        public static int SiteCachingTimeoutInSeconds
        {
            get
            {
                if (_siteCacheInterval == -1)
                    _siteCacheInterval = SharePointLocator.GetSiteCacheInterval();
                return _siteCacheInterval;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Resources.ArgumentMustBeGreaterThanZero", "SiteCachingTimeoutInSeconds", value));
                _siteCacheInterval = value;
            }
        }

        private IServiceLocatorConfig FarmLocatorConfig
        {
            get
            {
                if (_farmServiceLocatorConfig == null)
                    _farmServiceLocatorConfig = this.GetServiceLocatorConfig();
                return _farmServiceLocatorConfig;
            }
        }


        private static SharePointServiceLocator SharePointLocator
        {
            get
            {
                if (_sharePointLocatorInstance == null)
                    lock (_syncRoot)
                        if (_sharePointLocatorInstance == null)
                            _sharePointLocatorInstance = new SharePointServiceLocator();
                return _sharePointLocatorInstance;
            }
        }


        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <returns></returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public static IServiceLocator GetCurrent() { return SharePointLocator.DoGetCurrent(); }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public static IServiceLocator GetCurrent(SPSite site) { return SharePointLocator.DoGetCurrent(site); }

        /// <summary>
        /// Gets the current farm.
        /// </summary>
        /// <returns></returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public static IServiceLocator GetCurrentFarm() { return SharePointLocator.GetCurrentFarmLocator(); }

        /// <summary>
        /// Does the get current.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        protected IServiceLocator DoGetCurrent(SPSite site)
        {
            Validation.ArgumentNotNull(site, "site");
            IServiceLocator locator = null;
            if (_siteLocators.ContainsKey(site.ID))
            {
                var entry = _siteLocators[site.ID];
                if (DateTime.Now.Subtract(entry.LoadTime).TotalSeconds < SiteCachingTimeoutInSeconds)
                    locator = _siteLocators[site.ID].locator;
                else
                    locator = RefreshServiceLocatorInstance(site);
            }
            else
                locator = CreateServiceLocatorInstance(site);
            return locator;
        }

        /// <summary>
        /// Does the get current.
        /// </summary>
        /// <returns></returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        protected IServiceLocator DoGetCurrent()
        {
            return (SPContext.Current != null ? DoGetCurrent(SPContext.Current.Site) : GetCurrentFarmLocator());
        }

        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        private IServiceLocator GetCurrentFarmLocator()
        {
            if (_serviceLocatorInstance == null)
                lock (_syncRoot)
                    if (_serviceLocatorInstance == null)
                    {
                        // The SharePoint service locator has to have access to SPFarm.Local, because it uses the ServiceLocatorConfig  to store it's configuration settings. 
                        if (!SharePointEnvironment.CanAccessSharePoint)
                            throw new NoSharePointContextException("Properties.Resources.InvalidRunContext");
                        EnsureCommonServiceLocatorCurrentFails();
                        _serviceLocatorInstance = CreateServiceLocatorInstance();
                        if (_serviceLocatorInstance.GetType() == typeof(ActivatingServiceLocator))
                        {
                            var activatingLocator = (ActivatingServiceLocator)_serviceLocatorInstance;
                            activatingLocator.MappingRegisteredEvent += SharePointLocator.OnFarmTypeMappingChanged;
                        }
                    }
            return _serviceLocatorInstance;
        }

        private void EnsureCommonServiceLocatorCurrentFails() { ServiceLocator.SetLocatorProvider(throwNotSupportedException); }

        private IServiceLocator throwNotSupportedException() { throw new NotSupportedException("Properties.Resources.ServiceLocatorNotSupported"); }

        private IServiceLocator CreateServiceLocatorInstance()
        {
            var configuredTypeMappings = FarmLocatorConfig.GetTypeMappings();
            // Create the factory that can configure and create the service locator It's possible that the factory to be used has been changed in config. 
            var serviceLocatorFactory = GetServiceLocatorFactory(configuredTypeMappings);
            // Create the service locator and load it up with the default and configured type mappings
            var serviceLocator = serviceLocatorFactory.Create();
            serviceLocatorFactory.LoadTypeMappings(serviceLocator, GetDefaultTypeMappings());
            serviceLocatorFactory.LoadTypeMappings(serviceLocator, configuredTypeMappings);
            return serviceLocator;
        }

        /// <summary>
        /// Gets the service locator config.
        /// </summary>
        /// <returns></returns>
        protected virtual IServiceLocatorConfig GetServiceLocatorConfig()
        {
            return new ServiceLocatorConfig();
        }

        /// <summary>
        /// Gets the service locator config.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        protected virtual IServiceLocatorConfig GetServiceLocatorConfig(SPSite site)
        {
            var config = new ServiceLocatorConfig();
            config.Site = site;
            return config;
        }

        private IServiceLocator CreateServiceLocatorInstance(SPSite site)
        {
            var configuredTypeMappings = this.FarmLocatorConfig.GetTypeMappings();
            var factory = GetServiceLocatorFactory(configuredTypeMappings);
            var entry = new SiteLocatorEntry();
            entry.LoadTime = DateTime.Now;
            var siteserviceLocatorConfig = GetServiceLocatorConfig(site);
            entry.SiteMappings = siteserviceLocatorConfig.GetTypeMappings();
            entry.locator = factory.Create();
            lock (_syncRoot)
            {
                var farmLocator = this.GetCurrentFarmLocator();
                if (farmLocator.GetType() == typeof(ActivatingServiceLocator))
                {
                    // call order is important, the mappings at the site will override the mappings at the service.
                    factory.LoadTypeMappings(entry.locator, ((ActivatingServiceLocator)farmLocator).GetTypeMappings());
                    factory.LoadTypeMappings(entry.locator, entry.SiteMappings);
                }
                else
                    // since registering runtime registeration is a feature of activating service locator, we will assume that if it is not an activating locator, it is not supporting runtime registration.
                    SetupCustomLocator(factory, entry.locator, entry.SiteMappings);
                _siteLocators[site.ID] = entry;
            }

            return entry.locator;
        }

        private void SetupCustomLocator(IServiceLocatorFactory factory, IServiceLocator locator, IEnumerable<TypeMapping> siteMappings)
        {
            factory.LoadTypeMappings(locator, GetDefaultTypeMappings());
            factory.LoadTypeMappings(locator, FarmLocatorConfig.GetTypeMappings());
            factory.LoadTypeMappings(locator, siteMappings);
        }

        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        private IServiceLocator RefreshServiceLocatorInstance(SPSite site)
        {
            var siteserviceLocatorConfig = GetServiceLocatorConfig(site);
            var entry = _siteLocators[site.ID];
            // only update if changed since last time we loaded...
            if (siteserviceLocatorConfig.LastUpdate > entry.LoadTime)
            {
                if (entry.locator.GetType() == typeof(ActivatingServiceLocator))
                {
                    // get any mappings added programmatically.  Assume this behavior is unique to  Activating service locator, so ignore if not activating.
                    var activatingLocator = (ActivatingServiceLocator)entry.locator;
                    activatingLocator.Refresh(GetDefaultTypeMappings(), FarmLocatorConfig.GetTypeMappings(), entry.SiteMappings, siteserviceLocatorConfig.GetTypeMappings());
                }
                else
                {
                    var factory = GetServiceLocatorFactory(this.FarmLocatorConfig.GetTypeMappings());
                    SetupCustomLocator(factory, entry.locator, siteserviceLocatorConfig.GetTypeMappings());
                }
                entry.SiteMappings = siteserviceLocatorConfig.GetTypeMappings();
            }
            entry.LoadTime = DateTime.Now;
            return entry.locator;
        }

        private void OnFarmTypeMappingChanged(object sender, TypeMappingChangedArgs args)
        {
            lock (_syncRoot)
                foreach (SiteLocatorEntry entry in _siteLocators.Values)
                {
                    //only override if not defined at the site level.
                    if (entry.SiteMappings == null || !entry.SiteMappings.Exists((t) => t.Key == args.Mapping.Key && t.FromAssembly == args.Mapping.FromAssembly && t.FromType == args.Mapping.FromType))
                    {
                        var activatingLocator = entry.locator as ActivatingServiceLocator;
                        if (activatingLocator != null)
                            activatingLocator.RegisterTypeMapping(args.Mapping);
                    }
                }
        }

        private static IServiceLocatorFactory GetServiceLocatorFactory(IEnumerable<TypeMapping> configuredTypeMappings)
        {
            // Find configured factory. If it's there, creat it. 
            var factory = FindAndCreateConfiguredType<IServiceLocatorFactory>(configuredTypeMappings);
            // If there is no configured factory, then the ActivatingServiceLocatorFactory is the default one to use
            if (factory == null)
                factory = new ActivatingServiceLocatorFactory();
            return factory;
        }

        private static readonly List<TypeMapping> defaultMappings = new List<TypeMapping>()
                                                               {
                                                                    TypeMapping.Create<ILogger, SharePointLogger>(),
                                                                    TypeMapping.Create<ITraceLogger, TraceLogger>(),
                                                                    TypeMapping.Create<IEventLogLogger, EventLogLogger>(),
                                                                    TypeMapping.Create<IHierarchicalConfig, HierarchicalConfig>(),
                                                                    TypeMapping.Create<IConfigManager, ConfigManager>(),
                                                                    TypeMapping.Create<IServiceLocatorConfig, ServiceLocatorConfig>(),
                                                                    TypeMapping.Create<IApplicationContextProvider, ApplicationContextProvider>(),
                                                               };

        private static IEnumerable<TypeMapping> GetDefaultTypeMappings()
        {
            return defaultMappings;
        }

        private static TService FindAndCreateConfiguredType<TService>(IEnumerable<TypeMapping> configuredTypeMappings)
            where TService : class
        {
            var mapping = FindMappingForType<TService>(configuredTypeMappings);
            if (mapping == null)
                return null;
            return (TService)ActivatingServiceLocator.CreateInstanceFromTypeMapping(mapping, null);
        }

        private static TypeMapping FindMappingForType<TService>(IEnumerable<TypeMapping> configuredTypeMappings)
        {
            if (configuredTypeMappings == null)
                return null;
            foreach (TypeMapping configuredMapping in configuredTypeMappings)
                if (configuredMapping.FromType == typeof(TService).AssemblyQualifiedName)
                    return configuredMapping;
            return null;
        }

        /// <summary>
        /// Gets the site cache interval.
        /// </summary>
        /// <returns></returns>
        public int GetSiteCacheInterval()
        {
            var config = GetServiceLocatorConfig();
            var cacheInterval = config.GetSiteCacheInterval();
            if (cacheInterval == -1)
                cacheInterval = defaultSiteCacheIntervalInSeconds;
            return cacheInterval;
        }

        /// <summary>
        /// Replaces the current service locator.
        /// </summary>
        /// <param name="newServiceLocator">The new service locator.</param>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public static void ReplaceCurrentServiceLocator(IServiceLocator newServiceLocator)
        {
            SharePointLocator.EnsureCommonServiceLocatorCurrentFails();
            _serviceLocatorInstance = newServiceLocator;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public static void Reset()
        {
            ReplaceCurrentServiceLocator(null);
            _farmServiceLocatorConfig = null;
            _siteLocators.Clear();
        }
    }
}
