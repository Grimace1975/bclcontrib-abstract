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
namespace System.Abstract
{
    /// <summary>
    /// ServiceLocatorNamespaceWrapper
    /// </summary>
    internal class ServiceRegistrarNamespaceWrapper : IServiceRegistrar
    {
        private IServiceLocator _parent;
        private IServiceRegistrar _registrar;
        private string _namespace;

        public ServiceRegistrarNamespaceWrapper(IServiceLocator parent, IServiceRegistrar registrar, string @namespace)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(@namespace))
                throw new ArgumentNullException("@namespace");
            _parent = parent;
            _registrar = registrar;
            _namespace = @namespace;
        }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { throw new NotSupportedException(); }

        // register type
        public void Register(Type serviceType) { _registrar.Register(serviceType, _namespace); }
        public void Register(Type serviceType, string name) { _registrar.Register(serviceType, _namespace + "::" + name); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { _registrar.Register<TService, TImplementation>(_namespace); }
        public void Register<TService, TImplementation>(string name)
            where TImplementation : class, TService { _registrar.Register<TService, TImplementation>(_namespace + "::" + name); }
        public void Register<TService>(Type implementationType)
            where TService : class { _registrar.Register(implementationType, _namespace); }
        public void Register<TService>(Type implementationType, string name)
            where TService : class { _registrar.Register(implementationType, _namespace + "::" + name); }
        public void Register(Type serviceType, Type implementationType) { _registrar.Register(serviceType, implementationType, _namespace); }
        public void Register(Type serviceType, Type implementationType, string name) { _registrar.Register(serviceType, implementationType, _namespace + "::" + name); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { _registrar.RegisterInstance(instance, _namespace); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { _registrar.RegisterInstance(instance, _namespace + "::" + name); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { throw new NotSupportedException(); }
    }
}
