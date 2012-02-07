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
    /// ServiceRegistrarLifetimeBehavorWrapper
    /// </summary>
    internal struct ServiceRegistrarLifetimeBehavorWrapper : IServiceRegistrar, IServiceRegistrarBehavior
    {
        private IServiceRegistrar _parent;
        private ServiceRegistrarLifetime _lifetime;

        public ServiceRegistrarLifetimeBehavorWrapper(IServiceRegistrar parent, ServiceRegistrarLifetime lifetime)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            _parent = parent;
            _lifetime = lifetime;
        }

        public IServiceRegistrar Parent
        {
            get { return _parent; }
        }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent.Locator; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return _parent.HasRegistered<TService>(); }
        public bool HasRegistered(Type serviceType) { return _parent.HasRegistered(serviceType); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType) { return _parent.GetRegistrationsFor(serviceType); }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get { return _parent.Registrations; }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters
        {
            get { return _parent.LifetimeForRegisters; }
        }
        public void Register(Type serviceType) { _parent.Register(serviceType); }
        public void Register(Type serviceType, string name) { _parent.Register(serviceType, name); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { }
        public void Register<TService>(Type implementationType)
            where TService : class { }
        public void Register<TService>(Type implementationType, string name)
            where TService : class { }
        public void Register(Type serviceType, Type implementationType) { _parent.Register(serviceType, implementationType); }
        public void Register(Type serviceType, Type implementationType, string name) { _parent.Register(serviceType, implementationType, name); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { _parent.RegisterInstance(instance); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { _parent.RegisterInstance(instance, name); }
        public void RegisterInstance(Type serviceType, object instance) { _parent.RegisterInstance(serviceType, instance); }
        public void RegisterInstance(Type serviceType, object instance, string name) { _parent.RegisterInstance(serviceType, instance, name); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { _parent.Register<TService>(factoryMethod); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { _parent.Register<TService>(factoryMethod, name); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { _parent.Register(serviceType, factoryMethod); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { _parent.Register(serviceType, factoryMethod, name); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor) { throw new NotSupportedException(); }

        ServiceRegistrarLifetime IServiceRegistrarBehavior.Lifetime
        {
            get { return _lifetime; }
            set { _lifetime = value; }
        }
    }
}
