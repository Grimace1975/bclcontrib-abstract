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
        protected static TIService LazyValue;

        // Force "precise" initialization
        static ServiceManagerBase() { }

        public static Lazy<TIService> Lazy { get; protected set; }

        //public static Lazy<TIService> SetProvider(Func<TIService> provider) { return (Lazy = MakeByProviderProtected(provider, null)); }
        //public static Lazy<TIService> SetProvider(Func<TIService> provider, ISetupDescriptor setupDescriptor) { return (Lazy = MakeByProviderProtected(provider, setupDescriptor)); }
        //public static Lazy<TIService> MakeByProvider(Func<TIService> provider) { return MakeByProviderProtected(provider, null); }
        //public static Lazy<TIService> MakeByProvider(Func<TIService> provider, ISetupDescriptor setupDescriptor) { return MakeByProviderProtected(provider, setupDescriptor); }

        public static Lazy<TIService> MakeByProviderProtected(Func<TIService> provider, ISetupDescriptor setupDescriptor)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            var lazy = new Lazy<TIService>(provider);
            GetSetupDescriptorProtected(lazy, setupDescriptor);
            return lazy;
        }

        protected static SetupRegistration Registration { get; set; }

        #region Setup

        /// <summary>
        /// ISetupRegistration
        /// </summary
        public interface ISetupRegistration
        {
            Action<IServiceLocator, string> OnServiceRegistrar { get; }
        }

        /// <summary>
        /// SetupRegistration
        /// </summary
        protected class SetupRegistration
        {
            public SetupRegistration()
            {
                OnServiceRegistrar = (service, locator, name) =>
                {
                    //var behavior = (service.Registrar as IServiceRegistrarBehaviorAccessor);
                    //if (behavior != null && !behavior.RegisterInLocator)
                    //    throw new InvalidOperationException();
                    RegisterInstance(service, locator, name);
                    // specific registration
                    var setupRegistration = (service as ISetupRegistration);
                    if (setupRegistration != null)
                        setupRegistration.OnServiceRegistrar(locator, name);
                };
            }

            public Func<TIService, ISetupDescriptor, TIService> OnSetup { get; set; }
            public Action<TIService, ISetupDescriptor> OnChange { get; set; }
            public Action<TIService, IServiceLocator, string> OnServiceRegistrar { get; set; }
            public Func<Action<TIService>, TServiceSetupAction> MakeAction { get; set; }
        }

        /// <summary>
        /// RegisterInstance
        /// </summary>
        public static void RegisterInstance<T>(T service, IServiceLocator locator, string name)
            where T : class
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (name == null)
                locator.Registrar.RegisterInstance<T>(service);
            else
                locator.Registrar.RegisterInstance<T>(service, name);
        }

        #endregion

        #region IServiceSetup

        /// <summary>
        /// ApplySetup
        /// </summary>
        private static TIService ApplySetup(Lazy<TIService> service, TIService newInstance)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (newInstance == null)
                throw new NullReferenceException("instance");
            var registration = Registration;
            if (registration == null)
                throw new NullReferenceException("Registration");
            var onSetup = registration.OnSetup;
            if (onSetup == null)
                return newInstance;
            // find descriptor
            ISetupDescriptor setupDescriptor;
            if (_setupDescriptors.TryGetValue(service, out setupDescriptor))
                _setupDescriptors.Remove(service);
            return onSetup(newInstance, setupDescriptor);
        }

        /// <summary>
        /// ApplyChanges
        /// </summary>
        private static void ApplyChange(Lazy<TIService> service, ISetupDescriptor changeDescriptor)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (!service.IsValueCreated)
                throw new InvalidOperationException("Service value has not been created yet.");
            var registration = Registration;
            if (registration == null)
                throw new NullReferenceException("Registration");
            var onChange = registration.OnChange;
            if (onChange != null)
                onChange(service.Value, changeDescriptor);
        }

        /// <summary>
        /// GetSetupDescriptorProtected
        /// </summary>
        protected static ISetupDescriptor GetSetupDescriptorProtected(Lazy<TIService> service, ISetupDescriptor firstDescriptor)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (service.IsValueCreated)
                return new SetupDescriptor(Registration, d => ApplyChange(service, d));
            ISetupDescriptor descriptor;
            if (_setupDescriptors.TryGetValue(service, out descriptor))
            {
                if (firstDescriptor == null)
                    return descriptor;
                throw new InvalidOperationException(string.Format(Local.RedefineSetupDescriptorA, service.ToString()));
            }
            lock (_lock)
                if (!_setupDescriptors.TryGetValue(service, out descriptor))
                {
                    descriptor = (firstDescriptor ?? new SetupDescriptor(Registration, null));
                    _setupDescriptors.Add(service, descriptor);
                    service.HookValueFactory(valueFactory => ApplySetup(service, LazyValue = valueFactory()));
                }
            return descriptor;
        }

        /// <summary>
        /// ISetupDescriptor
        /// </summary>
        public interface ISetupDescriptor
        {
            void Do(Action<TIService> action);
            void Do(TServiceSetupAction action);
            void RegisterWithServiceLocator(Lazy<TIService> service, string name);
            void RegisterWithServiceLocator(Lazy<TIService> service, Lazy<IServiceLocator> locator, string name);
            void RegisterWithServiceLocator(Lazy<TIService> service, IServiceLocator locator, string name);
            IEnumerable<TServiceSetupAction> Actions { get; }
        }

        /// <summary>
        /// LazySetupDescriptor
        /// </summary>
        protected class SetupDescriptor : ISetupDescriptor
        {
            private List<TServiceSetupAction> _actions = new List<TServiceSetupAction>();
            private SetupRegistration _registration;
            private Action<ISetupDescriptor> _postAction;

            public SetupDescriptor(SetupRegistration registration, Action<ISetupDescriptor> postAction)
            {
                if (registration == null)
                    throw new ArgumentNullException("registration", "Please ensure EnsureRegistration() has been called");
                _registration = registration;
                _postAction = postAction;
            }

            void ISetupDescriptor.Do(Action<TIService> action)
            {
                _actions.Add(_registration.MakeAction(action));
                if (_postAction != null)
                    _postAction(this);
            }

            void ISetupDescriptor.Do(TServiceSetupAction action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                _actions.Add(action);
                if (_postAction != null)
                    _postAction(this);
            }

            void ISetupDescriptor.RegisterWithServiceLocator(Lazy<TIService> service, string name) { ((ISetupDescriptor)this).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, name); }
            void ISetupDescriptor.RegisterWithServiceLocator(Lazy<TIService> service, Lazy<IServiceLocator> locator, string name)
            {
                if (locator == null)
                    throw new ArgumentNullException("locator", "Unable to locate ServiceLocator, please ensure this is defined first.");
                var onServiceRegistrar = _registration.OnServiceRegistrar;
                if (onServiceRegistrar == null)
                    throw new NullReferenceException("registration.ServiceLocatorRegistrar");
                if (!locator.IsValueCreated)
                {
                    var locatorDescriptor = ServiceLocatorManager.GetSetupDescriptor(locator);
                    if (locatorDescriptor == null)
                        throw new NullReferenceException();
                    locatorDescriptor.Do(l => onServiceRegistrar(service.Value, l, name));
                }
                else
                {
                    var descriptor = GetSetupDescriptorProtected(service, null);
                    if (descriptor == null)
                        throw new NullReferenceException();
                    descriptor.Do(s => onServiceRegistrar(s, locator.Value, name));
                }
            }
            void ISetupDescriptor.RegisterWithServiceLocator(Lazy<TIService> service, IServiceLocator locator, string name)
            {
                if (locator == null)
                    throw new ArgumentNullException("locator", "Unable to locate ServiceLocator, please ensure this is defined first.");
                var serviceLocatorRegistrar = _registration.OnServiceRegistrar;
                if (serviceLocatorRegistrar == null)
                    throw new NullReferenceException("registration.ServiceLocatorRegistrar");
                serviceLocatorRegistrar(service.Value, locator, name);
            }

            IEnumerable<TServiceSetupAction> ISetupDescriptor.Actions
            {
                get { return _actions; }
            }
        }

        #endregion
    }
}
