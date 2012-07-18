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
using System.Abstract;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Contoso.Abstract.Internal;
namespace Contoso.Abstract
{
    /// <summary>
    /// IUnityServiceLocator
    /// </summary>
    public interface IUnityServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Gets the container.
        /// </summary>
        IUnityContainer Container { get; }
    }

    /// <summary>
    /// UnityServiceLocator
    /// </summary>
    [Serializable]
    public class UnityServiceLocator : IUnityServiceLocator, IDisposable, ServiceLocatorManager.ISetupRegistration
    {
        private IUnityContainer _container;
        private UnityServiceRegistrar _registrar;

        static UnityServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityServiceLocator"/> class.
        /// </summary>
        public UnityServiceLocator()
            : this(new UnityContainer()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityServiceLocator"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public UnityServiceLocator(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
            _container.AddNewExtension<UnityStrategiesExtension>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_container != null)
            {
                var container = _container;
                _container = null;
                _registrar = null;
                // prevent cyclical dispose
                if (container != null)
                    container.Dispose();
            }
        }

        Action<IServiceLocator, string> ServiceLocatorManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceLocatorManager.RegisterInstance<IUnityServiceLocator>(this, locator, name); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType) { return Resolve(serviceType); }

        /// <summary>
        /// Gets the underlying container.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <returns></returns>
        public TContainer GetUnderlyingContainer<TContainer>()
            where TContainer : class { return (_container as TContainer); }

        // registrar
        /// <summary>
        /// Gets the registrar.
        /// </summary>
        public IServiceRegistrar Registrar
        {
            get { return _registrar; }
        }

        // resolve
        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public TService Resolve<TService>()
            where TService : class
        {
            try { return _container.Resolve<TService>(); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        /// <summary>
        /// Resolves the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TService Resolve<TService>(string name)
            where TService : class
        {
            try { return _container.Resolve<TService>(name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public object Resolve(Type serviceType)
        {
            try { return _container.Resolve(serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object Resolve(Type serviceType, string name)
        {
            try { return _container.Resolve(serviceType, name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class
        {
            try { return new List<TService>(_container.ResolveAll<TService>()); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return new List<object>(_container.ResolveAll(serviceType)); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }

        // inject
        /// <summary>
        /// Injects the specified instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public TService Inject<TService>(TService instance)
            where TService : class { return (instance == null ? null : (TService)_container.BuildUp(instance.GetType(), instance)); }

        // release and teardown
        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Release(object instance)
        {
            if (instance != null)
                _container.Teardown(instance);
        }
        /// <summary>
        /// Tears down.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void TearDown<TService>(TService instance)
            where TService : class
        {
            if (instance != null)
                _container.Teardown(instance);
        }

        #region Domain specific

        /// <summary>
        /// Gets the container.
        /// </summary>
        public IUnityContainer Container
        {
            get { return _container; }
            private set
            {
                _container = value;
                _registrar = new UnityServiceRegistrar(this, value);
            }
        }

        #endregion
    }
}
