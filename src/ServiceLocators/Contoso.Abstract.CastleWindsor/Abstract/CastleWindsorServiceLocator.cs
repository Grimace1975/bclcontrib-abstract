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
using System.Linq;
using System.Abstract;
using System.Collections.Generic;
using Castle.Windsor;
using Castle.MicroKernel;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
namespace Contoso.Abstract
{
    /// <summary>
    /// ICastleWindsorServiceLocator
    /// </summary>
    public interface ICastleWindsorServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Gets the container.
        /// </summary>
        IWindsorContainer Container { get; }
    }

    /// <summary>
    /// CastleWindsorServiceLocator
    /// </summary>
    [Serializable]
    public class CastleWindsorServiceLocator : ICastleWindsorServiceLocator, IDisposable, ServiceLocatorManager.ISetupRegistration
    {
        private IWindsorContainer _container;
        private IKernel _kernel;
        private CastleWindsorServiceRegistrar _registrar;

        static CastleWindsorServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="CastleWindsorServiceLocator"/> class.
        /// </summary>
        public CastleWindsorServiceLocator()
            : this(CreateContainer()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CastleWindsorServiceLocator"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public CastleWindsorServiceLocator(IWindsorContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
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
                _kernel = null;
                _registrar = null;
                // prevent cyclical dispose
                if (container != null)
                    container.Dispose();
            }
        }

        Action<IServiceLocator, string> ServiceLocatorManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceLocatorManager.RegisterInstance<ICastleWindsorServiceLocator>(this, locator, name); }
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
        /// Creates the child.
        /// </summary>
        /// <returns></returns>
        public IServiceLocator CreateChild(object tag)
        {
            var childContainer = CreateContainer();
            _container.AddChildContainer(childContainer);
            return new CastleWindsorServiceLocator(childContainer);
        }

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
            try { return (_kernel.HasComponent(typeof(TService)) ? _kernel.Resolve<TService>() : Activator.CreateInstance<TService>()); }
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
            try { return _kernel.Resolve<TService>(name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public object Resolve(Type serviceType)
        {
            try { return (_kernel.HasComponent(serviceType) ? _kernel.Resolve(serviceType) : Activator.CreateInstance(serviceType)); }
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
            try { return _kernel.Resolve(name, serviceType); }
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
            try { return new List<TService>(_kernel.ResolveAll<TService>()); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return new List<object>(_kernel.ResolveAll(serviceType).Cast<object>()); }
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
            where TService : class
        {
            if (instance == null)
                return null;
            foreach (var property in instance.GetType().GetProperties().Where(x => x.CanWrite && _kernel.HasComponent(x.PropertyType)))
                property.SetValue(instance, _kernel.Resolve(property.PropertyType), null);
            return instance;
        }

        // release and teardown
        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Release(object instance)
        {
            if (instance != null)
                _container.Release(instance);
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
                foreach (var property in instance.GetType().GetProperties().Where(x => _kernel.HasComponent(x.PropertyType)))
                    _container.Release(property.GetValue(instance, null));
        }

        #region Domain specific

        /// <summary>
        /// Gets the container.
        /// </summary>
        public IWindsorContainer Container
        {
            get { return _container; }
            private set
            {
                _container = value;
                _kernel = _container.Kernel;
                _registrar = new CastleWindsorServiceRegistrar(this, value);
            }
        }

        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();
            //var kernel = container.Kernel;
            //kernel.Resolver.AddSubResolver(new ArrayResolver(kernel));
            //kernel.Resolver.AddSubResolver(new ListResolver(kernel));
            //kernel.AddFacility<Castle.Facilities.FactorySupport.FactorySupportFacility>();
            return container;
        }

        #endregion
    }
}
