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
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
namespace Contoso.Abstract
{
    /// <summary>
    /// INinjectServiceLocator
    /// </summary>
    public interface INinjectServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Gets the container.
        /// </summary>
        IKernel Container { get; }
    }

    /// <summary>
    /// NinjectServiceLocator
    /// </summary>
    [Serializable]
    public class NinjectServiceLocator : INinjectServiceLocator, IDisposable, ServiceLocatorManager.ISetupRegistration
    {
        private IKernel _container;
        private NinjectServiceRegistrar _registrar;

        static NinjectServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceLocator"/> class.
        /// </summary>
        public NinjectServiceLocator()
            : this(new StandardKernel(new INinjectModule[0])) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceLocator"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public NinjectServiceLocator(IKernel kernel)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");
            Container = kernel;
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

        Action<IServiceLocator, string> ServiceLocatorManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceLocatorManager.RegisterInstance<INinjectServiceLocator>(this, locator, name); }
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
            //http://stackoverflow.com/questions/5992493/ninject-parent-child-container-heirarchy-like-inheritance
            throw new NotSupportedException();
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
            try { return _container.Get<TService>(new IParameter[0]); }
            //catch (ActivationException activationEx) { return (ResolveAsFirstBinding(activationEx, typeof(TService)) as TService); }
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
            try
            {
                var value = _container.Get<TService>(name, new IParameter[0]);
                if (value == null)
                    throw new ServiceLocatorResolutionException(typeof(TService));
                return value;
            }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public object Resolve(Type serviceType)
        {
            try { return _container.Get(serviceType, new IParameter[0]); }
            //catch (ActivationException activationEx) { return ResolveAsFirstBinding(activationEx, serviceType); }
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
            try { return _container.Get(serviceType, name, new IParameter[0]); }
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
            try { return new List<TService>(_container.GetAll<TService>(new IParameter[0])); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return new List<object>(_container.GetAll(serviceType, new IParameter[0])); }
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
            _container.Inject(instance, new IParameter[0]);
            return instance;
        }

        // release and teardown
        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Release(object instance) { throw new NotSupportedException(); }
        /// <summary>
        /// Tears down.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void TearDown<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }

        #region Domain specific

        /// <summary>
        /// Gets the container.
        /// </summary>
        public IKernel Container
        {
            get { return _container; }
            private set
            {
                _container = value;
                _registrar = new NinjectServiceRegistrar(this, value);
            }
        }

        private object ResolveAsFirstBinding(Exception ex, Type serviceType)
        {
            var firstBinding = Container.GetBindings(serviceType)
                .OrderBy(x => x.Metadata.Name)
                .FirstOrDefault();
            if (firstBinding != null)
                return Container.Get(serviceType, firstBinding.Metadata.Name);
            throw new ServiceLocatorResolutionException(serviceType, ex);
        }

        #endregion
    }
}
