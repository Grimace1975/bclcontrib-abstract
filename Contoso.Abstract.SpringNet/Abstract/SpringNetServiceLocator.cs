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
using Spring.Objects.Factory;
using Spring.Context.Support;
using Spring.Context;
namespace Contoso.Abstract
{
    /// <summary>
    /// ISpringNetServiceLocator
    /// </summary>
    public interface ISpringNetServiceLocator : IServiceLocator
    {
        GenericApplicationContext Container { get; }
    }

    /// <summary>
    /// SpringNetServiceLocator
    /// </summary>
    [Serializable]
    public class SpringNetServiceLocator : ISpringNetServiceLocator, IDisposable
    {
        private GenericApplicationContext  _container;
        private SpringNetServiceRegistrar _registrar;

        static SpringNetServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        public SpringNetServiceLocator()
            : this(new GenericApplicationContext()) { }
        public SpringNetServiceLocator(GenericApplicationContext container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
            ServiceLocatorManager.ApplySetup(this);
        }

        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose(); _container = null;
                _registrar = null;
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
            var serviceType = typeof(TService);
            try { return (TService)_container.GetObject(GetName(serviceType), serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }

        internal static string GetName(Type serviceType) { return serviceType.FullName; }

        public TService Resolve<TService>(string name)
            where TService : class
        {
            var serviceType = typeof(TService);
            try { return (TService)_container.GetObject(name, serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public object Resolve(Type serviceType)
        {
            try { return _container.GetObject(GetName(serviceType), serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        public object Resolve(Type serviceType, string name)
        {
            try { return _container.GetObject(name, serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        public IEnumerable<TService> ResolveAll<TService>()
              where TService : class { throw new NotSupportedException(); }
        public IEnumerable<object> ResolveAll(Type serviceType) { throw new NotSupportedException(); }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class
        {
            try { return (TService)_container.ConfigureObject(instance, GetName(typeof(TService))); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }

        // release and teardown
        [Obsolete("Not used with this implementation of IServiceLocator.")]
        public void Release(object instance) { throw new NotSupportedException(); }
        [Obsolete("Not used with this implementation of IServiceLocator.")]
        public void TearDown<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }
        public void Reset() { Dispose(); }

        #region Domain specific

        public GenericApplicationContext Container
        {
            get { return _container; }
            private set
            {
                _container = value;
                _registrar = new SpringNetServiceRegistrar(this, value);
            }
        }

        #endregion
    }
}
