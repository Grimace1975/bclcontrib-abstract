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
using Ninject;
using Ninject.Modules;
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
        public IServiceLocator GetLocator() { return _parent; }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { Bind<TService>().To<TImplementation>(); }
        public void Register<TService, TImplementation>(string id)
            where TImplementation : class, TService { Bind<TService>().To(typeof(TImplementation)).Named(id); }
        public void Register<TService>(Type implementationType)
            where TService : class { Bind<TService>().To(implementationType); }
        public void Register<TService>(Type implementationType, string id)
            where TService : class { Bind<TService>().To(implementationType).Named(id); }
        public void Register(Type serviceType, Type implementationType) { Bind(serviceType).To(implementationType); }
        public void Register(Type serviceType, Type implementationType, string id) { Bind(serviceType).To(implementationType).Named(id); }

        // register id
        public void Register(Type serviceType, string id) { Bind(serviceType).ToSelf().Named(id); }

        // register instance
        public void Register<TService>(TService instance)
            where TService : class { Bind<TService>().ToConstant(instance); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { Bind<TService>().ToMethod(x => factoryMethod(_parent)); }

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
