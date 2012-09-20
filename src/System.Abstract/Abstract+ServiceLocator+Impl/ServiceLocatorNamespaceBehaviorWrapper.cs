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
using System.Collections.Generic;
using System.Abstract.Parts;
namespace System.Abstract
{
    /// <summary>
    /// ServiceLocatorNamespaceBehaviorWrapper
    /// </summary>
    internal class ServiceLocatorNamespaceBehaviorWrapper : IServiceWrapper<IServiceLocator>, IServiceLocator
    {
        private IServiceLocator _parent;
        private IServiceRegistrar _registrar;
        private string _namespace;

        public ServiceLocatorNamespaceBehaviorWrapper(IServiceLocator parent, string @namespace)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(@namespace))
                throw new ArgumentNullException("@namespace");
            _parent = parent;
            _namespace = @namespace;
            _registrar = new ServiceRegistrarNamespaceBehaviorWrapper(this, _parent.Registrar, @namespace);
        }

        // wrapper
        public IServiceLocator Parent
        {
            get { return _parent; }
        }

        public object GetService(Type serviceType) { return _parent.GetService(serviceType); }

        public IServiceLocator CreateChild(object tag) { return _parent.CreateChild(tag); }

        public TContainer GetUnderlyingContainer<TContainer>()
            where TContainer : class { return _parent.GetUnderlyingContainer<TContainer>(); }

        // registrar
        public IServiceRegistrar Registrar
        {
            get { return _registrar; }
        }

        // resolve
        public TService Resolve<TService>()
            where TService : class { return _parent.Resolve<TService>(_namespace); }
        public TService Resolve<TService>(string name)
            where TService : class { return _parent.Resolve<TService>(_namespace + "::" + name); }
        public object Resolve(Type serviceType) { return _parent.Resolve(serviceType, _namespace); }
        public object Resolve(Type serviceType, string name) { return _parent.Resolve(serviceType, _namespace + "::" + name); }
        //
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class { throw new NotSupportedException(); }
        public IEnumerable<object> ResolveAll(Type serviceType) { throw new NotSupportedException(); }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }

        // release and teardown
        public void Release(object instance) { throw new NotSupportedException(); }
        public void TearDown<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }
        public void Reset() { throw new NotSupportedException(); }
    }
}
