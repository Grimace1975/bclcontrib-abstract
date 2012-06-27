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
using Ninject;
using Ninject.Modules;
using System.Collections.Generic;
using Ninject.Syntax;
using Ninject.Infrastructure;
namespace Contoso.Abstract
{
    /// <summary>
    /// INinjectServiceRegistrar
    /// </summary>
    public interface INinjectServiceRegistrar : IServiceRegistrar { }

    public class NinjectServiceRegistrar : NinjectModule, INinjectServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private NinjectServiceLocator _parent;
        private IKernel _container;
        private Guid _moduleId;

        public NinjectServiceRegistrar(NinjectServiceLocator parent, IKernel kernel)
        {
            _parent = parent;
            _container = kernel;
            _container.Load(new INinjectModule[] { this });
            LifetimeForRegisters = ServiceRegistrarLifetime.Transient;
        }

        object ICloneable.Clone() { return MemberwiseClone(); }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return Kernel.GetBindings(typeof(TService)).Any(); }
        public bool HasRegistered(Type serviceType) { return Kernel.GetBindings(serviceType).Any(); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return Kernel.GetBindings(serviceType)
                .Select(x => new ServiceRegistration { ServiceType = x.Service });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return Bindings
                    .Select(x => new ServiceRegistration { ServiceType = x.Service });
            }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters { get; private set; }
        public void Register(Type serviceType) { Bind(serviceType).ToSelf(); }
        public void Register(Type serviceType, string name) { Bind(serviceType).ToSelf().Named(name); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { SetLifetime(Bind<TService>().To<TImplementation>()); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { SetLifetime(Bind<TService>().To(typeof(TImplementation))).Named(name); }

        public void Register<TService>(Type implementationType)
            where TService : class { SetLifetime(Bind<TService>().To(implementationType)); }
        public void Register<TService>(Type implementationType, string name)
            where TService : class { SetLifetime(Bind<TService>().To(implementationType)).Named(name); }
        public void Register(Type serviceType, Type implementationType) { SetLifetime(Bind(serviceType).To(implementationType)); }
        public void Register(Type serviceType, Type implementationType, string name) { SetLifetime(Bind(serviceType).To(implementationType)).Named(name); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { EnsureTransientLifestyle(); Bind<TService>().ToConstant(instance); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { EnsureTransientLifestyle(); Bind<TService>().ToConstant(instance).Named(name); }
        public void RegisterInstance(Type serviceType, object instance) { EnsureTransientLifestyle(); Bind(serviceType).ToConstant(instance); }
        public void RegisterInstance(Type serviceType, object instance, string name) { EnsureTransientLifestyle(); Bind(serviceType).ToConstant(instance).Named(name); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { SetLifetime(Bind<TService>().ToMethod(x => factoryMethod(_parent))); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { SetLifetime(Bind<TService>().ToMethod(x => factoryMethod(_parent))).Named(name); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { SetLifetime(Bind(serviceType).ToMethod(x => factoryMethod(_parent))); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { SetLifetime(Bind(serviceType).ToMethod(x => factoryMethod(_parent))).Named(name); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            throw new NotSupportedException();
        }

        #region Domain specific

        public override void Load()
        {
            _moduleId = Guid.NewGuid();
        }

        public override string Name
        {
            get { return _moduleId.ToString(); }
        }

        //private string MakeId(Type serviceType, Type implementationType) { return serviceType.Name + "->" + implementationType.FullName; }

        #endregion

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

        private IBindingNamedWithOrOnSyntax<TService> SetLifetime<TService>(IBindingWhenInNamedWithOrOnSyntax<TService> bindingWhenInNamedWithOrOnSyntax)
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return bindingWhenInNamedWithOrOnSyntax.InScope(StandardScopeCallbacks.Transient);
                case ServiceRegistrarLifetime.Singleton: return bindingWhenInNamedWithOrOnSyntax.InScope(StandardScopeCallbacks.Singleton);
                case ServiceRegistrarLifetime.Thread: return bindingWhenInNamedWithOrOnSyntax.InScope(StandardScopeCallbacks.Thread);
                default: throw new NotSupportedException();
            }
        }
    }
}
