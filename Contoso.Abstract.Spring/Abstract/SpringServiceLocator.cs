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
    /// ISpringServiceLocator
    /// </summary>
    public interface ISpringServiceLocator : IServiceLocator
    {
        GenericApplicationContext Container { get; }
    }

    /// <summary>
    /// SpringServiceLocator
    /// </summary>
    [Serializable]
    public class SpringServiceLocator : ISpringServiceLocator, IDisposable, ServiceLocatorManager.ISetupRegistration
    {
        private GenericApplicationContext _container;
        private SpringServiceRegistrar _registrar;

        static SpringServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        public SpringServiceLocator()
            : this(new GenericApplicationContext()) { }
        public SpringServiceLocator(GenericApplicationContext container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
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
            get { return (locator, name) => ServiceLocatorManager.RegisterInstance<ISpringServiceLocator>(this, locator, name); }
        }

        public object GetService(Type serviceType) { return Resolve(serviceType); }

        public TContainer GetUnderlyingContainer<TContainer>()
            where TContainer : class { return (_container as TContainer); }

        // registrar
        public IServiceRegistrar Registrar
        {
            get { return _registrar; }
        }

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
        public void Release(object instance) { throw new NotSupportedException(); }
        public void TearDown<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }

        #region Domain specific

        public GenericApplicationContext Container
        {
            get { return _container; }
            private set
            {
                _container = value;
                _registrar = new SpringServiceRegistrar(this, value);
            }
        }

        #endregion
    }
}
