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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace System.Abstract
{
    /// <summary>
    /// IServiceLocator
    /// </summary>
    public interface IServiceLocator : IServiceProvider
    {
        // registrar
        IServiceRegistrar Registrar { get; }
        TServiceRegistrar GetRegistrar<TServiceRegistrar>()
            where TServiceRegistrar : class, IServiceRegistrar;

        // resolve
        TService Resolve<TService>()
            where TService : class;
        TService Resolve<TService>(string name)
            where TService : class;
        object Resolve(Type serviceType);
        object Resolve(Type serviceType, string name);
        //
        IEnumerable<TService> ResolveAll<TService>()
            where TService : class;
        IEnumerable<object> ResolveAll(Type serviceType);

        // inject
        TService Inject<TService>(TService instance)
            where TService : class;

        // release and teardown
        void Release(object instance);
        void TearDown<TService>(TService instance)
            where TService : class;
    }

    /// <summary>
    /// IServiceLocatorExtensions
    /// </summary>
    public static class IServiceLocatorExtensions
    {
        public static TServiceLocator GetServiceLocator<TServiceLocator>(this IServiceLocator locator)
            where TServiceLocator : class, IServiceLocator
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            return (locator as TServiceLocator);
        }

        public static TService Resolve<TService>(this IServiceLocator locator, Type serviceType)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            return (TService)locator.Resolve(serviceType);
        }

        public static TService Resolve<TService>(this IServiceLocator locator, Type serviceType, string name)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            return (TService)locator.Resolve(serviceType, name);
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IServiceLocator locator, Type serviceType)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            return locator.ResolveAll(serviceType).Cast<TService>();
        }

        public static IServiceLocator Wrap(this IServiceLocator locator, string @namespace)
        {
            if (@namespace == null)
                throw new ArgumentNullException("@namespace");
            return new ServiceLocatorNamespaceWrapper(locator, @namespace);
        }

        #region Lazy Setup

        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, null); return service; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, string name) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, name); return service; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, Lazy<IServiceLocator> locator) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, Lazy<IServiceLocator> locator, string name) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, null, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, predicate, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, null, new[] { GetPreviousCallingMethodAssembly() })); return service; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, null, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, Predicate<Type> predicate) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, predicate, new[] { GetPreviousCallingMethodAssembly() })); return service; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, predicate, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), null, new[] { GetPreviousCallingMethodAssembly() })); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), null, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, Predicate<Type> predicate) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), predicate, new[] { GetPreviousCallingMethodAssembly() })); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), predicate, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, null, new[] { GetPreviousCallingMethodAssembly() })); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, null, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, Predicate<Type> predicate) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, predicate, new[] { GetPreviousCallingMethodAssembly() })); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, predicate, assemblies)); return service; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Assembly GetPreviousCallingMethodAssembly()
        {
            var method = new StackTrace().GetFrame(2).GetMethod();
            return (method != null ? method.ReflectedType.Assembly : null);
        }

        #endregion

        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            var locator = registrar.Locator;
            var registrationType = typeof(IServiceRegistrant);
            var matchedTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => !t.IsInterface && !t.IsAbstract && t.GetInterfaces().Contains(registrationType) && (predicate == null || predicate(t)))
                .Where(t => !ServiceLocatorManager.GetWantsToSkipRegistration(t));
            foreach (var matchedType in matchedTypes)
                locator.Resolve<IServiceRegistrant>(matchedType).Register(registrar);
        }

        public static void RegisterByNamingConvention(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByNamingConvention(predicate, assemblies, (serviceType, implementationType) => registrar.Register(serviceType, implementationType)); }
        public static void RegisterByNamingConvention(Predicate<Type> predicate, IEnumerable<Assembly> assemblies, Action<Type, Type> action)
        {
            if (assemblies.Count() == 0)
                return;
            var interfaceTypes = assemblies.SelectMany(a => a.AsTypesEnumerator(t => t.IsInterface))
                .Where(t => t.Name.StartsWith("I") && (predicate == null || predicate(t)));
            foreach (var interfaceType in interfaceTypes)
            {
                var concreteName = interfaceType.Name.Substring(1);
                var matchedTypes = interfaceType.Assembly.AsTypesEnumerator(interfaceType)
                    .Where(t => t.Name == concreteName && (predicate == null || predicate(t)))
                    .Where(t => !ServiceLocatorManager.GetWantsToSkipRegistration(t))
                    .ToList();
                if (matchedTypes.Count == 1)
                    action(interfaceType, matchedTypes.First());
            }
        }

        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByTypeMatch(typeof(TBasedOn), predicate, assemblies, (serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name)); }
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByTypeMatch(basedOnType, predicate, assemblies, (serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name)); }
        public static void RegisterByTypeMatch(Type basedOnType, Predicate<Type> predicate, IEnumerable<Assembly> assemblies, Action<Type, Type, string> action)
        {
            if (assemblies.Count() == 0)
                return;
            var types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => basedOnType.IsAssignableFrom(t) && !t.Equals(basedOnType) && !t.IsInterface && !t.IsAbstract && (predicate == null || predicate(t)));
            foreach (var type in types)
                action(basedOnType, type, Guid.NewGuid().ToString());
        }
    }
}