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
        IServiceRegistrar GetRegistrar();
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
        void Reset();
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

        #region Lazy Setup

        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> lazy) { ServiceLocatorManager.GetSetupDescriptor(lazy).RegisterWithServiceLocator(null); return lazy; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> lazy, string name) { ServiceLocatorManager.GetSetupDescriptor(lazy).RegisterWithServiceLocator(name); return lazy; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> lazy, Func<IServiceLocator> locator) { ServiceLocatorManager.GetSetupDescriptor(lazy).RegisterWithServiceLocator(locator, null); return lazy; }
        public static Lazy<IServiceLocator> RegisterWithServiceLocator(this Lazy<IServiceLocator> lazy, Func<IServiceLocator> locator, string name) { ServiceLocatorManager.GetSetupDescriptor(lazy).RegisterWithServiceLocator(locator, name); return lazy; }
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> lazy, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(lazy).Do((r, l) => RegisterByIServiceRegistration(r, l, null, assemblies)); return lazy; }
        public static Lazy<IServiceLocator> RegisterByIServiceRegistration(this Lazy<IServiceLocator> lazy, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(lazy).Do((r, l) => RegisterByIServiceRegistration(r, l, predicate, assemblies)); return lazy; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> lazy) { ServiceLocatorManager.GetSetupDescriptor(lazy).Do((r, l) => RegisterByNamingConvention(r, l, null, new[] { GetPreviousCallingMethodAssembly() })); return lazy; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> lazy, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(lazy).Do((r, l) => RegisterByNamingConvention(r, l, null, assemblies)); return lazy; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> lazy, Predicate<Type> predicate) { ServiceLocatorManager.GetSetupDescriptor(lazy).Do((r, l) => RegisterByNamingConvention(r, l, predicate, new[] { GetPreviousCallingMethodAssembly() })); return lazy; }
        public static Lazy<IServiceLocator> RegisterByNamingConvention(this Lazy<IServiceLocator> lazy, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceLocatorManager.GetSetupDescriptor(lazy).Do((r, l) => RegisterByNamingConvention(r, l, predicate, assemblies)); return lazy; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Assembly GetPreviousCallingMethodAssembly()
        {
            var method = new StackTrace().GetFrame(2).GetMethod();
            return (method != null ? method.ReflectedType.Assembly : null);
        }

        #endregion

        public static void RegisterByIServiceRegistration(IServiceRegistrar registrar, IServiceLocator locator, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            var registrationType = typeof(IServiceRegistration);
            var matchedTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => (!t.IsInterface) && (!t.IsAbstract) && (t.GetInterfaces().Contains(registrationType)))
                .Where(t => (predicate == null) || (predicate(t)))
                .Where(t => !ServiceLocatorManager.GetWantsToSkipRegistration(t));
            foreach (var matchedType in matchedTypes)
                locator.Resolve<IServiceRegistration>(matchedType).Register(registrar);
        }

        public static void RegisterByNamingConvention(IServiceRegistrar registrar, IServiceLocator locator, Predicate<Type> predicate, params Assembly[] assemblies) { RegisterByNamingConvention(assemblies, predicate, (interfaceType, type) => registrar.Register(interfaceType, type)); }
        public static void RegisterByNamingConvention(IEnumerable<Assembly> assemblies, Predicate<Type> predicate, Action<Type, Type> action)
        {
            if (assemblies.Count() == 0)
                return;
            var interfaceTypes = assemblies.SelectMany(a => a.AsTypesEnumerator(t => t.IsInterface))
                .Where(t => t.Name.StartsWith("I"))
                .Where(t => (predicate == null) || (predicate(t)));
            foreach (var interfaceType in interfaceTypes)
            {
                string concreteName = interfaceType.Name.Substring(1);
                var matchedTypes = interfaceType.Assembly.AsTypesEnumerator(interfaceType)
                    .Where(t => t.Name == concreteName)
                    .Where(t => (predicate == null) || (predicate(t)))
                    .Where(t => !ServiceLocatorManager.GetWantsToSkipRegistration(t))
                    .ToList();
                if (matchedTypes.Count == 1)
                    action(interfaceType, matchedTypes.First());
            }
        }
    }
}