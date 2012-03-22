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
using Contoso.Abstract.SPG2010;
using SPIServiceLocator = Microsoft.Practices.ServiceLocation.IServiceLocator;
using InstantiationType = Microsoft.Practices.SharePoint.Common.ServiceLocation.InstantiationType;
using System.Reflection;
namespace Contoso.Abstract
{
    /// <summary>
    /// ISPServiceRegistrar
    /// </summary>
    public interface ISPServiceRegistrar : IServiceRegistrar
    {
        IServiceLocatorConfig Config { get; }
        void Remove<TService>();
        void Remove<TService>(string name);
    }

    /// <summary>
    /// SPServiceRegistrar
    /// </summary>
    public class SPServiceRegistrar : ISPServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private SPServiceLocator _parent;
        private SPIServiceLocator _container;
        private ServiceLocatorConfig _registrar;

        public SPServiceRegistrar(SPServiceLocator parent, SPIServiceLocator container)
        {
            _parent = parent;
            _container = container;
            _registrar = (_container.GetInstance<IServiceLocatorConfig>() as ServiceLocatorConfig);
            if (_registrar == null)
                throw new NullReferenceException("registrar");
            LifetimeForRegisters = ServiceRegistrarLifetime.Transient;
        }

        public void Dispose() { }
        object ICloneable.Clone() { return MemberwiseClone(); }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { throw new NotSupportedException(); }
        public bool HasRegistered(Type serviceType) { throw new NotSupportedException(); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return Registrations
                .Where(x => serviceType.IsAssignableFrom(x.ServiceType));
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _registrar.GetTypeMappings()
                    .Select(x => new ServiceRegistration
                    {
                        ServiceType = Assembly.Load(x.FromAssembly).GetType(x.FromType),
                        ImplementationType = Assembly.Load(x.ToAssembly).GetType(x.ToAssembly),
                        Name = x.Key
                    });
            }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters { get; private set; }
        public void Register(Type serviceType) { throw new NotSupportedException(); }
        public void Register(Type serviceType, string name) { throw new NotSupportedException(); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { _registrar.RegisterTypeMapping<TService, TImplementation>(null, GetLifestyle()); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { _registrar.RegisterTypeMapping<TService, TImplementation>(name, GetLifestyle()); }
        public void Register<TService>(Type implementationType)
             where TService : class { throw new NotSupportedException(); }
        public void Register<TService>(Type implementationType, string name)
             where TService : class { throw new NotSupportedException(); }
        public void Register(Type serviceType, Type implementationType) { throw new NotSupportedException(); }
        public void Register(Type serviceType, Type implementationType, string name) { throw new NotSupportedException(); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { throw new NotSupportedException(); }
        public void RegisterInstance(Type serviceType, object instance) { throw new NotSupportedException(); }
        public void RegisterInstance(Type serviceType, object instance, string name) { throw new NotSupportedException(); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { throw new NotSupportedException(); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { throw new NotSupportedException(); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { throw new NotSupportedException(); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { throw new NotSupportedException(); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor) { throw new NotSupportedException(); }

        #region Behavior

        bool IServiceRegistrarBehaviorAccessor.RegisterInLocator
        {
            get { return false; }
        }

        ServiceRegistrarLifetime IServiceRegistrarBehaviorAccessor.Lifetime
        {
            get { return LifetimeForRegisters; }
            set { LifetimeForRegisters = value; }
        }

        #endregion

        #region Domain specific

        public IServiceLocatorConfig Config
        {
            get { return _registrar; }
        }

        public void Remove<TService>() { _registrar.RemoveTypeMappings<TService>(); }
        public void Remove<TService>(string name) { _registrar.RemoveTypeMapping<TService>(name); }

        #endregion

        private InstantiationType GetLifestyle()
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return InstantiationType.NewInstanceForEachRequest;
                case ServiceRegistrarLifetime.Singleton: return InstantiationType.AsSingleton;
                default: throw new NotSupportedException();
            }
        }
    }
}
