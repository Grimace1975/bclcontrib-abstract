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
namespace Contoso.Abstract
{
    /// <summary>
    /// INinjectServiceRegistrar
    /// </summary>
    public interface INinjectServiceRegistrar : IServiceRegistrar { }

    public class NinjectServiceRegistrar : NinjectModule, INinjectServiceRegistrar, IDisposable
    {
        private NinjectServiceLocator _parent;
        private IKernel _container;
        private Guid _moduleId;

        public NinjectServiceRegistrar(NinjectServiceLocator parent, IKernel kernel)
        {
            _parent = parent;
            _container = kernel;
            _container.Load(new INinjectModule[] { this });
        }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

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
        public void Register(Type serviceType) { Bind(serviceType).ToSelf(); }
        public void Register(Type serviceType, string name) { Bind(serviceType).ToSelf().Named(name); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { Bind<TService>().To<TImplementation>(); }
        public void Register<TService, TImplementation>(string name)
            where TImplementation : class, TService { Bind<TService>().To(typeof(TImplementation)).Named(name); }
        public void Register<TService>(Type implementationType)
            where TService : class { Bind<TService>().To(implementationType); }
        public void Register<TService>(Type implementationType, string name)
            where TService : class { Bind<TService>().To(implementationType).Named(name); }
        public void Register(Type serviceType, Type implementationType) { Bind(serviceType).To(implementationType); }
        public void Register(Type serviceType, Type implementationType, string name) { Bind(serviceType).To(implementationType).Named(name); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { Bind<TService>().ToConstant(instance); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { Bind<TService>().ToConstant(instance).Named(name); }
        public void RegisterInstance(Type serviceType, object instance) { Bind(serviceType).ToConstant(instance); }
        public void RegisterInstance(Type serviceType, object instance, string name) { Bind(serviceType).ToConstant(instance).Named(name); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { Bind<TService>().ToMethod(x => factoryMethod(_parent)); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { Bind(serviceType).ToMethod(x => factoryMethod(_parent)); }

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
    }
}
