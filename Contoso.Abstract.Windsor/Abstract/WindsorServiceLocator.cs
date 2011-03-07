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
    /// IWindsorServiceLocator
    /// </summary>
    public interface IWindsorServiceLocator : IServiceLocator
    {
        IWindsorContainer Container { get; }
    }

    /// <summary>
    /// WindsorServiceLocator
    /// </summary>
    [Serializable]
    public class WindsorServiceLocator : IWindsorServiceLocator, IDisposable
    {
        private IWindsorContainer _container;
        private IKernel _kernel;
        private WindsorServiceRegistrar _registrar;

        public WindsorServiceLocator()
            : this(CreateContainer()) { }
        public WindsorServiceLocator(IWindsorContainer container)
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
                _kernel = null;
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
            try { return _kernel.Resolve<TService>(); }
            catch (Exception ex) { throw new ServiceResolutionException(typeof(TService), ex); }
        }
        public TService Resolve<TService>(string id)
            where TService : class
        {
            try { return _kernel.Resolve<TService>(id); }
            catch (Exception ex) { throw new ServiceResolutionException(typeof(TService), ex); }
        }
        public object Resolve(Type serviceType)
        {
            try { return _kernel.Resolve(serviceType); }
            catch (Exception ex) { throw new ServiceResolutionException(serviceType, ex); }
        }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return new List<object>(_kernel.ResolveAll(serviceType).Cast<object>()); }
            catch (Exception ex) { throw new ServiceResolutionException(serviceType, ex); }
        }
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class
        {
            try { return new List<TService>(_kernel.ResolveAll<TService>()); }
            catch (Exception ex) { throw new ServiceResolutionException(typeof(TService), ex); }
        }

        // inject
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
        public void Release(object instance)
        {
            if (instance != null)
                _container.Release(instance);
        }
        public void TearDown<TService>(TService instance)
            where TService : class
        {
            if (instance != null)
                foreach (var property in instance.GetType().GetProperties().Where(x => _kernel.HasComponent(x.PropertyType)))
                    _container.Release(property.GetValue(instance, null));
        }
        public void Reset() { Dispose(); }

        #region Domain specific

        public IWindsorContainer Container
        {
            get { return _container; }
            private set
            {
                _container = value;
                _kernel = _container.Kernel;
                _registrar = new WindsorServiceRegistrar(this, value);
            }
        }

        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();
            var kernel = container.Kernel;
            kernel.Resolver.AddSubResolver(new ArrayResolver(kernel));
            kernel.Resolver.AddSubResolver(new ListResolver(kernel));
            return container;
        }

        #endregion
    }
}
