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
    /// IHiroServiceRegistrar
    /// </summary>
    public interface IHiroServiceRegistrar : IServiceRegistrar
    {
    }

    /// <summary>
    /// HiroServiceRegistrar
    /// </summary>
    public class HiroServiceRegistrar : IHiroServiceRegistrar, IDisposable
    {
        private HiroServiceLocator _parent;
        private DependencyMap _builder;
        private IMicroContainer _container;

        public HiroServiceRegistrar(HiroServiceLocator parent, DependencyMap builder, out Func<IMicroContainer> containerBuilder)
        {
            _parent = parent;
            _builder = builder;
            containerBuilder = (() => _container = _builder.CreateContainer());
        }

        public void Dispose() { }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // enumerate
        public bool HasRegistered<TService>() { return _builder.Contains(typeof(TService)); }
        public bool HasRegistered(Type serviceType) { return _builder.Contains(serviceType); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _builder.Dependencies
                .Where(x => serviceType.IsAssignableFrom(x.ServiceType))
                .Select(x => new ServiceRegistration { ServiceType = x.ServiceType, ServiceName = x.ServiceName });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _builder.Dependencies
                    .Select(x => new ServiceRegistration { ServiceType = x.ServiceType, ServiceName = x.ServiceName });
            }
        }

        // register type
        public void Register(Type serviceType) { _builder.AddService(serviceType, serviceType); }
        public void Register(Type serviceType, string name) { _builder.AddService(name, serviceType, serviceType); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { _builder.AddService<TService, TImplementation>(); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { _builder.AddService<TService, TImplementation>(name); }
        public void Register<TService>(Type implementationType)
             where TService : class { _builder.AddService(typeof(TService), implementationType); }
        public void Register<TService>(Type implementationType, string name)
             where TService : class { _builder.AddService(name, typeof(TService), implementationType); }
        public void Register(Type serviceType, Type implementationType) { _builder.AddService(serviceType, implementationType); }
        public void Register(Type serviceType, Type implementationType, string name) { _builder.AddService(name, serviceType, implementationType); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { Func<IMicroContainer, TService> f = (x => instance); _builder.AddService<TService>(f); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { Func<IMicroContainer, TService> f = (x => instance); _builder.AddService<TService>(name, f); }
        public void RegisterInstance(Type serviceType, object instance) { Func<IMicroContainer, object> f = (x => instance); _builder.AddService(serviceType, f); }
        public void RegisterInstance(Type serviceType, object instance, string name) { Func<IMicroContainer, object> f = (x => instance); _builder.AddService(name, serviceType, f); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { Func<IMicroContainer, TService> f = (x => factoryMethod(_parent)); _builder.AddService<TService>(f); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { Func<IMicroContainer, TService> f = (x => factoryMethod(_parent)); _builder.AddService<TService>(name, f); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { Func<IMicroContainer, object> f = (x => factoryMethod(_parent)); _builder.AddService(serviceType, f); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { Func<IMicroContainer, object> f = (x => factoryMethod(_parent)); _builder.AddService(name, serviceType, f); }
    }
}
