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
    /// ServiceLocatorExtensions
    /// </summary>
    public static class ServiceLocatorExtensions
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

        #region BehaveAs

        public static IServiceLocator BehaveAs(this IServiceLocator locator, string @namespace)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (@namespace == null)
                throw new ArgumentNullException("@namespace");
            return new ServiceLocatorNamespaceWrapper(locator, @namespace);
        }
        public static IServiceRegistrar BehaveAs(this IServiceRegistrar registrar, ServiceRegistrarLifetime lifetime)
        {
            if (registrar == null)
                throw new ArgumentNullException("registrar");
            return new ServiceRegistrarLifetimeBehavorWrapper(registrar, lifetime);
        }

        #endregion

        #region Lazy Setup

        public static Lazy<IServiceLocator> Register(this Lazy<IServiceLocator> service, Action<IServiceRegistrar> registrant) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => registrant(l.Registrar)); return service; }
        //
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, null); return service; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, string name) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, name); return service; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, Lazy<IServiceLocator> locator) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, Lazy<IServiceLocator> locator, string name) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, null, assembiles)); return service; }
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, null, assemblies)); return service; }//
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service, Predicate<Type> predicate) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, predicate, assembiles)); return service; }
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, predicate, assemblies)); return service; }
        //
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, null, assembiles)); return service; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, null, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, Predicate<Type> predicate) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, predicate, assembiles)); return service; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, predicate, assemblies)); return service; }
        //
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), null, assembiles)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), null, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, Predicate<Type> predicate) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), predicate, assembiles)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), predicate, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, null, assembiles)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, null, assemblies)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, Predicate<Type> predicate) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, predicate, assembiles)); return service; }
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, predicate, assemblies)); return service; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Assembly GetPreviousCallingMethodAssembly()
        {
            var method = new StackTrace().GetFrame(2).GetMethod();
            return (method != null ? method.ReflectedType.Assembly : null);
        }

        #endregion

        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar) { RegisterByIServiceRegistration(registrar, x => DefaultPredicate(registrar, x), new[] { GetPreviousCallingMethodAssembly() }); }
        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar, params Assembly[] assemblies) { RegisterByIServiceRegistration(registrar, x => DefaultPredicate(registrar, x), assemblies); }
        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar, Predicate<Type> predicate) { RegisterByIServiceRegistration(registrar, predicate, new[] { GetPreviousCallingMethodAssembly() }); }
        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Count() == 0)
                return;
            var locator = registrar.Locator;
            var registrationType = typeof(IServiceRegistrant);
            var matchedTypes = assemblies.SelectMany(a => a.AsConcreteTypes(registrationType, predicate))
                .Where(t => !ServiceLocatorManager.GetWantsToSkipRegistration(t));
            foreach (var matchedType in matchedTypes)
                locator.Resolve<IServiceRegistrant>(matchedType).Register(registrar);
        }

        public static void RegisterByNamingConvention(this IServiceRegistrar registrar) { RegisterByNamingConvention((serviceType, implementationType) => registrar.Register(serviceType, implementationType), x => DefaultPredicate(registrar, x), new[] { GetPreviousCallingMethodAssembly() }); }
        public static void RegisterByNamingConvention(this IServiceRegistrar registrar, params Assembly[] assemblies) { RegisterByNamingConvention((serviceType, implementationType) => registrar.Register(serviceType, implementationType), x => DefaultPredicate(registrar, x), assemblies); }
        public static void RegisterByNamingConvention(this IServiceRegistrar registrar, Predicate<Type> predicate) { RegisterByNamingConvention((serviceType, implementationType) => registrar.Register(serviceType, implementationType), predicate, new[] { GetPreviousCallingMethodAssembly() }); }
        public static void RegisterByNamingConvention(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByNamingConvention((serviceType, implementationType) => registrar.Register(serviceType, implementationType), predicate, assemblies); }
        public static void RegisterByNamingConvention(Action<Type, Type> action, Predicate<Type> predicate, IEnumerable<Assembly> assemblies)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (assemblies == null || assemblies.Count() == 0)
                return;
            var interfaceTypes = assemblies.SelectMany(a => a.AsTypes(t => t.IsInterface && t.Name.StartsWith("I") && (predicate == null || predicate(t))));
            foreach (var interfaceType in interfaceTypes)
            {
                var concreteName = interfaceType.Name.Substring(1);
                var matchedTypes = interfaceType.Assembly.AsConcreteTypes(interfaceType, predicate)
                    .Where(t => t.Name == concreteName && !ServiceLocatorManager.GetWantsToSkipRegistration(t))
                    .ToList();
                if (matchedTypes.Count == 1)
                    action(interfaceType, matchedTypes.First());
            }
        }

        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), typeof(TBasedOn), x => DefaultPredicate(registrar, x), new[] { GetPreviousCallingMethodAssembly() }); }
        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar, params Assembly[] assemblies) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), typeof(TBasedOn), x => DefaultPredicate(registrar, x), assemblies); }
        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar, Predicate<Type> predicate) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), typeof(TBasedOn), predicate, new[] { GetPreviousCallingMethodAssembly() }); }
        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), typeof(TBasedOn), predicate, assemblies); }
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), basedOnType, x => DefaultPredicate(registrar, x), new[] { GetPreviousCallingMethodAssembly() }); }
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType, params Assembly[] assemblies) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), basedOnType, x => DefaultPredicate(registrar, x), assemblies); }
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType, Predicate<Type> predicate) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), basedOnType, predicate, new[] { GetPreviousCallingMethodAssembly() }); }
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), basedOnType, predicate, assemblies); }
        public static void RegisterByTypeMatch(Action<Type, Type, string> action, Type basedOnType, Predicate<Type> predicate, IEnumerable<Assembly> assemblies)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (basedOnType == null)
                throw new ArgumentNullException("basedOnType");
            if (assemblies == null || assemblies.Count() == 0)
                return;
            var matchedTypes = assemblies.SelectMany(a => a.AsConcreteTypes(basedOnType, predicate));
            foreach (var matchedType in matchedTypes)
                action(basedOnType, matchedType, Guid.NewGuid().ToString());
        }

        public static readonly Func<IServiceRegistrar, Type, bool> DefaultPredicate = (r, t) => !r.HasRegistered(t);
    }
}