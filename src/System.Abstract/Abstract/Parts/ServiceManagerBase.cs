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
    /// IServiceManager
    /// </summary>
    public interface IServiceManager
    {
    }

    /// <summary>
    /// ServiceManagerBase
    /// </summary>
    public abstract partial class ServiceManagerBase<TIService, TServiceSetupAction, TServiceManagerDebugger> : IServiceManager
        where TIService : class
    {
        private static readonly ConditionalWeakTable<Lazy<TIService>, ISetupDescriptor> _setupDescriptors = new ConditionalWeakTable<Lazy<TIService>, ISetupDescriptor>();
        private static readonly object _lock = new object();

        // Force "precise" initialization
        static ServiceManagerBase() { }

        /// <summary>
        /// Gets or sets the lazy.
        /// </summary>
        /// <value>
        /// The lazy.
        /// </value>
        public static Lazy<TIService> Lazy { get; protected set; }

        /// <summary>
        /// Makes the by provider protected.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="setupDescriptor">The setup descriptor.</param>
        /// <returns></returns>
        [DebuggerStepThroughAttribute]
        public static Lazy<TIService> MakeByProviderProtected(Func<TIService> provider, ISetupDescriptor setupDescriptor)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            var lazy = new Lazy<TIService>(provider);
            GetSetupDescriptorProtected(lazy, setupDescriptor);
            return lazy;
        }

        /// <summary>
        /// 
        /// </summary>
        protected static TIService LazyValue;

        /// <summary>
        /// Gets or sets the debugger.
        /// </summary>
        /// <value>
        /// The debugger.
        /// </value>
        public static TServiceManagerDebugger Debugger { get; set; }

        /// <summary>
        /// Gets or sets the registration.
        /// </summary>
        /// <value>
        /// The registration.
        /// </value>
        protected static ServiceRegistration Registration { get; set; }

        #region Setup

        /// <summary>
        /// ISetupRegistration
        /// </summary>
        public interface ISetupRegistration
        {
            /// <summary>
            /// Gets the on service registrar.
            /// </summary>
            Action<IServiceLocator, string> DefaultServiceRegistrar { get; }
        }

        /// <summary>
        /// ServiceRegistration
        /// </summary>
        protected class ServiceRegistration
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ServiceManagerBase&lt;TIService, TServiceSetupAction, TServiceManagerDebugger&gt;.ServiceRegistration"/> class.
            /// </summary>
            public ServiceRegistration()
            {
                DefaultServiceRegistrar = (service, locator, name) =>
                {
                    //var behavior = (service.Registrar as IServiceRegistrarBehaviorAccessor);
                    //if (behavior != null && !behavior.RegisterInLocator)
                    //    throw new InvalidOperationException();
                    RegisterInstance(service, locator, name);
                    // specific registration
                    var setupRegistration = (service as ISetupRegistration);
                    if (setupRegistration != null)
                        setupRegistration.DefaultServiceRegistrar(locator, name);
                };
            }

            /// <summary>
            /// Gets or sets the on setup.
            /// </summary>
            /// <value>
            /// The on setup.
            /// </value>
            public Func<TIService, ISetupDescriptor, TIService> OnSetup { get; set; }
            /// <summary>
            /// Gets or sets the on change.
            /// </summary>
            /// <value>
            /// The on change.
            /// </value>
            public Action<TIService, ISetupDescriptor> OnChange { get; set; }
            /// <summary>
            /// Gets or sets the on service registrar.
            /// </summary>
            /// <value>
            /// The on service registrar.
            /// </value>
            public Action<TIService, IServiceLocator, string> DefaultServiceRegistrar { get; set; }
            /// <summary>
            /// Gets or sets the make action.
            /// </summary>
            /// <value>
            /// The make action.
            /// </value>
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
        [DebuggerStepThroughAttribute]
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
            /// <summary>
            /// Does the specified action.
            /// </summary>
            /// <param name="action">The action.</param>
            void Do(Action<TIService> action);
            /// <summary>
            /// Does the specified action.
            /// </summary>
            /// <param name="action">The action.</param>
            void Do(TServiceSetupAction action);
            /// <summary>
            /// Registers the with service locator.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="service">The service.</param>
            /// <param name="locator">The locator.</param>
            /// <param name="name">The name.</param>
            void RegisterWithServiceLocator<T>(Lazy<TIService> service, Lazy<IServiceLocator> locator, string name)
                where T : class, TIService;
            /// <summary>
            /// Registers the with service locator.
            /// </summary>
            /// <param name="service">The service.</param>
            /// <param name="locator">The locator.</param>
            /// <param name="name">The name.</param>
            void RegisterWithServiceLocator(Lazy<TIService> service, Lazy<IServiceLocator> locator, string name);
            /// <summary>
            /// Registers the with service locator.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="service">The service.</param>
            /// <param name="locator">The locator.</param>
            /// <param name="name">The name.</param>
            void RegisterWithServiceLocator<T>(Lazy<TIService> service, IServiceLocator locator, string name)
                where T : class, TIService;
            /// <summary>
            /// Registers the with service locator.
            /// </summary>
            /// <param name="service">The service.</param>
            /// <param name="locator">The locator.</param>
            /// <param name="name">The name.</param>
            void RegisterWithServiceLocator(Lazy<TIService> service, IServiceLocator locator, string name);
            /// <summary>
            /// Gets the actions.
            /// </summary>
            IEnumerable<TServiceSetupAction> Actions { get; }
        }

        /// <summary>
        /// LazySetupDescriptor
        /// </summary>
        protected class SetupDescriptor : ISetupDescriptor
        {
            private List<TServiceSetupAction> _actions = new List<TServiceSetupAction>();
            private ServiceRegistration _registration;
            private Action<ISetupDescriptor> _postAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="ServiceManagerBase&lt;TIService, TServiceSetupAction, TDebuggerFlags&gt;.SetupDescriptor"/> class.
            /// </summary>
            /// <param name="registration">The registration.</param>
            /// <param name="postAction">The post action.</param>
            public SetupDescriptor(ServiceRegistration registration, Action<ISetupDescriptor> postAction)
            {
                if (registration == null)
                    throw new ArgumentNullException("registration", "Please ensure EnsureRegistration() has been called");
                _registration = registration;
                _postAction = postAction;
            }

            [DebuggerStepThroughAttribute]
            void ISetupDescriptor.Do(Action<TIService> action)
            {
                _actions.Add(_registration.MakeAction(action));
                if (_postAction != null)
                    _postAction(this);
            }

            [DebuggerStepThroughAttribute]
            void ISetupDescriptor.Do(TServiceSetupAction action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                _actions.Add(action);
                if (_postAction != null)
                    _postAction(this);
            }

            [DebuggerStepThroughAttribute]
            void ISetupDescriptor.RegisterWithServiceLocator<T>(Lazy<TIService> service, Lazy<IServiceLocator> locator, string name)
            {
                if (service == null)
                    throw new ArgumentNullException("service");
                if (locator == null)
                    throw new ArgumentNullException("locator", "Unable to locate ServiceLocator, please ensure this is defined first.");
                if (!locator.IsValueCreated)
                {
                    var descriptor = ServiceLocatorManager.GetSetupDescriptor(locator);
                    if (descriptor == null)
                        throw new NullReferenceException();
                    descriptor.Do(l => RegisterInstance<T>((T)service.Value, l, name));
                }
                else
                {
                    var descriptor = GetSetupDescriptorProtected(service, null);
                    if (descriptor == null)
                        throw new NullReferenceException();
                    descriptor.Do(l => RegisterInstance<T>((T)service.Value, locator.Value, name));
                }
            }
            [DebuggerStepThroughAttribute]
            void ISetupDescriptor.RegisterWithServiceLocator(Lazy<TIService> service, Lazy<IServiceLocator> locator, string name)
            {
                if (service == null)
                    throw new ArgumentNullException("service");
                if (locator == null)
                    throw new ArgumentNullException("locator", "Unable to locate ServiceLocator, please ensure this is defined first.");
                var serviceRegistrar = _registration.DefaultServiceRegistrar;
                if (serviceRegistrar == null)
                    throw new NullReferenceException("registration.ServiceLocatorRegistrar");
                if (!locator.IsValueCreated)
                {
                    // question: should this use RegisterWithServiceLocator below?
                    var descriptor = ServiceLocatorManager.GetSetupDescriptor(locator);
                    if (descriptor == null)
                        throw new NullReferenceException();
                    descriptor.Do(l => serviceRegistrar(service.Value, l, name));
                }
                else
                {
                    var descriptor = GetSetupDescriptorProtected(service, null);
                    if (descriptor == null)
                        throw new NullReferenceException();
                    descriptor.Do(s => serviceRegistrar(s, locator.Value, name));
                }
            }
            [DebuggerStepThroughAttribute]
            void ISetupDescriptor.RegisterWithServiceLocator<T>(Lazy<TIService> service, IServiceLocator locator, string name)
            {
                if (service == null)
                    throw new ArgumentNullException("service");
                if (locator == null)
                    throw new ArgumentNullException("locator", "Unable to locate ServiceLocator, please ensure this is defined first.");
                RegisterInstance<T>((T)service.Value, locator, name);
            }
            [DebuggerStepThroughAttribute]
            void ISetupDescriptor.RegisterWithServiceLocator(Lazy<TIService> service, IServiceLocator locator, string name)
            {
                if (service == null)
                    throw new ArgumentNullException("service");
                if (locator == null)
                    throw new ArgumentNullException("locator", "Unable to locate ServiceLocator, please ensure this is defined first.");
                var serviceRegistrar = _registration.DefaultServiceRegistrar;
                if (serviceRegistrar == null)
                    throw new NullReferenceException("registration.ServiceLocatorRegistrar");
                serviceRegistrar(service.Value, locator, name);
            }

            IEnumerable<TServiceSetupAction> ISetupDescriptor.Actions
            {
                [DebuggerStepThroughAttribute]
                get { return _actions; }
            }
        }

        #endregion
    }
}
