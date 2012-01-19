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
        public NinjectServiceLocator()
            : this(new StandardKernel(new INinjectModule[0])) { }
        public NinjectServiceLocator(IKernel kernel)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");
            Container = kernel;
        }

        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose(); _container = null;
                _registrar = null;
            }
        }

        Action<IServiceLocator, string> ServiceLocatorManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceLocatorManager.RegisterInstance<INinjectServiceLocator>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // registrar
        public IServiceRegistrar Registrar
        {
            get { return _registrar; }
        }
        public TServiceRegistrar GetRegistrar<TServiceRegistrar>()
            where TServiceRegistrar : class, IServiceRegistrar { return (_registrar as TServiceRegistrar); }

        // resolve
        public TService Resolve<TService>()
            where TService : class
        {
            try { return _container.Get<TService>(new IParameter[0]); }
            //catch (ActivationException activationEx) { return (ResolveAsFirstBinding(activationEx, typeof(TService)) as TService); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
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
        public object Resolve(Type serviceType)
        {
            try { return _container.Get(serviceType, new IParameter[0]); }
            //catch (ActivationException activationEx) { return ResolveAsFirstBinding(activationEx, serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        public object Resolve(Type serviceType, string name)
        {
            try { return _container.Get(serviceType, name, new IParameter[0]); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        public IEnumerable<TService> ResolveAll<TService>()
              where TService : class
        {
            try { return new List<TService>(_container.GetAll<TService>(new IParameter[0])); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return new List<object>(_container.GetAll(serviceType, new IParameter[0])); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class
        {
            if (instance == null)
                return null;
            _container.Inject(instance, new IParameter[0]);
            return instance;
        }

        // release and teardown
        public void Release(object instance) { throw new NotSupportedException(); }
        public void TearDown<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }

        #region Domain specific

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
