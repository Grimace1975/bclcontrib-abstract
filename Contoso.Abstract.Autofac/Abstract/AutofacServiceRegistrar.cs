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
using Autofac;
using Autofac.Builder;
using Autofac.Core;
namespace Contoso.Abstract
{
    /// <summary>
    /// IAutofacServiceRegistrar
    /// </summary>
    public interface IAutofacServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// AutofacServiceRegistrar
    /// </summary>
    public class AutofacServiceRegistrar : IAutofacServiceRegistrar, IDisposable
    {
        private AutofacServiceLocator _parent;
        private ContainerBuilder _builder;
        private IContainer _container;

        public AutofacServiceRegistrar(AutofacServiceLocator parent, ContainerBuilder builder, out Func<IContainer> containerBuilder)
        {
            _parent = parent;
            _builder = builder;
            containerBuilder = (() => _container = _builder.Build());
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
        public bool HasRegistered<TService>()
        {
            // parent container will ensure build
            return _parent.Container.IsRegistered<TService>();
        }
        public bool HasRegistered(Type serviceType)
        {
            // parent container will ensure build
            return _parent.Container.IsRegistered(serviceType);
        }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            // parent container will ensure build
            return _parent.Container.ComponentRegistry.Registrations
                .SelectMany(x => x.Services.OfType<TypedService>())
                .Where(x => serviceType.IsAssignableFrom(x.ServiceType))
                .Select(x => new ServiceRegistration { ServiceType = x.ServiceType, ServiceName = x.Description });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                // parent container will ensure build
                return _parent.Container.ComponentRegistry.Registrations
                    .SelectMany(x => x.Services.OfType<TypedService>())
                    .Select(x => new ServiceRegistration { ServiceType = x.ServiceType, ServiceName = x.Description });
            }
        }

        // register type
        public void Register(Type serviceType)
        {
            if (_container == null)
                _builder.RegisterType(serviceType);
            else
                throw new NotSupportedException();
        }
        public void Register(Type serviceType, string name)
        {
            if (_container == null)
                _builder.RegisterType(serviceType).Named(name, serviceType);
            else
                throw new NotSupportedException();
        }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            if (_container == null)
                _builder.RegisterType<TImplementation>().As<TService>();
            else
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType<TImplementation>().As<TService>().CreateRegistration());
        }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService
        {
            Register<TService, TImplementation>();
            if (_container == null)
                _builder.RegisterType<TImplementation>().Named<TService>(name);
            else
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType<TImplementation>().Named<TService>(name).CreateRegistration());
        }
        public void Register<TService>(Type implementationType)
             where TService : class
        {
            if (_container == null)
                _builder.RegisterType(implementationType).As<TService>();
            else
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType(implementationType).As<TService>().CreateRegistration());
        }
        public void Register<TService>(Type implementationType, string name)
             where TService : class
        {
            Register<TService>(implementationType);
            if (_container == null)
                _builder.RegisterType(implementationType).Named<TService>(name);
            else
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType(implementationType).Named<TService>(name).CreateRegistration());
        }
        public void Register(Type serviceType, Type implementationType)
        {
            if (_container == null)
                _builder.RegisterType(implementationType).As(serviceType);
            else
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType(implementationType).As(serviceType).CreateRegistration());
        }
        public void Register(Type serviceType, Type implementationType, string name)
        {
            Register(serviceType, implementationType);
            if (_container == null)
                _builder.RegisterType(implementationType).Named(name, serviceType);
            else
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType(implementationType).Named(name, serviceType).CreateRegistration());
        }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class
        {
            if (_container == null)
                _builder.RegisterInstance(instance).As<TService>();
            else
                throw new NotSupportedException();
        }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class
        {
            if (_container == null)
                _builder.RegisterInstance(instance).Named(name, typeof(TService));
            else
                throw new NotSupportedException();
        }
        public void RegisterInstance(Type serviceType, object instance)
        {
            if (_container == null)
                _builder.RegisterInstance(instance).As(serviceType);
            else
                throw new NotSupportedException();
        }
        public void RegisterInstance(Type serviceType, object instance, string name)
        {
            if (_container == null)
                _builder.RegisterInstance(instance).Named(name, serviceType);
            else
                throw new NotSupportedException();
        }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class
        {
            if (_container == null)
                _builder.Register(x => factoryMethod(_parent)).As<TService>();
            else
                throw new NotSupportedException();
        }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class
        {
            if (_container == null)
                _builder.Register(x => factoryMethod(_parent)).Named<TService>(name);
            else
                throw new NotSupportedException();
        }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod)
        {
            if (_container == null)
                _builder.Register(x => factoryMethod(_parent)).As(serviceType);
            else
                throw new NotSupportedException();
        }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name)
        {
            if (_container == null)
                _builder.Register(x => factoryMethod(_parent)).Named(name, serviceType);
            else
                throw new NotSupportedException();
        }
    }
}
