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
using StructureMap.Configuration.DSL;
using StructureMap;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Pipeline;
using System.Collections.Generic;
namespace Contoso.Abstract
{
    /// <summary>
    /// IStructureMapServiceRegistrar
    /// </summary>
    public interface IStructureMapServiceRegistrar : IServiceRegistrar
    {
        void RegisterAll<TService>();
    }

    /// <summary>
    /// StructureMapServiceRegistrar
    /// </summary>
    public class StructureMapServiceRegistrar : Registry, IStructureMapServiceRegistrar, IDisposable
    {
        private StructureMapServiceLocator _parent;
        private IContainer _container;

        public StructureMapServiceRegistrar(StructureMapServiceLocator parent, IContainer container)
        {
            _parent = parent;
            _container = container;
        }

        public void Dispose() { }

        public bool HasPendingRegistrations { get; private set; }

        public void LoadPendingRegistrations()
        {
            _container.Configure(x => x.AddRegistry(this));
            HasPendingRegistrations = false;
        }

        // enumerate
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return _container.Model.HasDefaultImplementationFor(typeof(TService)); }
        public bool HasRegistered(Type serviceType) { return _container.Model.HasDefaultImplementationFor(serviceType); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.Model.AllInstances
                .Where(x => serviceType.IsAssignableFrom(x.ConcreteType))
                .Select(x => new ServiceRegistration { ServiceType = x.PluginType, ServiceName = x.Description });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _container.Model.AllInstances
                    .Select(x => new ServiceRegistration { ServiceType = x.PluginType, ServiceName = x.Description });
            }
        }

        // register type
        public void Register(Type serviceType) { new GenericFamilyExpression(serviceType, this).Use((Instance)new ConfiguredInstance(serviceType)); HasPendingRegistrations = true; }
        public void Register(Type serviceType, string name) { new GenericFamilyExpression(serviceType, this).Use((Instance)new ConfiguredInstance(serviceType) { Name = name }); HasPendingRegistrations = true; }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { new GenericFamilyExpression(typeof(TService), this).Use((Instance)new ConfiguredInstance(typeof(TImplementation))); HasPendingRegistrations = true; }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { new GenericFamilyExpression(typeof(TService), this).Use((Instance)new ConfiguredInstance(typeof(TImplementation)) { Name = name }); HasPendingRegistrations = true; }
        public void Register<TService>(Type implementationType)
            where TService : class { new GenericFamilyExpression(typeof(TService), this).Use((Instance)new ConfiguredInstance(implementationType)); HasPendingRegistrations = true; }
        public void Register<TService>(Type implementationType, string name)
            where TService : class { new GenericFamilyExpression(typeof(TService), this).Use((Instance)new ConfiguredInstance(implementationType) { Name = name }); HasPendingRegistrations = true; }
        public void Register(Type serviceType, Type implementationType) { new GenericFamilyExpression(serviceType, this).Use((Instance)new ConfiguredInstance(implementationType)); HasPendingRegistrations = true; }
        public void Register(Type serviceType, Type implementationType, string name) { new GenericFamilyExpression(serviceType, this).Use((Instance)new ConfiguredInstance(implementationType) { Name = name }); HasPendingRegistrations = true; }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { new GenericFamilyExpression(typeof(TService), this).Use((Instance)new ObjectInstance(instance)); HasPendingRegistrations = true; }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { new GenericFamilyExpression(typeof(TService), this).Use((Instance)new ObjectInstance(instance) { Name = name }); HasPendingRegistrations = true; }
        public void RegisterInstance(Type serviceType, object instance) { new GenericFamilyExpression(serviceType, this).Use((Instance)new ObjectInstance(instance)); HasPendingRegistrations = true; }
        public void RegisterInstance(Type serviceType, object instance, string name) { new GenericFamilyExpression(serviceType, this).Use((Instance)new ObjectInstance(instance) { Name = name }); HasPendingRegistrations = true; }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { new GenericFamilyExpression(typeof(TService), this).Use((Instance)new LambdaInstance<object>(x => factoryMethod(_parent))); HasPendingRegistrations = true; }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { new GenericFamilyExpression(typeof(TService), this).Use((Instance)new LambdaInstance<object>(x => factoryMethod(_parent)) { Name = name }); HasPendingRegistrations = true; }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { new GenericFamilyExpression(serviceType, this).Use((Instance)new LambdaInstance<object>(x => factoryMethod(_parent))); HasPendingRegistrations = true; }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { new GenericFamilyExpression(serviceType, this).Use((Instance)new LambdaInstance<object>(x => factoryMethod(_parent)) { Name = name }); HasPendingRegistrations = true; }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            _container.Configure(x => x.RegisterInterceptor(new Interceptor(interceptor, _container)));
        }

        #region Domain extents

        public void RegisterAll<TService>() { Scan(scanner => scanner.AddAllTypesOf<TService>()); }

        #endregion
    }
}
