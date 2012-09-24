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
using Munq;
namespace Contoso.Abstract
{
    /// <summary>
    /// IUnityServiceRegistrar
    /// </summary>
    public interface IMunqServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// UnityServiceRegistrar
    /// </summary>
    internal class MunqServiceRegistrar : IMunqServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private MunqServiceLocator _parent;
        private IocContainer _container;

        public MunqServiceRegistrar(MunqServiceLocator parent, IocContainer container)
        {
            _parent = parent;
            _container = container;
        }

        public void Dispose() { }
        object ICloneable.Clone() { return MemberwiseClone(); }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return _container.CanResolve(null, typeof(TService)); }
        public bool HasRegistered(Type serviceType) { return _container.CanResolve(null, serviceType); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.GetRegistrations(serviceType)
                .Select(x => new ServiceRegistration { ServiceType = null, ImplementationType = x.ResolvesTo, Name = x.Name });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get { throw new NotImplementedException(); }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters { get; private set; }
        public void Register(Type serviceType) { SetLifetime(_container.Register(serviceType, serviceType)); }
        public void Register(Type serviceType, string name) { SetLifetime(_container.Register(name, serviceType, serviceType)); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { SetLifetime(_container.Register<TService, TImplementation>()); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { SetLifetime(_container.Register<TService, TImplementation>(name)); }
        public void Register<TService>(Type implementationType)
           where TService : class { SetLifetime(_container.Register(typeof(TService), implementationType)); }
        public void Register<TService>(Type implementationType, string name)
           where TService : class { SetLifetime(_container.Register(name, typeof(TService), implementationType)); }
        public void Register(Type serviceType, Type implementationType) { SetLifetime(_container.Register(serviceType, implementationType)); }
        public void Register(Type serviceType, Type implementationType, string name) { SetLifetime(_container.Register(name, serviceType, implementationType)); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { EnsureTransientLifestyle(); _container.RegisterInstance<TService>(instance); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { EnsureTransientLifestyle(); _container.RegisterInstance<TService>(name, instance); }
        public void RegisterInstance(Type serviceType, object instance) { EnsureTransientLifestyle(); _container.RegisterInstance(serviceType, instance); }
        public void RegisterInstance(Type serviceType, object instance, string name) { EnsureTransientLifestyle(); _container.RegisterInstance(name, serviceType, instance); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { SetLifetime(_container.Register<TService>((Func<IDependencyResolver, TService>)(c => factoryMethod(_parent)))); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { SetLifetime(_container.Register<TService>(name, (Func<IDependencyResolver, TService>)(c => factoryMethod(_parent)))); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { SetLifetime(_container.Register(serviceType, (Func<IDependencyResolver, object>)(c => factoryMethod(_parent)))); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { SetLifetime(_container.Register(name, serviceType, (Func<IDependencyResolver, object>)(c => factoryMethod(_parent)))); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor) { throw new NotSupportedException(); }

        #region Behavior

        bool IServiceRegistrarBehaviorAccessor.RegisterInLocator
        {
            get { return true; }
        }

        ServiceRegistrarLifetime IServiceRegistrarBehaviorAccessor.Lifetime
        {
            get { return LifetimeForRegisters; }
            set { LifetimeForRegisters = value; }
        }

        #endregion

        private void EnsureTransientLifestyle()
        {
            if (LifetimeForRegisters != ServiceRegistrarLifetime.Transient)
                throw new NotSupportedException();
        }

        private void SetLifetime(IRegistration reg)
        {
            // must cast to IServiceRegistrar for behavior wrappers
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: break;
                case ServiceRegistrarLifetime.Singleton: reg.AsContainerSingleton(); break;
                case ServiceRegistrarLifetime.Request: reg.AsRequestSingleton(); break;
                case ServiceRegistrarLifetime.Thread: reg.AsThreadSingleton(); break;
                case ServiceRegistrarLifetime.Session: reg.AsSessionSingleton(); break;
                case ServiceRegistrarLifetime.Pooled: reg.AsCached(); break;
                default: throw new NotSupportedException();
            }
        }
    }
}
