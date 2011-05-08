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
using Microsoft.Practices.Unity;
namespace Contoso.Abstract
{
    /// <summary>
    /// IUnityServiceRegistrar
    /// </summary>
    public interface IUnityServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// UnityServiceRegistrar
    /// </summary>
    internal sealed class UnityServiceRegistrar : IUnityServiceRegistrar, IDisposable
    {
        private UnityServiceLocator _parent;
        private IUnityContainer _container;

        public UnityServiceRegistrar(UnityServiceLocator parent, IUnityContainer container)
        {
            _parent = parent;
            _container = container;
        }

        public void Dispose() { }

        // locator
        public IServiceLocator GetLocator() { return _parent; }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // register type
        public void Register(Type serviceType) { _container.RegisterType(serviceType, new InjectionMember[0]); }
        public void Register(Type serviceType, string name) { _container.RegisterType(serviceType, name, new InjectionMember[0]); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { _container.RegisterType<TService, TImplementation>(new InjectionMember[0]); }
        public void Register<TService, TImplementation>(string name)
            where TImplementation : class, TService { _container.RegisterType<TService, TImplementation>(name, new InjectionMember[0]); }
        public void Register<TService>(Type implementationType)
           where TService : class { _container.RegisterType(typeof(TService), implementationType, new InjectionMember[0]); }
        public void Register<TService>(Type implementationType, string name)
           where TService : class { _container.RegisterType(typeof(TService), implementationType, name, new InjectionMember[0]); }
        public void Register(Type serviceType, Type implementationType) { _container.RegisterType(serviceType, implementationType, new InjectionMember[0]); }
        public void Register(Type serviceType, Type implementationType, string name) { _container.RegisterType(serviceType, implementationType, name, new InjectionMember[0]); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { _container.RegisterInstance<TService>(instance); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { _container.RegisterInstance<TService>(name, instance); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { _container.RegisterType<TService>(new InjectionFactory(c => factoryMethod(_parent))); }

        //private string MakeId(Type serviceType, Type implementationType) { return serviceType.Name + "->" + implementationType.FullName; }
    }
}
