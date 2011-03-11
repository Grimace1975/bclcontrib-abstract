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

        public AutofacServiceRegistrar(AutofacServiceLocator parent, ContainerBuilder builder, out Action<IContainer> containerSetter)
        {
            _parent = parent;
            _builder = builder;
            containerSetter = (x => _container = x);
        }

        public void Dispose() { }

        // locator
        public IServiceLocator GetLocator() { return _parent; }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService
        {
            if (_container != null)
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType<TImplementation>().As<TService>().CreateRegistration());
            else
                _builder.RegisterType<TImplementation>().As<TService>();
        }
        public void Register<TService, TImplementation>(string id)
            where TImplementation : class, TService
        {
            if (_container != null)
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType<TImplementation>().Named<TService>(id).CreateRegistration());
            else
                _builder.RegisterType<TImplementation>().Named<TService>(id);
        }
        public void Register<TService>(Type implementationType)
             where TService : class
        {
            if (_container != null)
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType(implementationType).As<TService>().CreateRegistration());
            else
                _builder.RegisterType(implementationType).As<TService>();
        }
        public void Register<TService>(Type implementationType, string id)
             where TService : class
        {
            if (_container != null)
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType(implementationType).Named<TService>(id).CreateRegistration());
            else
                _builder.RegisterType(implementationType).Named<TService>(id);
        }
        public void Register(Type serviceType, Type implementationType)
        {
            if (_container != null)
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType(implementationType).As(serviceType).CreateRegistration());
            else
                _builder.RegisterType(implementationType).As(serviceType);
        }
        public void Register(Type serviceType, Type implementationType, string id)
        {
            if (_container != null)
                _container.ComponentRegistry.Register(RegistrationBuilder.ForType(implementationType).Named(id, serviceType).CreateRegistration());
            else
                _builder.RegisterType(implementationType).Named(id, serviceType);
        }

        // register id
        public void Register(Type serviceType, string id)
        {
            if (_container != null)
                throw new NotSupportedException();
            else
                _builder.RegisterType(serviceType).Named(id, serviceType);
        }

        // register instance
        public void Register<TService>(TService instance)
            where TService : class
        {
            if (_container != null)
                throw new NotSupportedException();
            else
                _builder.RegisterInstance(instance);
        }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class
        {
            if (_container != null)
                throw new NotSupportedException();
            else
                _builder.Register(x => factoryMethod(_parent));
        }
    }
}
