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
    /// ServiceRegistrarNamespaceBehaviorWrapper
    /// </summary>
    internal struct ServiceRegistrarNamespaceBehaviorWrapper : IServiceWrapper<IServiceRegistrar>, IServiceRegistrar
    {
        private IServiceLocator _parent;
        private IServiceRegistrar _registrar;
        private string _namespace;

        public ServiceRegistrarNamespaceBehaviorWrapper(IServiceLocator parent, IServiceRegistrar registrar, string @namespace)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(@namespace))
                throw new ArgumentNullException("@namespace");
            _parent = parent;
            _registrar = registrar;
            _namespace = @namespace;
        }

        // wrapper
        public IServiceRegistrar Parent
        {
            get { return _registrar; }
        }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return _registrar.HasRegistered<TService>(); }
        public bool HasRegistered(Type serviceType) { return _registrar.HasRegistered(serviceType); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType) { return _registrar.GetRegistrationsFor(serviceType); }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get { return _registrar.Registrations; }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters
        {
            get { return _registrar.LifetimeForRegisters; }
        }
        public void Register(Type serviceType) { _registrar.Register(serviceType, _namespace); }
        public void Register(Type serviceType, string name) { _registrar.Register(serviceType, _namespace + "::" + name); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { _registrar.Register<TService, TImplementation>(_namespace); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
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
        public void RegisterInstance(Type serviceType, object instance) { _registrar.RegisterInstance(serviceType, instance, _namespace); }
        public void RegisterInstance(Type serviceType, object instance, string name) { _registrar.RegisterInstance(serviceType, instance, _namespace + "::" + name); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { throw new NotSupportedException(); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { throw new NotSupportedException(); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { throw new NotSupportedException(); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { throw new NotSupportedException(); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor) { _registrar.RegisterInterceptor(interceptor); }
    }
}
