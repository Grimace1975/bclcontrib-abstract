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
using Autofac;
namespace Contoso.Abstract
{
    /// <summary>
    /// IAutofacServiceLocator
    /// </summary>
    public interface IAutofacServiceLocator : IServiceLocator
    {
        IContainer Container { get; }
    }

    /// <summary>
    /// AutofacServiceLocator
    /// </summary>
    [Serializable]
    public class AutofacServiceLocator : IAutofacServiceLocator, IDisposable
    {
        private IContainer _container;
        private AutofacServiceRegistrar _registrar;
        private Func<IContainer> _containerBuilder;

        static AutofacServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        public AutofacServiceLocator()
            : this(new ContainerBuilder()) { }
        public AutofacServiceLocator(ContainerBuilder container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            _registrar = new AutofacServiceRegistrar(this, container, out _containerBuilder);
            ServiceLocatorManager.ApplySetup(this);
        }

        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose(); _container = null;
                _registrar = null;
                _containerBuilder = null;
            }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // registrar
        public IServiceRegistrar GetRegistrar() { return _registrar; }
        public TServiceRegistrar GetRegistrar<TServiceRegistrar>()
            where TServiceRegistrar : class, IServiceRegistrar { return (_registrar as TServiceRegistrar); }

        // resolve
        public TService Resolve<TService>()
            where TService : class
        {
            try { return (Container.IsRegistered<TService>() ? Container.Resolve<TService>() : Activator.CreateInstance<TService>()); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public TService Resolve<TService>(string name)
            where TService : class
        {
            try { return Container.ResolveNamed<TService>(name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public object Resolve(Type serviceType)
        {
            try { return (Container.IsRegistered(serviceType) ? Container.Resolve(serviceType) : Activator.CreateInstance(serviceType)); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        public object Resolve(Type serviceType, string name)
        {
            try { return Container.ResolveNamed(name, serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class
        {
            try { return new List<TService>(Container.Resolve<IEnumerable<TService>>()); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
            try { return new List<object>((IEnumerable<object>)Container.Resolve(type)); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class { return Container.InjectProperties(instance); }

        // release and teardown
        [Obsolete("Not used for any real purposes.")]
        public void Release(object instance) { throw new NotSupportedException(); }
        [Obsolete("Not used for any real purposes.")]
        public void TearDown<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }
        public void Reset() { Dispose(); }

        #region Domain specific

        public IContainer Container
        {
            get
            {
                if (_container == null)
                    _container = _containerBuilder();
                return _container;
            }
            private set { _container = value; }
        }

        #endregion
    }
}
