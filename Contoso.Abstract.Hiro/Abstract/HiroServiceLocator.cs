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
using Hiro;
using Hiro.Containers;
namespace Contoso.Abstract
{
    /// <summary>
    /// IHiroServiceLocator
    /// </summary>
    public interface IHiroServiceLocator : IServiceLocator
    {
        DependencyMap Builder { get; }
        IMicroContainer Container { get; }
    }

    /// <summary>
    /// HiroServiceLocator
    /// </summary>
    [Serializable]
    public class HiroServiceLocator : IHiroServiceLocator, IDisposable
    {
        private IMicroContainer _container;
        private HiroServiceRegistrar _registrar;
        private Func<IMicroContainer> _containerBuilder;

        static HiroServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        public HiroServiceLocator()
            : this(new DependencyMap()) { }
        public HiroServiceLocator(DependencyMap builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            Builder = builder;
            _registrar = new HiroServiceRegistrar(this, builder, out _containerBuilder);
            ServiceLocatorManager.ApplySetup(this);
        }
        //public HiroServiceLocator(IMicroContainer container)
        //{
        //    if (container == null)
        //        throw new ArgumentNullException("container");
        //    Container = container;
        //    ServiceLocatorManager.Setup(this);
        //}

        public void Dispose()
        {
            if (_container != null)
            {
                _container = null;
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
            try { return (Builder.Contains(typeof(TService)) ? Container.GetInstance<TService>() : Activator.CreateInstance<TService>()); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public TService Resolve<TService>(string name)
            where TService : class
        {
            if (!Builder.Contains(typeof(TService), name))
                throw new ServiceLocatorResolutionException(typeof(TService), string.Format("Unregistered '{0}'", name));
            try { return Container.GetInstance<TService>(name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public object Resolve(Type serviceType)
        {
            try { return (Builder.Contains(serviceType) ? Container.GetInstance(serviceType, null) : Activator.CreateInstance(serviceType)); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        public object Resolve(Type serviceType, string name)
        {
            if (!Builder.Contains(serviceType, name))
                throw new ServiceLocatorResolutionException(serviceType, string.Format("Unregistered '{0}'", name));
            try { return Container.GetInstance(serviceType, name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class
        {
            try { return new List<TService>(Container.GetAllInstances(typeof(TService)).Cast<TService>()); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return new List<object>(Container.GetAllInstances(serviceType)); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class { return null; }

        // release and teardown
        public void Release(object instance) { }
        public void TearDown<TService>(TService instance)
            where TService : class { }
        public void Reset() { Dispose(); }

        #region Domain specific

        public DependencyMap Builder { get; private set; }

        public IMicroContainer Container
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
