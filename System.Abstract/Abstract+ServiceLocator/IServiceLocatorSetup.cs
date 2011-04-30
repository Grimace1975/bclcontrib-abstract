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
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
namespace System.Abstract
{
    /// <summary>
    /// IServiceLocatorSetup
    /// </summary>
    public interface IServiceLocatorSetup
    {
        IServiceLocatorSetup Do(Action<IServiceRegistrar, IServiceLocator> action);
        void Finally(IServiceRegistrar registrar, IServiceLocator locator);
    }

    /// <summary>
    /// IServiceLocatorSetupExtensions
    /// </summary>
    public static class IServiceLocatorSetupExtensions
    {
        public static IServiceLocatorSetup RegisterByIServiceRegistration(this IServiceLocatorSetup setup, params Assembly[] assemblies) { return setup.Do((r, l) => DoRegisterByIServiceRegistration(r, l, null, assemblies)); }
        public static IServiceLocatorSetup RegisterByIServiceRegistration(this IServiceLocatorSetup setup, Predicate<Type> predicate, params Assembly[] assemblies) { return setup.Do((r, l) => DoRegisterByIServiceRegistration(r, l, predicate, assemblies)); }
        public static IServiceLocatorSetup RegisterByNamingConvention(this IServiceLocatorSetup setup) { return setup.Do((r, l) => DoRegisterFromAssembliesByNameConvention(r, l, null, new[] { IServiceLocatorSetupExtensions.GetPreviousCallingMethodsAssembly() })); }
        public static IServiceLocatorSetup RegisterByNamingConvention(this IServiceLocatorSetup setup, params Assembly[] assemblies) { return setup.Do((r, l) => DoRegisterFromAssembliesByNameConvention(r, l, null, assemblies)); }
        public static IServiceLocatorSetup RegisterByNamingConvention(this IServiceLocatorSetup setup, Predicate<Type> predicate) { return setup.Do((r, l) => DoRegisterFromAssembliesByNameConvention(r, l, predicate, new[] { IServiceLocatorSetupExtensions.GetPreviousCallingMethodsAssembly() })); }
        public static IServiceLocatorSetup RegisterFromAssembliesByNameConvention(this IServiceLocatorSetup setup, Predicate<Type> predicate, params Assembly[] assemblies) { return setup.Do((r, l) => DoRegisterFromAssembliesByNameConvention(r, l, predicate, assemblies)); }

        internal static Assembly GetPreviousCallingMethodsAssembly()
        {
            var frame = new StackTrace().GetFrame(2);
            var method = frame.GetMethod();
            return (method != null ? method.ReflectedType.Assembly : null);
        }

        public static void DoRegisterByIServiceRegistration(IServiceRegistrar registrar, IServiceLocator locator, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            var registrationType = typeof(IServiceRegistration);
            var matchedTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => (!t.IsInterface) && (!t.IsAbstract) && (t.GetInterfaces().Contains(registrationType)))
                .Where(t => (predicate == null) || (predicate(t)));
            foreach (var matchedType in matchedTypes)
                locator.Resolve<IServiceRegistration>(matchedType).Register(registrar);
        }

        public static void DoRegisterFromAssembliesByNameConvention(IServiceRegistrar registrar, IServiceLocator locator, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            var registrationType = typeof(IServiceRegistration);
            var matchedTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => (!t.IsInterface) && (!t.IsAbstract) && (t.GetInterfaces().Contains(registrationType)))
                .ToList();
            foreach (var matchedType in matchedTypes)
                locator.Resolve<IServiceRegistration>(matchedType).Register(registrar);
            // default registation
            var remainingAssemblies = assemblies.Where(a => !matchedTypes.Any(y => y.Assembly == a));
            ApplyDefaultNamingConvention(remainingAssemblies, predicate, (interfaceType, type) => registrar.Register(interfaceType, type));
        }

        public static void ApplyDefaultNamingConvention(IEnumerable<Assembly> assemblies, Predicate<Type> predicate, Action<Type, Type> action)
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
                    .ToList();
                if (matchedTypes.Count == 1)
                    action(interfaceType, matchedTypes.First());
            }
        }
    }
}