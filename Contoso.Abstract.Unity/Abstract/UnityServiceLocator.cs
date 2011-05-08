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
        IUnityContainer Container { get; }
    }

    /// <summary>
    /// UnityServiceLocator
    /// </summary>
    [Serializable]
    public class UnityServiceLocator : IUnityServiceLocator, IDisposable
    {
        private IUnityContainer _container;
        private UnityServiceRegistrar _registrar;

        public UnityServiceLocator()
            : this(new UnityContainer()) { }
        public UnityServiceLocator(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
            _container.AddNewExtension<UnityStrategiesExtension>();
        }

        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose(); _container = null;
                _registrar = null;
            }
        }

        // registrar
        public IServiceRegistrar GetRegistrar() { return _registrar; }
        public TServiceRegistrar GetRegistrar<TServiceRegistrar>()
            where TServiceRegistrar : class, IServiceRegistrar { return (_registrar as TServiceRegistrar); }

        // resolve
        public TService Resolve<TService>()
            where TService : class
        {
            try { return _container.Resolve<TService>(); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public TService Resolve<TService>(string name)
            where TService : class
        {
            try { return _container.Resolve<TService>(name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public object Resolve(Type serviceType)
        {
            try { return _container.Resolve(serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        public object Resolve(Type serviceType, string name)
        {
            try { return _container.Resolve(serviceType, name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class
        {
            try { return new List<TService>(_container.ResolveAll<TService>()); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return new List<object>(_container.ResolveAll(serviceType)); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class { return (instance == null ? null : (TService)_container.BuildUp(instance.GetType(), instance)); }

        // release and teardown
        public void Release(object instance)
        {
            if (instance != null)
                _container.Teardown(instance);
        }
        public void TearDown<TService>(TService instance)
            where TService : class
        {
            if (instance != null)
                _container.Teardown(instance);
        }
        public void Reset() { Dispose(); }

        #region Domain specific

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
