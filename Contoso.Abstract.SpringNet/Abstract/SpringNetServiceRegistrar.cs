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
using Spring.Objects.Factory;
using Spring.Context;
using Spring.Objects.Factory.Support;
using Spring.Context.Support;
namespace Contoso.Abstract
{
    /// <summary>
    /// ISpringNetServiceRegistrar
    /// </summary>
    public interface ISpringNetServiceRegistrar : IServiceRegistrar { }

    public class SpringNetServiceRegistrar : ISpringNetServiceRegistrar
    {
        private SpringNetServiceLocator _parent;
        private GenericApplicationContext _container;
        private IObjectDefinitionFactory _factory = new DefaultObjectDefinitionFactory();

        public SpringNetServiceRegistrar(SpringNetServiceLocator parent, GenericApplicationContext container)
        {
            _parent = parent;
            _container = container;
        }

        // locator
        public IServiceLocator GetLocator() { return _parent; }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // register type
        public void Register(Type serviceType)
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, serviceType);
            _container.RegisterObjectDefinition(SpringNetServiceLocator.GetName(serviceType), b.ObjectDefinition);
        }
        public void Register(Type serviceType, string name)
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, serviceType);
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, typeof(TImplementation));
            _container.RegisterObjectDefinition(SpringNetServiceLocator.GetName(typeof(TImplementation)), b.ObjectDefinition);
        }
        public void Register<TService, TImplementation>(string name)
            where TImplementation : class, TService
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, typeof(TImplementation));
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }
        public void Register<TService>(Type implementationType)
            where TService : class
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType);
            _container.RegisterObjectDefinition(SpringNetServiceLocator.GetName(implementationType), b.ObjectDefinition);
        }
        public void Register<TService>(Type implementationType, string name)
            where TService : class
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType);
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }
        public void Register(Type serviceType, Type implementationType)
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType);
            _container.RegisterObjectDefinition(SpringNetServiceLocator.GetName(implementationType), b.ObjectDefinition);
        }
        public void Register(Type serviceType, Type implementationType, string name)
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType);
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { _container.ObjectFactory.RegisterSingleton(SpringNetServiceLocator.GetName(typeof(TService)), instance); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { _container.ObjectFactory.RegisterSingleton(name, instance); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class
        {
            throw new NotSupportedException();
            //_container.ObjectFactory.RegisterSingleton(SpringNetServiceLocator.GetName(factoryMethod.GetType()), ((object x) => factoryMethod(_parent)));
        }
    }
}
