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
    public class StructureMapServiceLocator : IStructureMapServiceLocator, IDisposable
    {
        private IContainer _container;
        private StructureMapServiceRegistrar _registrar;

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
            try { return _container.GetInstance<TService>(); }
            catch (Exception ex) { throw new ServiceResolutionException(typeof(TService), ex); }
        }
        public TService Resolve<TService>(string key)
            where TService : class
        {
            try { return _container.GetInstance<TService>(key); }
            catch (Exception ex) { throw new ServiceResolutionException(typeof(TService), ex); }
        }
        public object Resolve(Type serviceType)
        {
            try { return _container.GetInstance(serviceType); }
            catch (Exception ex) { throw new ServiceResolutionException(serviceType, ex); }
        }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            try { return _container.GetAllInstances(serviceType).Cast<object>(); }
            catch (Exception ex) { throw new ServiceResolutionException(serviceType, ex); }
        }
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class { return _container.GetAllInstances<TService>(); }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class
        {
            if (instance == null)
                return null;
            _container.BuildUp(instance);
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
            get { return _container; }
            private set
            {
                _container = value;
                _registrar = new StructureMapServiceRegistrar(this, value);
            }
        }

        #endregion
    }
}
