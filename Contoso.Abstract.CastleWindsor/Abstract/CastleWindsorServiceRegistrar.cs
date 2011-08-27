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
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // enumerate
        public bool HasRegistered<TService>() { return HasRegistered(typeof(TService)); }
        public bool HasRegistered(Type serviceType) { return _container.Kernel.HasComponent(serviceType); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.Kernel.GetAssignableHandlers(serviceType)
                .Select(x => new ServiceRegistration { ServiceType = x.Service });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get { throw new NotSupportedException(); }
        }

        // register type
        public void Register(Type serviceType) { _container.Register(Component.For(serviceType).LifeStyle.Transient); }
        public void Register(Type serviceType, string name) { _container.Register(Component.For(serviceType).Named(name).LifeStyle.Transient); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { _container.Register(Component.For<TService>().ImplementedBy<TImplementation>().LifeStyle.Transient); }
        public void Register<TService, TImplementation>(string name)
            where TImplementation : class, TService { _container.Register(Component.For<TService>().Named(name).ImplementedBy<TImplementation>().LifeStyle.Transient); }
        public void Register<TService>(Type implementationType)
             where TService : class { _container.Register(Component.For<TService>().ImplementedBy(implementationType).LifeStyle.Transient); }
        public void Register<TService>(Type implementationType, string name)
             where TService : class { _container.Register(Component.For<TService>().Named(name).ImplementedBy(implementationType).LifeStyle.Transient); }
        public void Register(Type serviceType, Type implementationType) { _container.Register(Component.For(serviceType).ImplementedBy(implementationType).LifeStyle.Transient); }
        public void Register(Type serviceType, Type implementationType, string name) { _container.Register(Component.For(serviceType).Named(name).ImplementedBy(implementationType).LifeStyle.Transient); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { _container.Register(Component.For<TService>().Instance(instance)); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { _container.Register(Component.For<TService>().Named(name).Instance(instance)); }
        public void RegisterInstance(Type serviceType, object instance) { _container.Register(Component.For(serviceType).Instance(instance)); }
        public void RegisterInstance(Type serviceType, object instance, string name) { _container.Register(Component.For(serviceType).Named(name).Instance(instance)); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { _container.Register(Component.For<TService>().UsingFactoryMethod<TService>(x => factoryMethod(_parent)).LifeStyle.Transient); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { _container.Register(Component.For(serviceType).UsingFactoryMethod(x => factoryMethod(_parent)).LifeStyle.Transient); }

        #region Domain specific

        //public void RegisterAll<TService>() { AllTypes.Of<TService>(); }
        //private static string MakeId(Type serviceType, Type implementationType) { return serviceType.Name + "->" + implementationType.FullName; }

        #endregion
    }
}
