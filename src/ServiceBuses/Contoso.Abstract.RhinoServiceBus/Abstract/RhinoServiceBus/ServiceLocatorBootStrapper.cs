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
using System.Linq;
using System.Reflection;
using Rhino.ServiceBus.Actions;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
namespace Contoso.Abstract.RhinoServiceBus
{
    public abstract class ServiceLocatorBootStrapper : AbstractBootStrapper
    {
        internal System.Abstract.IServiceLocator _locator;

        protected ServiceLocatorBootStrapper() { }
        protected ServiceLocatorBootStrapper(System.Abstract.IServiceLocator locator)
        {
            _locator = locator;
        }

        protected override void ConfigureBusFacility(AbstractRhinoServiceBusConfiguration configuration)
        {
            configuration.UseAbstractServiceLocator(_locator);
            base.ConfigureBusFacility(configuration);
        }

        private void ConfigureConsumers(Assembly assemblyToScan)
        {
            var types = assemblyToScan.GetTypes()
                .Where(type => typeof(IMessageConsumer).IsAssignableFrom(type) &&
                    !typeof(IOccasionalMessageConsumer).IsAssignableFrom(type) &&
                    IsTypeAcceptableForThisBootStrapper(type));
            foreach (var type in types)
                ConfigureConsumer(type);
        }

        protected virtual void ConfigureConsumer(Type type)
        {
            _locator.Registrar.Register<IMessageConsumer>(type, type.FullName);
        }

        protected virtual void ConfigureContainer()
        {
            var registrar = _locator.Registrar;
            registrar.RegisterByTypeMatch<IDeploymentAction>(Assembly);
            registrar.RegisterByTypeMatch<IEnvironmentValidationAction>(Assembly);
            ConfigureConsumers(typeof(Rhino.ServiceBus.IServiceBus).Assembly);
            ConfigureConsumers(Assembly);
        }

        public override void CreateContainer()
        {
            if (_locator == null)
                _locator = ServiceLocatorManager.Current;
            ConfigureContainer();
        }

        public override void Dispose()
        {
            var disposable = (_locator as IDisposable);
            if (disposable != null)
                disposable.Dispose();
        }

        public override void ExecuteDeploymentActions(string user)
        {
            foreach (var action in _locator.ResolveAll<IDeploymentAction>())
                action.Execute(user);
        }

        public override void ExecuteEnvironmentValidationActions()
        {
            foreach (var action in _locator.ResolveAll<IEnvironmentValidationAction>())
                action.Execute();
        }

        public override T GetInstance<T>()
        {
            return (T)_locator.Resolve(typeof(T));
        }

        protected System.Abstract.IServiceLocator Container
        {
            get { return _locator; }
        }
    }
}