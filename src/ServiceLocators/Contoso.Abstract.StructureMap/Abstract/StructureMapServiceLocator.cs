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
using System.Reflection;
using StructureMap;
namespace Contoso.Abstract
{
    /// <summary>
    /// IStructureMapServiceLocator
    /// </summary>
    public interface IStructureMapServiceLocator : IServiceLocator
    {
        IContainer Container { get; }
    }

    /// <summary>
    /// StructureMapServiceLocator
    /// </summary>
    [Serializable]
    public class StructureMapServiceLocator : IStructureMapServiceLocator, IDisposable, ServiceLocatorManager.ISetupRegistration
    {
        private IContainer _container;
        private StructureMapServiceRegistrar _registrar;

        static StructureMapServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        public StructureMapServiceLocator()
            : this(new Container()) { }
        public StructureMapServiceLocator(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
        }

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
            get { return (locator, name) => ServiceLocatorManager.RegisterInstance<IStructureMapServiceLocator>(this, locator, name); }
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
            try { return (TService)Container.GetInstance<TService>(); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public TService Resolve<TService>(string name)
            where TService : class
        {
            try { return (TService)Container.GetInstance<TService>(name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(typeof(TService), ex); }
        }
        public object Resolve(Type serviceType)
        {
            try { return Container.GetInstance(serviceType); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        public object Resolve(Type serviceType, string name)
        {
            try { return Container.GetInstance(serviceType, name); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class { return Container.GetAllInstances<TService>(); }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return Container.GetAllInstances(serviceType).Cast<object>(); }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class
        {
            if (instance == null)
                return null;
            Container.BuildUp(instance);
            foreach (var property in instance.GetType().GetProperties().Where(x => x.CanWrite && _container.Model.HasImplementationsFor(x.PropertyType)))
                property.SetValue(instance, _container.GetInstance(property.PropertyType), null);
            return instance;
        }

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
                if (_registrar.HasPendingRegistrations)
                    _registrar.LoadPendingRegistrations();
                return _container;
            }
            private set
            {
                _container = value;
                _registrar = new StructureMapServiceRegistrar(this, value);
            }
        }

        #endregion
    }
}
