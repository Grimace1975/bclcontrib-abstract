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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.SharePoint.Common;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;

namespace Contoso.Abstract.SPG2010
{
    public class ActivatingServiceLocator : ServiceLocatorImplBase
    {
        private readonly object _syncRoot = new object();
        private Dictionary<string, Dictionary<string, TypeMapping>> _typeMappingsDictionary = new Dictionary<string, Dictionary<string, TypeMapping>>();
        private Dictionary<string, Dictionary<string, object>> _singletonsDictionary = new Dictionary<string, Dictionary<string, object>>();

        public event EventHandler<TypeMappingChangedArgs> MappingRegisteredEvent;

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override object DoGetInstance(Type serviceType, string key)
        {
            Validation.ArgumentNotNull(serviceType, "serviceType");
            var nonNullKey = PreventNull(key);
            if (!_typeMappingsDictionary.ContainsKey(serviceType.AssemblyQualifiedName))
                throw BuildNotRegisteredException(serviceType, key);
            var mappingsForType = _typeMappingsDictionary[serviceType.AssemblyQualifiedName];
            if (!mappingsForType.ContainsKey(nonNullKey))
                throw BuildNotRegisteredException(serviceType, key);
            var typeMapping = mappingsForType[nonNullKey];
            if (typeMapping.InstantiationType == InstantiationType.AsSingleton)
                return GetSingleton(typeMapping);
            return CreateInstanceFromTypeMapping(typeMapping, this);
        }

        private object GetSingleton(TypeMapping typeMapping)
        {
            var singletonValueDictionary = GetSingletonValueDictionary(typeMapping);
            if (!singletonValueDictionary.ContainsKey(typeMapping.GetNonNullKey()))
                lock (_syncRoot)
                    if (!singletonValueDictionary.ContainsKey(typeMapping.GetNonNullKey()))
                        singletonValueDictionary[typeMapping.GetNonNullKey()] = CreateInstanceFromTypeMapping(typeMapping, this);
            return singletonValueDictionary[typeMapping.GetNonNullKey()];
        }

        private Dictionary<string, object> GetSingletonValueDictionary(TypeMapping typeMapping)
        {
            if (!_singletonsDictionary.ContainsKey(typeMapping.FromType))
                lock (_syncRoot)
                    if (!_singletonsDictionary.ContainsKey(typeMapping.FromType))
                        _singletonsDictionary[typeMapping.FromType] = new Dictionary<string, object>();
            return _singletonsDictionary[typeMapping.FromType];
        }

