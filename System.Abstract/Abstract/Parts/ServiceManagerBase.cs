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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
namespace System.Abstract.Parts
{
    /// <summary>
    /// ServiceManagerBase
    /// </summary>
    public abstract partial class ServiceManagerBase<TIService, TServiceSetupAction>
        where TIService : class
    {
        private static readonly ConditionalWeakTable<Lazy<TIService>, ISetupDescriptor> _setupDescriptors = new ConditionalWeakTable<Lazy<TIService>, ISetupDescriptor>();
        private static readonly object _lock = new object();

        public static Lazy<TIService> Lazy { get; private set; }
        public static Lazy<TIService> SetProvider(Func<TIService> provider) { return Lazy = new Lazy<TIService>(provider); }
        public static Lazy<TIService> SetProvider(Func<TIService> provider, ISetupDescriptor setup) { return Lazy = new Lazy<TIService>(provider); }
        protected static SetupRegistration Registration { get; set; }

        // Force "precise" initialization
        static ServiceManagerBase() { }

        public static TIService Current
        {
            get { return Lazy.Value; }
        }

        protected class SetupRegistration
        {
            public Action<TIService, IEnumerable<TServiceSetupAction>> OnSetup { get; set; }
            public Func<Func<IServiceLocator>, string, TServiceSetupAction> ServiceLocatorRegistrar { get; set; }
        }

        protected static void RegisterInstance<T>(IServiceLocator locator, T service, string name)
            where T : class
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            var registrar = locator.GetRegistrar();
            if (name == null)
                registrar.RegisterInstance<T>(service);
            else
                registrar.RegisterInstance<T>(service, name);
        }

        #region IServiceSetup

        public static void ApplySetup(TIService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            var registration = Registration;
            if (registration == null)
                throw new NullReferenceException("Registration");
            var onSetup = registration.OnSetup;
            if (onSetup == null)
                return;
            // preform setup
            ISetupDescriptor setupDescriptor;
            var list = (CompilerServicesExtensions.TryGetValueByLazyValue<TIService, ISetupDescriptor>(_setupDescriptors, service, out setupDescriptor) && (setupDescriptor == null) ? setupDescriptor.ToList() : null);
            onSetup(service, list);
        }

        public static ISetupDescriptor GetSetupDescriptor(Lazy<TIService> lazy)
        {
            if (lazy == null)
                throw new ArgumentNullException("lazy");
            ISetupDescriptor setupDescriptor;
            if (_setupDescriptors.TryGetValue(lazy, out setupDescriptor))
                return setupDescriptor;
            lock (_lock)
                if (!_setupDescriptors.TryGetValue(lazy, out setupDescriptor))
                {
                    setupDescriptor = new SetupDescriptor(Registration);
                    _setupDescriptors.Add(lazy, setupDescriptor);
                }
            return setupDescriptor;
        }

        /// <summary>
        /// ISetupActionCollection
        /// </summary>
        public interface ISetupDescriptor
        {
            void Do(TServiceSetupAction action);
            void RegisterWithServiceLocator(string name);
            void RegisterWithServiceLocator(Func<IServiceLocator> locator, string name);
            IEnumerable<TServiceSetupAction> ToList();
        }

        /// <summary>
        /// SetupActionCollection
        /// </summary>
        protected class SetupDescriptor : ISetupDescriptor
        {
            private List<TServiceSetupAction> _actions = new List<TServiceSetupAction>();
            private SetupRegistration _registration;

            public SetupDescriptor(SetupRegistration registration) { _registration = registration; }

            void ISetupDescriptor.Do(TServiceSetupAction action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                _actions.Add(action);
            }

            void ISetupDescriptor.RegisterWithServiceLocator(string name) { ((ISetupDescriptor)this).RegisterWithServiceLocator(ServiceLocatorManager.GetDefaultServiceLocator, name); }
            void ISetupDescriptor.RegisterWithServiceLocator(Func<IServiceLocator> locator, string name)
            {
                if (locator == null)
                    throw new ArgumentNullException("locator");
                var serviceLocatorRegistrar = _registration.ServiceLocatorRegistrar;
                if (serviceLocatorRegistrar == null)
                    throw new NullReferenceException("registration.ServiceLocatorRegistrar");
                _actions.Add(serviceLocatorRegistrar(locator, name));
            }

            IEnumerable<TServiceSetupAction> ISetupDescriptor.ToList() { return _actions; }
        }

        #endregion
    }
}
