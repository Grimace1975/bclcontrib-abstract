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
using Castle.MicroKernel.Registration;
using System.Reflection;
using Castle.Core;
namespace Contoso.Abstract
{
    /// <summary>
    /// ICastleWindsorServiceRegistrar
    /// </summary>
    public interface ICastleWindsorServiceRegistrar : IServiceRegistrar
    {
        //void RegisterAll<Source>();
    }

    /// <summary>
    /// CastleWindsorServiceRegistrar
    /// </summary>
    public class CastleWindsorServiceRegistrar : ICastleWindsorServiceRegistrar, IDisposable
    {
        private CastleWindsorServiceLocator _parent;
        private IWindsorContainer _container;

        public CastleWindsorServiceRegistrar(CastleWindsorServiceLocator parent, IWindsorContainer container)
        {
            _parent = parent;
            _container = container;
        }

        public void Dispose() { }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return HasRegistered(typeof(TService)); }
        public bool HasRegistered(Type serviceType) { return _container.Kernel.HasComponent(serviceType); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.Kernel.GetAssignableHandlers(serviceType)
                .Select(x => new ServiceRegistration { ImplementationType = x.ComponentModel.Implementation });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _container.Kernel.GetAssignableHandlers(typeof(object))
                    .Select(x => new ServiceRegistration { ImplementationType = x.ComponentModel.Implementation });
            }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters
        {
            get { return ServiceRegistrarLifetime.Transient; }
        }
        public void Register(Type serviceType) { _container.Register(Component.For(serviceType).LifeStyle.Is(GetLifetime())); }
        public void Register(Type serviceType, string name) { _container.Register(Component.For(serviceType).Named(name).LifeStyle.Is(GetLifetime())); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { EnsureTransientLifestyle(); _container.Register(Component.For<TService>().ImplementedBy<TImplementation>().LifeStyle.Is(GetLifetime())); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { EnsureTransientLifestyle(); _container.Register(Component.For<TService>().Named(name).ImplementedBy<TImplementation>().LifeStyle.Is(GetLifetime())); }
        public void Register<TService>(Type implementationType)
             where TService : class { EnsureTransientLifestyle(); _container.Register(Component.For<TService>().ImplementedBy(implementationType).LifeStyle.Is(GetLifetime())); }
        public void Register<TService>(Type implementationType, string name)
             where TService : class { EnsureTransientLifestyle(); _container.Register(Component.For<TService>().Named(name).ImplementedBy(implementationType).LifeStyle.Is(GetLifetime())); }
        public void Register(Type serviceType, Type implementationType) { EnsureTransientLifestyle(); _container.Register(Component.For(serviceType).ImplementedBy(implementationType).LifeStyle.Is(GetLifetime())); }
        public void Register(Type serviceType, Type implementationType, string name) { EnsureTransientLifestyle(); _container.Register(Component.For(serviceType).Named(name).ImplementedBy(implementationType).LifeStyle.Is(GetLifetime())); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { _container.Register(Component.For<TService>().Instance(instance)); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { _container.Register(Component.For<TService>().Named(name).Instance(instance)); }
        public void RegisterInstance(Type serviceType, object instance) { _container.Register(Component.For(serviceType).Instance(instance)); }
        public void RegisterInstance(Type serviceType, object instance, string name) { _container.Register(Component.For(serviceType).Named(name).Instance(instance)); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { _container.Register(Component.For<TService>().UsingFactoryMethod<TService>(x => factoryMethod(_parent)).LifeStyle.Is(GetLifetime())); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { _container.Register(Component.For<TService>().UsingFactoryMethod<TService>(x => factoryMethod(_parent)).Named(name).LifeStyle.Is(GetLifetime())); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { _container.Register(Component.For(serviceType).UsingFactoryMethod(x => factoryMethod(_parent)).LifeStyle.Is(GetLifetime())); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { _container.Register(Component.For(serviceType).UsingFactoryMethod(x => factoryMethod(_parent)).Named(name).LifeStyle.Is(GetLifetime())); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            _container.Kernel.ComponentModelCreated +=
                model =>
                {
                    if (interceptor.Match(model.Implementation))
                        interceptor.ItemCreated(model.Implementation, model.LifestyleType == LifestyleType.Transient);
                };
        }

        #region Domain specific

        //public void RegisterAll<TService>() { AllTypes.Of<TService>(); }
        //private static string MakeId(Type serviceType, Type implementationType) { return serviceType.Name + "->" + implementationType.FullName; }

        #endregion

        private void EnsureTransientLifestyle()
        {
            if (LifetimeForRegisters != ServiceRegistrarLifetime.Transient)
                throw new NotSupportedException();
        }

        private LifestyleType GetLifetime()
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return LifestyleType.Transient;
                case ServiceRegistrarLifetime.Singleton: return LifestyleType.Singleton;
                case ServiceRegistrarLifetime.Thread: return LifestyleType.Thread;
                case ServiceRegistrarLifetime.Pooled: return LifestyleType.Pooled;
                case ServiceRegistrarLifetime.Request: return LifestyleType.PerWebRequest;
                default: throw new NotSupportedException();
            }
        }
    }
}
