﻿#region License
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
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Diagnostics.CodeAnalysis;
namespace Contoso.Abstract.SPG2010
{
    public interface IServiceLocatorConfig
    {
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        void RegisterTypeMapping<TFrom, TTo>() where TTo : TFrom;
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        void RegisterTypeMapping<TFrom, TTo>(string key) where TTo : TFrom;
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        void RemoveTypeMappings<T>();
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        void RemoveTypeMapping<T>(string key);
        List<TypeMapping> GetTypeMappings();
        DateTime? LastUpdate { get; }
        SPSite Site { get; set; }
        int GetSiteCacheInterval();
        void SetSiteCacheInterval(int interval);
    }

    public class ServiceLocatorConfig : IServiceLocatorConfig
    {
        private IConfigManager _manager;
        private SPSite _site = null;

        public ServiceLocatorConfig()
        {
            _manager = new ConfigManager();
        }

        public ServiceLocatorConfig(IConfigManager manager)
        {
            Validation.ArgumentNotNull(manager, "manager");
            _manager = manager;
        }

        public SPSite Site
        {
            get { return _site; }
            set
            {
                Validation.ArgumentNotNull(value, "site");
                _site = value;
            }
        }

        private IConfigManager Manager
        {
            get
            {
                if (_manager == null)
                    _manager = new ConfigManager();
                return _manager;
            }
        }

        /// <summary>
        /// Gets the interval to cache a site locator for
        /// </summary>
        /// <returns>the interval value, -1 if not configured</returns>
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public int GetSiteCacheInterval()
        {
            int interval = -1;
            if (SharePointEnvironment.CanAccessFarmConfig)
            {
                var bag = Manager.GetPropertyBag(ConfigLevel.CurrentSPFarm);
                if (bag != null && Manager.ContainsKeyInPropertyBag(GetSiteCacheIntervalConfigKey(), bag))
                    interval = Manager.GetFromPropertyBag<int>(GetSiteCacheIntervalConfigKey(), bag);
            }
            return interval;
        }

        public void SetSiteCacheInterval(int interval)
        {
            if (interval < 0)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Resources.ArgumentMustBeGreaterThanZero", "SiteCachingTimeoutInSeconds", interval));
            var bag = Manager.GetPropertyBag(ConfigLevel.CurrentSPFarm);
            if (bag != null)
                Manager.SetInPropertyBag(GetSiteCacheIntervalConfigKey(), interval, bag);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public void RegisterTypeMapping<TFrom, TTo>()
            where TTo : TFrom { RegisterTypeMapping<TFrom, TTo>(null, InstantiationType.NewInstanceForEachRequest); }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public void RegisterTypeMapping<TFrom, TTo>(string key)
            where TTo : TFrom { RegisterTypeMapping<TFrom, TTo>(key, InstantiationType.NewInstanceForEachRequest); }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual void RegisterTypeMapping<TFrom, TTo>(string key, InstantiationType instantiationType)
            where TTo : TFrom
        {
            var typeMappings = GetConfigData();
            var newTypeMapping = TypeMapping.Create<TFrom, TTo>(key, instantiationType);
            RemovePreviousMappingsForFromType(typeMappings, newTypeMapping);
            typeMappings.Add(newTypeMapping);
            SetTypeMappingsList(typeMappings);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual void RemoveTypeMappings<T>()
        {
            var typeMappings = GetConfigData();
            foreach (TypeMapping mapping in typeMappings.ToArray())
                if (mapping.FromType == typeof(T).AssemblyQualifiedName)
                    typeMappings.Remove(mapping);
            SetTypeMappingsList(typeMappings);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual void RemoveTypeMapping<T>(string key)
        {
            var typeMappings = GetConfigData();
            foreach (TypeMapping mapping in typeMappings.ToArray())
                if (mapping.FromType == typeof(T).AssemblyQualifiedName && mapping.Key == key)
                    typeMappings.Remove(mapping);
            SetTypeMappingsList(typeMappings);
        }

        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public virtual void RemoveTypeMapping(TypeMapping mapping)
        {
            var typeMappings = GetConfigData();
            var index = typeMappings.FindIndex((t) => t.Equals(mapping));
            if (index >= 0)
                typeMappings.RemoveAt(index);
            SetTypeMappingsList(typeMappings);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public List<TypeMapping> GetTypeMappings()
        {
            return GetConfigData();
        }

        private static void RemovePreviousMappingsForFromType(List<TypeMapping> mappings, TypeMapping newTypeMapping)
        {
            foreach (TypeMapping mapping in mappings.ToArray())
                if (mapping.FromType == newTypeMapping.FromType && mapping.Key == newTypeMapping.Key)
                    mappings.Remove(mapping);
        }

        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        private void SetTypeMappingsList(List<TypeMapping> typeMappings)
        {
            // We need to deal with low level serialization to break the dependency between service location and configuration manager, otherwise config mgr and hierarchical config can't be overridden.
            var propertyBag = GetPropertyBag();
            if (propertyBag != null)
            {
                var configData = new ServiceLocationConfigData(typeMappings);
                configData.LastUpdate = DateTime.Now;
                Manager.SetInPropertyBag(GetConfigKey(), configData, propertyBag);
            }
            else
                throw new InvalidOperationException("Resources.ContextMissingSetTypeMappingList");
        }

        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        private ServiceLocationConfigData GetConfigData()
        {
            // We need to deal with low level seralization to break the dependency between service location and  // configuration manager, otherwise config mgr and hierarchical config can't be overridden.
            var propertyBag = GetPropertyBag();
            ServiceLocationConfigData configData = null;
            // In some cases this will be null, like when attempting to load farm service locator config from the sandbox without the proxy installed.
            if (propertyBag != null)
                configData = Manager.GetFromPropertyBag<ServiceLocationConfigData>(GetConfigKey(), propertyBag);
            if (configData == null)
                configData = new ServiceLocationConfigData() { LastUpdate = null };
            return configData;
        }

        [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        protected virtual IPropertyBag GetPropertyBag()
        {
            IPropertyBag bag = null;
            if (_site == null)
            {
                if (SharePointEnvironment.CanAccessFarmConfig)
                    bag = Manager.GetPropertyBag(ConfigLevel.CurrentSPFarm);
            }
            else
            {
                Manager.SetWeb(_site.RootWeb);
                bag = Manager.GetPropertyBag(ConfigLevel.CurrentSPSite);
            }
            return bag;
        }

        public DateTime? LastUpdate
        {
            [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
            [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
            get { return GetConfigData().LastUpdate; }
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        protected virtual string GetConfigKey() { return "Microsoft.Practices.SharePoint.Common.TypeMappings"; }

        protected virtual string GetSiteCacheIntervalConfigKey() { return "Microsoft.Practices.SharePoint.Common.SiteLocatorCacheInterval"; }
    }
}
