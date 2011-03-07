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
using System.Collections.Generic;
using System.Diagnostics;
namespace System.Abstract
{
    /// <summary>
    /// ServiceLocatorGrapher
    /// </summary>
    public class ServiceLocatorGrapher : IServiceLocatorGrapher
    {
        private List<Action<IServiceRegistrar, IServiceLocator>> _actions;
        private List<Type> _serviceRegistrationTypes;
        private List<ConventionMatch> _conventionMatches = new List<ConventionMatch>();

        private struct ConventionMatch
        {
            public ConventionMatch(Type interfaceType, Type type) { InterfaceType = interfaceType; Type = type; }
            public Type InterfaceType;
            public Type Type;
        }

        public IServiceLocatorGrapher Do(Action<IServiceRegistrar, IServiceLocator> action)
        {
            if (_actions != null)
                _actions = new List<Action<IServiceRegistrar, IServiceLocator>>();
            _actions.Add(action);
            return this;
        }

        public IServiceLocatorGrapher RegisterFromAssemblies(Predicate<Type> predicate, params Assembly[] assemblies)
        {
            var registrationType = typeof(IServiceRegistration);
            var matchedTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => (!t.IsInterface) && (!t.IsAbstract) && (t.GetInterfaces().Contains(registrationType)))
                .Where(t => (predicate == null) || (predicate(t)));
            if (_serviceRegistrationTypes != null)
                _serviceRegistrationTypes = new List<Type>();
            _serviceRegistrationTypes.AddRange(matchedTypes);
            return this;
        }

        public IServiceLocatorGrapher RegisterFromAssembliesByNameConvention(Predicate<Type> predicate, params Assembly[] assemblies)
        {
            var registrationType = typeof(IServiceRegistration);
            var matchedTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => (!t.IsInterface) && (!t.IsAbstract) && (t.GetInterfaces().Contains(registrationType)))
                .ToList();
            if (matchedTypes.Count > 0)
            {
                if (_serviceRegistrationTypes != null)
                    _serviceRegistrationTypes = new List<Type>();
                _serviceRegistrationTypes.AddRange(matchedTypes);
            }
            // default registation
            var remainingAssemblies = assemblies.Where(a => !matchedTypes.Any(y => y.Assembly == a));
            ApplyDefaultNamingConvention(remainingAssemblies, predicate, (interfaceType, type) => _conventionMatches.Add(new ConventionMatch(interfaceType, type)));
            return this;
        }

        public void Finally(IServiceRegistrar registrar, IServiceLocator locator)
        {
            if (_actions != null)
                foreach (var action in _actions)
                    action(registrar, locator);
            if (_serviceRegistrationTypes != null)
                foreach (var serviceRegistrationType in _serviceRegistrationTypes)
                    locator.Resolve<IServiceRegistration>(serviceRegistrationType).Register(registrar);
            foreach (var conventionMatch in _conventionMatches)
                registrar.Register(conventionMatch.InterfaceType, conventionMatch.Type);
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
