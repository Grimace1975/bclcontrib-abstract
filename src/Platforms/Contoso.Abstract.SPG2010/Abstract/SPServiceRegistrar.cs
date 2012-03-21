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
using System.Abstract;
using System.Collections.Generic;
using Contoso.Abstract.SPG2010;
using SPIServiceLocator = Microsoft.Practices.ServiceLocation.IServiceLocator;
namespace Contoso.Abstract
{
    /// <summary>
    /// ISPServiceRegistrar
    /// </summary>
    public interface ISPServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// SPServiceRegistrar
    /// </summary>
    public class SPServiceRegistrar : ISPServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private SPServiceLocator _parent;
        private SPIServiceLocator _container;
        private IServiceLocatorConfig _registrar;

        public SPServiceRegistrar(SPServiceLocator parent, SPIServiceLocator container)
        {
            _parent = parent;
            _container = container;
            _registrar = _container.GetInstance<IServiceLocatorConfig>();
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
            throw new NotSupportedException();
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get { throw new NotSupportedException(); }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters { get; private set; }
        public void Register(Type serviceType)
        {
            throw new NotSupportedException();
        }
        public void Register(Type serviceType, string name)
        {
            throw new NotSupportedException();
        }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _registrar.RegisterTypeMapping<TService, TImplementation>();
        }

        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService
        {
            _registrar.RegisterTypeMapping<TService, TImplementation>(name);
        }
        public void Register<TService>(Type implementationType)
             where TService : class
        {
            throw new NotSupportedException();
        }
        public void Register<TService>(Type implementationType, string name)
             where TService : class
        {
            throw new NotSupportedException();
        }
        public void Register(Type serviceType, Type implementationType)
        {
            throw new NotSupportedException();
        }
        public void Register(Type serviceType, Type implementationType, string name)
        {
            throw new NotSupportedException();
        }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class
        {
            throw new NotSupportedException();
        }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class
        {
            throw new NotSupportedException();
        }
        public void RegisterInstance(Type serviceType, object instance)
        {
            throw new NotSupportedException();
        }
        public void RegisterInstance(Type serviceType, object instance, string name)
        {
            throw new NotSupportedException();
        }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class
        {
            throw new NotSupportedException();
        }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class
        {
            throw new NotSupportedException();

        }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod)
        {
            throw new NotSupportedException();
        }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name)
        {
            throw new NotSupportedException();
        }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            throw new NotSupportedException();
        }

        #region Behavior

        ServiceRegistrarLifetime IServiceRegistrarBehaviorAccessor.Lifetime
        {
            get { return LifetimeForRegisters; }
            set { LifetimeForRegisters = value; }
        }

        #endregion
    }
}