        private static string PreventNull(string value)
        {
            if (value == null)
                return string.Empty;
            return value;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private ActivationException BuildNotRegisteredException(Type serviceType, string key)
        {
            return new ActivationException(string.Format(CultureInfo.CurrentCulture, "Properties.Resources.TypeMappingNotRegistered", serviceType.Name, key));
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static object CreateInstanceFromTypeMapping(TypeMapping typeMapping, IServiceLocator serviceLocator)
        {
            Validation.ArgumentNotNull(typeMapping, "typeMapping");
            Assembly.Load(typeMapping.ToAssembly);
            var typeToCreate = Type.GetType(typeMapping.ToType);
            if (serviceLocator == null)
                return Activator.CreateInstance(typeToCreate);
            var constructors = typeToCreate.GetConstructors();
            ConstructorInfo constructorInformation = null;
            foreach (ConstructorInfo constructor in constructors)
            {
                var foundParameters = constructor.GetParameters();
                if (foundParameters.Length == 0)
                    constructorInformation = constructor;
                if (constructorInformation == null)
                    constructorInformation = constructor;
            }
            if (constructorInformation == null || typeToCreate == typeof(IServiceLocatorConfig))
                return Activator.CreateInstance(typeToCreate);
            var parameters = new List<object>();
            foreach (ParameterInfo parameter in constructorInformation.GetParameters())
                parameters.Add(serviceLocator.GetInstance(parameter.ParameterType, string.Empty));
            return Activator.CreateInstance(typeToCreate, parameters.ToArray());
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (!_typeMappingsDictionary.ContainsKey(serviceType.AssemblyQualifiedName))
                yield break;
            var mappingsForType = _typeMappingsDictionary[serviceType.AssemblyQualifiedName];
            foreach (TypeMapping typeMapping in mappingsForType.Values)
                if (typeMapping.InstantiationType == InstantiationType.AsSingleton)
                    yield return GetSingleton(typeMapping);
                else
                    yield return CreateInstanceFromTypeMapping(typeMapping, this);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Normal service locator method")]
        public ActivatingServiceLocator RegisterTypeMapping<TFrom, TTo>()
            where TTo : TFrom, new() { return RegisterTypeMapping(typeof(TFrom), typeof(TTo), null, InstantiationType.NewInstanceForEachRequest); }
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public ActivatingServiceLocator RegisterTypeMapping<TFrom, TTo>(InstantiationType instantiationType)
            where TTo : TFrom, new() { return RegisterTypeMapping(typeof(TFrom), typeof(TTo), null, instantiationType); }
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public ActivatingServiceLocator RegisterTypeMapping<TFrom, TTo>(string key, InstantiationType instantiationType)
            where TTo : TFrom, new() { return RegisterTypeMapping(typeof(TFrom), typeof(TTo), key, instantiationType); }
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public ActivatingServiceLocator RegisterTypeMapping<TFrom, TTo>(string key)
            where TTo : TFrom, new() { return RegisterTypeMapping(typeof(TFrom), typeof(TTo), key, InstantiationType.NewInstanceForEachRequest); }
        public ActivatingServiceLocator RegisterTypeMapping(Type fromType, Type toType) { return RegisterTypeMapping(fromType, toType, null, InstantiationType.NewInstanceForEachRequest); }
        public ActivatingServiceLocator RegisterTypeMapping(Type fromType, Type toType, string key, InstantiationType instantiationType)
        {
            Validation.ArgumentNotNull(fromType, "fromType");
            Validation.ArgumentNotNull(toType, "toType");
            return RegisterTypeMapping(new TypeMapping(fromType, toType, key) { InstantiationType = instantiationType });
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public ActivatingServiceLocator RegisterTypeMapping(TypeMapping mapping)
        {
            TypeMapping.ValidateMapping(mapping);
            if (!_typeMappingsDictionary.ContainsKey(mapping.FromType))
                _typeMappingsDictionary[mapping.FromType] = new Dictionary<string, TypeMapping>();
            var mappingsForType = _typeMappingsDictionary[mapping.FromType];
            mappingsForType[mapping.GetNonNullKey()] = mapping;
            EventHandler<TypeMappingChangedArgs> handler = this.MappingRegisteredEvent;
            if (handler != null)
            {
                var args = new TypeMappingChangedArgs();
                args.Mapping = mapping;
                handler(this, args);
            }
            return this;
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public bool IsTypeRegistered<TService>() { return IsTypeRegistered(typeof(TService)); }
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public bool IsTypeRegistered(Type t)
        {
            return this._typeMappingsDictionary.ContainsKey(t.AssemblyQualifiedName);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public bool IsTypeRegistered<TService>(string key) { return IsTypeRegistered(typeof(TService), key); }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public bool IsTypeRegistered(Type t, string key)
        {
            if (_typeMappingsDictionary.ContainsKey(t.AssemblyQualifiedName))
            {
                var mappingsForType = _typeMappingsDictionary[t.AssemblyQualifiedName];
                if (mappingsForType.ContainsKey(PreventNull(key)))
                    return true;
            }
            return false;
        }

        internal IEnumerable<TypeMapping> GetTypeMappings()
        {
            return _typeMappingsDictionary.Values.SelectMany(tdict => tdict.Values).ToList();
        }

        private static void RemoveMappings(List<TypeMapping> list, IEnumerable<TypeMapping> toRemove)
        {
            // select all of the mappings in teh collection from the list that are in the toRemove list, and remove them from the list.
            foreach (int index in toRemove.Select(mapping => list.FindIndex((t) => t.Equals(mapping))).Where(num => num >= 0))
                list.RemoveAt(index);
        }

        private void RegisterMappings(IEnumerable<TypeMapping> mappings)
        {
            foreach (var mapping in mappings)
                RegisterTypeMapping(mapping);
        }

        internal void Refresh(IEnumerable<TypeMapping> defaultConfig, IEnumerable<TypeMapping> farmConfig, IEnumerable<TypeMapping> siteConfig, IEnumerable<TypeMapping> newSiteMappings)
        {
            Validation.ArgumentNotNull(siteConfig, "siteConfig");
            Validation.ArgumentNotNull(newSiteMappings, "newSiteMappings");
            // Start with all type mappings, and remove configured type mappings.  The remaining type mappings were added programmatically.
            var programmaticMappings = GetTypeMappings() as List<TypeMapping> ?? new List<TypeMapping>();
            // Remove all of the configured type mappings.
            RemoveMappings(programmaticMappings, defaultConfig);
            RemoveMappings(programmaticMappings, farmConfig);
            RemoveMappings(programmaticMappings, siteConfig);
            // Clear the dictionary, and re-add the mappings using the new site type mappings.  Programatically added type mappings take highest precendence.
            _typeMappingsDictionary.Clear();
            //
            RegisterMappings(defaultConfig);
            RegisterMappings(farmConfig);
            RegisterMappings(newSiteMappings);
            RegisterMappings(programmaticMappings);
        }
    }
}
