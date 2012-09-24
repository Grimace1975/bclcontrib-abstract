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
using System.Abstract.Parts;
namespace System.Abstract
{
    /// <summary>
    /// ServiceLocatorExtensions
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <typeparam name="TServiceLocator">The type of the service locator.</typeparam>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static TServiceLocator GetServiceLocator<TServiceLocator>(this IServiceLocator locator)
            where TServiceLocator : class, IServiceLocator
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            return (locator as TServiceLocator);
        }

        /// <summary>
        /// Resolves the specified locator.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="locator">The locator.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public static TService Resolve<TService>(this IServiceLocator locator, Type serviceType)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            return (TService)locator.Resolve(serviceType);
        }

        /// <summary>
        /// Resolves the specified locator.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="locator">The locator.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static TService Resolve<TService>(this IServiceLocator locator, Type serviceType, string name)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            return (TService)locator.Resolve(serviceType, name);
        }

        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="locator">The locator.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public static IEnumerable<TService> ResolveAll<TService>(this IServiceLocator locator, Type serviceType)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            return locator.ResolveAll(serviceType).Cast<TService>();
        }

        #region BehaveAs

        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static T BehaveAs<T>(this IServiceLocator service)
            where T : class, IServiceLocator
        {
            IServiceWrapper<IServiceLocator> serviceWrapper;
            do
            {
                serviceWrapper = (service as IServiceWrapper<IServiceLocator>);
                if (serviceWrapper != null)
                    service = serviceWrapper.Parent;
            } while (serviceWrapper != null);
            return (service as T);
        }

        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="namespace">The @namespace.</param>
        /// <returns></returns>
        public static IServiceLocator BehaveAs(this IServiceLocator locator, string @namespace)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (@namespace == null)
                throw new ArgumentNullException("@namespace");
            return new ServiceLocatorNamespaceBehaviorWrapper(locator, @namespace);
        }
        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// <returns></returns>
        public static IServiceRegistrar BehaveAs(this IServiceRegistrar registrar, ServiceRegistrarLifetime lifetime)
        {
            if (registrar == null)
                throw new ArgumentNullException("registrar");
            var registrarAsCloneable = (registrar as ICloneable);
            if (registrarAsCloneable == null)
                throw new ArgumentNullException("registrar", "Provider must have ICloneable");
            var newRegistrar = (IServiceRegistrar)registrarAsCloneable.Clone();
            var newRegistrarAsAccessor = (newRegistrar as IServiceRegistrarBehaviorAccessor);
            if (newRegistrarAsAccessor == null)
                throw new ArgumentNullException("registrar", "Provider must have IServiceRegistrarBehaviorAccessor");
            newRegistrarAsAccessor.Lifetime = lifetime;
            return (IServiceRegistrar)newRegistrar;
        }

        #endregion

        #region Lazy Setup

        /// <summary>
        /// Registers the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="registrant">The registrant.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> Register(this Lazy<IServiceLocator> service, Action<IServiceRegistrar> registrant) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => registrant(l.Registrar)); return service; }

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator<T>(this Lazy<IServiceLocator> service)
            where T : class, IServiceLocator { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator<T>(this Lazy<IServiceLocator> service, string name)
            where T : class, IServiceLocator { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator<T>(this Lazy<IServiceLocator> service, Lazy<IServiceLocator> locator)
            where T : class, IServiceLocator { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator<T>(this Lazy<IServiceLocator> service, Lazy<IServiceLocator> locator, string name)
            where T : class, IServiceLocator { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, string name) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, Lazy<IServiceLocator> locator) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, Lazy<IServiceLocator> locator, string name) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator<T>(this Lazy<IServiceLocator> service, IServiceLocator locator)
            where T : class, IServiceLocator { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator<T>(this Lazy<IServiceLocator> service, IServiceLocator locator, string name)
            where T : class, IServiceLocator { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, IServiceLocator locator) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> service, IServiceLocator locator, string name) { ServiceLocatorManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        /// <summary>
        /// Registers the by I service registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, null, assembiles)); return service; }
        /// <summary>
        /// Registers the by I service registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, null, assemblies)); return service; }
        /// <summary>
        /// Registers the by I service registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service, Predicate<Type> predicate) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, predicate, assembiles)); return service; }
        /// <summary>
        /// Registers the by I service registration.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByIServiceRegistration(l.Registrar, predicate, assemblies)); return service; }
        //
        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, null, assembiles)); return service; }
        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, null, assemblies)); return service; }
        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, Predicate<Type> predicate) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, predicate, assembiles)); return service; }
        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByNamingConvention(l.Registrar, predicate, assemblies)); return service; }
        //
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), null, assembiles)); return service; }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), null, assemblies)); return service; }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, Predicate<Type> predicate) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), predicate, assembiles)); return service; }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByTypeMatch<TBasedOn>(this Lazy<IServiceLocator> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, typeof(TBasedOn), predicate, assemblies)); return service; }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, null, assembiles)); return service; }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, null, assemblies)); return service; }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, Predicate<Type> predicate) { var assembiles = new[] { GetPreviousCallingMethodAssembly() }; ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, predicate, assembiles)); return service; }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> RegisterByTypeMatch(this Lazy<IServiceLocator> service, Type basedOnType, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(service).Do(l => RegisterByTypeMatch(l.Registrar, basedOnType, predicate, assemblies)); return service; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Assembly GetPreviousCallingMethodAssembly()
        {
            var log = ServiceLogManager.Current;
            var thisAssembly = typeof(ServiceLocatorExtensions).Assembly;
            var stackTrace = new StackTrace();
            for (var i = 2; i < 10 && i < stackTrace.FrameCount; i++)
            {
                Assembly assembly;
                var method = stackTrace.GetFrame(i).GetMethod();
                if (method != null)
                    log.InformationFormat("{0}: {1} at {2}.", i, method.ToString(), method.ReflectedType.Assembly.FullName);
                if (method != null && (assembly = method.ReflectedType.Assembly) != thisAssembly)
                    return assembly;
            }
            throw new InvalidOperationException("Unable to find an assembly");
        }

        #endregion

        /// <summary>
        /// Registers the by I service registration.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar) { RegisterByIServiceRegistration(registrar, x => DefaultPredicate(registrar, x), new[] { GetPreviousCallingMethodAssembly() }); }
        /// <summary>
        /// Registers the by I service registration.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar, params Assembly[] assemblies) { RegisterByIServiceRegistration(registrar, x => DefaultPredicate(registrar, x), assemblies); }
        /// <summary>
        /// Registers the by I service registration.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="predicate">The predicate.</param>
        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar, Predicate<Type> predicate) { RegisterByIServiceRegistration(registrar, predicate, new[] { GetPreviousCallingMethodAssembly() }); }
        /// <summary>
        /// Registers the by I service registration.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByIServiceRegistration(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            if (registrar == null)
                throw new ArgumentNullException("registrar");
            var debugger = ServiceLocatorManager.Debugger;
            if (debugger == null || (debugger.Flags & ServiceLocatorManagerDebugger.DebuggerFlags.ByIServiceRegistration) != ServiceLocatorManagerDebugger.DebuggerFlags.ByIServiceRegistration)
            {
                if (assemblies == null || assemblies.Count() == 0)
                    return;
                var locator = registrar.Locator;
                var registrationType = typeof(IServiceRegistrant);
                var matchedTypes = assemblies.SelectMany(a => a.AsConcreteTypes(registrationType, predicate))
                    .Where(t => !ServiceLocatorManager.HasIgnoreServiceLocator(t));
                foreach (var matchedType in matchedTypes)
                    locator.Resolve<IServiceRegistrant>(matchedType).Register(registrar);
            }
            else
            {
                var log = debugger.Log;
                log.Information("RegisterByIServiceRegistration");
                if (assemblies == null || assemblies.Count() == 0)
                {
                    log.Information("Done. No assemblies requested.");
                    return;
                }
                var locator = registrar.Locator;
                var registrationType = typeof(IServiceRegistrant);
                var matchedTypes = assemblies.SelectMany(a =>
                    {
                        log.InformationFormat("- Scanning assembly {0}:", a.FullName);
                        return a.AsConcreteTypes(registrationType, predicate);
                    })
                    .Where(t =>
                    {
                        if (!ServiceLocatorManager.HasIgnoreServiceLocator(t))
                            return true;
                        log.InformationFormat("- {0} HasIgnoreServiceLocator and has been skipped.", t.FullName);
                        return false;
                    });
                foreach (var matchedType in matchedTypes)
                {
                    log.InformationFormat("- {0} Matching Type.", matchedType.FullName);
                    locator.Resolve<IServiceRegistrant>(matchedType).Register(registrar);
                }
                log.Information("Done.");
            }
        }

        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        public static void RegisterByNamingConvention(this IServiceRegistrar registrar) { RegisterByNamingConvention((serviceType, implementationType) => registrar.Register(serviceType, implementationType), x => DefaultPredicate(registrar, x), new[] { GetPreviousCallingMethodAssembly() }); }
        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByNamingConvention(this IServiceRegistrar registrar, params Assembly[] assemblies) { RegisterByNamingConvention((serviceType, implementationType) => registrar.Register(serviceType, implementationType), x => DefaultPredicate(registrar, x), assemblies); }
        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="predicate">The predicate.</param>
        public static void RegisterByNamingConvention(this IServiceRegistrar registrar, Predicate<Type> predicate) { RegisterByNamingConvention((serviceType, implementationType) => registrar.Register(serviceType, implementationType), predicate, new[] { GetPreviousCallingMethodAssembly() }); }
        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByNamingConvention(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByNamingConvention((serviceType, implementationType) => registrar.Register(serviceType, implementationType), predicate, assemblies); }
        /// <summary>
        /// Registers the by naming convention.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByNamingConvention(Action<Type, Type> action, Predicate<Type> predicate, IEnumerable<Assembly> assemblies)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            var debugger = ServiceLocatorManager.Debugger;
            if (debugger == null || (debugger.Flags & ServiceLocatorManagerDebugger.DebuggerFlags.ByNamingConvention) != ServiceLocatorManagerDebugger.DebuggerFlags.ByNamingConvention)
            {
                if (assemblies == null || assemblies.Count() == 0)
                    return;
                var interfaceTypes = assemblies.SelectMany(a => a.AsTypes(t => t.IsInterface && t.Name.StartsWith("I") && (predicate == null || predicate(t))));
                foreach (var interfaceType in interfaceTypes)
                {
                    var concreteName = interfaceType.Name.Substring(1);
                    var matchedTypes = interfaceType.Assembly.AsConcreteTypes(interfaceType, predicate)
                        .Where(t => t.Name == concreteName && !ServiceLocatorManager.HasIgnoreServiceLocator(t))
                        .ToList();
                    if (matchedTypes.Count == 1)
                        action(interfaceType, matchedTypes.First());
                }
            }
            else
            {
                var log = debugger.Log;
                log.Information("RegisterByNamingConvention");
                if (assemblies == null || assemblies.Count() == 0)
                {
                    log.Information("Done. No assemblies requested.");
                    return;
                }
                var interfaceTypes = assemblies.SelectMany(a =>
                {
                    log.InformationFormat("- Scanning assembly {0}:", a.FullName);
                    return a.AsTypes(t => t.IsInterface && t.Name.StartsWith("I") && (predicate == null || predicate(t)));
                });
                foreach (var interfaceType in interfaceTypes)
                {
                    var concreteName = interfaceType.Name.Substring(1);
                    var matchedTypes = interfaceType.Assembly.AsConcreteTypes(interfaceType, predicate)
                        .Where(t =>
                        {
                            if (t.Name == concreteName)
                            {
                                if (!ServiceLocatorManager.HasIgnoreServiceLocator(t))
                                    return true;
                                log.InformationFormat("- {0} HasIgnoreServiceLocator and has been skipped.", t.FullName);
                                return false;
                            }
                            return true;
                        })
                        .ToList();
                    if (matchedTypes.Count == 1)
                    {
                        log.InformationFormat("- {0} matches {1}.", interfaceType.FullName, matchedTypes.First());
                        action(interfaceType, matchedTypes.First());
                    }
                    else
                        log.InformationFormat("- {0} Matched {1} types and has been skipped.", interfaceType.FullName, matchedTypes.Count);
                }
                log.Information("Done.");
            }
        }

        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="registrar">The registrar.</param>
        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), typeof(TBasedOn), x => DefaultPredicate(registrar, x), new[] { GetPreviousCallingMethodAssembly() }); }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar, params Assembly[] assemblies) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), typeof(TBasedOn), x => DefaultPredicate(registrar, x), assemblies); }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <param name="predicate">The predicate.</param>
        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar, Predicate<Type> predicate) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), typeof(TBasedOn), predicate, new[] { GetPreviousCallingMethodAssembly() }); }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <typeparam name="TBasedOn">The type of the based on.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByTypeMatch<TBasedOn>(this IServiceRegistrar registrar, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), typeof(TBasedOn), predicate, assemblies); }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), basedOnType, x => DefaultPredicate(registrar, x), new[] { GetPreviousCallingMethodAssembly() }); }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType, params Assembly[] assemblies) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), basedOnType, x => DefaultPredicate(registrar, x), assemblies); }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <param name="predicate">The predicate.</param>
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType, Predicate<Type> predicate) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), basedOnType, predicate, new[] { GetPreviousCallingMethodAssembly() }); }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByTypeMatch(this IServiceRegistrar registrar, Type basedOnType, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByTypeMatch((serviceType, implementationType, name) => registrar.Register(serviceType, implementationType, name), basedOnType, predicate, assemblies); }
        /// <summary>
        /// Registers the by type match.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="basedOnType">Type of the based on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void RegisterByTypeMatch(Action<Type, Type, string> action, Type basedOnType, Predicate<Type> predicate, IEnumerable<Assembly> assemblies)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (basedOnType == null)
                throw new ArgumentNullException("basedOnType");
            var debugger = ServiceLocatorManager.Debugger;
            if (debugger == null || (debugger.Flags & ServiceLocatorManagerDebugger.DebuggerFlags.ByTypeMatch) != ServiceLocatorManagerDebugger.DebuggerFlags.ByTypeMatch)
            {
                if (assemblies == null || assemblies.Count() == 0)
                    return;
                var matchedTypes = assemblies.SelectMany(a => a.AsConcreteTypes(basedOnType, predicate));
                foreach (var matchedType in matchedTypes)
                    action(basedOnType, matchedType, Guid.NewGuid().ToString());
            }
            else
            {
                var log = debugger.Log;
                log.Information("RegisterByTypeMatch");
                if (assemblies == null || assemblies.Count() == 0)
                {
                    log.Information("Done. No assemblies requested.");
                    return;
                }
                var matchedTypes = assemblies.SelectMany(a =>
                {
                    log.InformationFormat("- Scanning assembly {0}:", a.FullName);
                    return a.AsConcreteTypes(basedOnType, predicate);
                });
                foreach (var matchedType in matchedTypes)
                {
                    log.InformationFormat("- {0} Matched {1}.", basedOnType.FullName, matchedType.FullName);
                    action(basedOnType, matchedType, Guid.NewGuid().ToString());
                }
                log.Information("Done.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly Func<IServiceRegistrar, Type, bool> DefaultPredicate = (r, t) => !r.HasRegistered(t);
    }
}