using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Config;
using System;
using System.Linq;
using Rhino.ServiceBus.Actions;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.MessageModules;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Msmq;
using Rhino.Queues.Model;
namespace Contoso.Abstract
{
    internal class ServiceBusBuilder : IBusContainerBuilder
    {
        private readonly AbstractRhinoServiceBusConfiguration _config;
        private readonly System.Abstract.IServiceLocator _locator;
        private readonly System.Abstract.IServiceRegistrar _registrar;

        public ServiceBusBuilder(System.Abstract.IServiceLocator locator, AbstractRhinoServiceBusConfiguration config)
        {
            _locator = locator;
            _registrar = locator.Registrar;
            _config = config;
            _config.BuildWith(this);
        }

        public void RegisterBus()
        {
            var config = (RhinoServiceBusConfiguration)_config;
            var registrar = _locator.Registrar;
            registrar.Register<IDeploymentAction, CreateQueuesAction>(Guid.NewGuid().ToString());
            registrar.Register<DefaultServiceBus, DefaultServiceBus>();
            registrar.Register<IStartableServiceBus>(l => new DefaultServiceBus(l.Resolve<IServiceLocator>(), l.Resolve<ITransport>(), l.Resolve<ISubscriptionStorage>(), l.Resolve<IReflection>(), l.ResolveAll<IMessageModule>().ToArray(), config.MessageOwners.ToArray(), l.Resolve<IEndpointRouter>()));
            registrar.Register<IServiceBus, DefaultServiceBus>();
            registrar.Register<IStartable, DefaultServiceBus>();
        }

        public void RegisterDefaultServices()
        {
            var registrar = _locator.Registrar;
            registrar.Register<IServiceLocator, ServiceLocatorAdapter>();
            //_locator.RegisterTypesFromAssembly<IBusConfigurationAware>(typeof(IServiceBus).Assembly);
            foreach (var aware in _locator.ResolveAll<IBusConfigurationAware>())
                aware.Configure(_config, this);
            foreach (var messageModule in _config.MessageModules)
                if (!_locator.Registrar.HasRegistered(messageModule))
                    registrar.Register<IMessageModule>(messageModule, messageModule.FullName);
            registrar.Register<IReflection, DefaultReflection>();
            registrar.Register<IMessageSerializer>(_config.SerializerType);
            registrar.Register<IEndpointRouter, EndpointRouter>();
        }

        public void RegisterLoadBalancerEndpoint(Uri loadBalancerEndpoint)
        {
            //_locator.RegisterType<LoadBalancerMessageModule>(typeof(LoadBalancerMessageModule).FullName, new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new InjectionParameter<Uri>(loadBalancerEndpoint), new ResolvedParameter<IEndpointRouter>() }) });
        }

        public void RegisterLoggingEndpoint(Uri logEndpoint)
        {
            var registrar = _locator.Registrar;
            registrar.Register<MessageLoggingModule, MessageLoggingModule>(typeof(MessageLoggingModule).FullName); //, new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<IEndpointRouter>(), new InjectionParameter<Uri>(logEndpoint) }) });
            registrar.Register<IDeploymentAction, CreateLogQueueAction>(Guid.NewGuid().ToString()); //, new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<MessageLoggingModule>(typeof(MessageLoggingModule).FullName), new ResolvedParameter<ITransport>() }) });
        }

        public void RegisterMsmqOneWay()
        {
            //var config = (OnewayRhinoServiceBusConfiguration)_config;
            var registrar = _locator.Registrar;
            //_locator.RegisterType<IMessageBuilder<Message>, MsmqMessageBuilder>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
            //_locator.RegisterType<IOnewayBus, MsmqOnewayBus>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new InjectionParameter<MessageOwner[]>(config.MessageOwners), new ResolvedParameter<IMessageBuilder<Message>>() }) });
        }

        public void RegisterMsmqTransport(Type queueStrategyType)
        {
            var registrar = _locator.Registrar;
            if (queueStrategyType.Equals(typeof(FlatQueueStrategy)))
                registrar.Register<IQueueStrategy>(l => (IQueueStrategy)Activator.CreateInstance(queueStrategyType, l.Resolve<IEndpointRouter>(), _config.Endpoint));
            else
                registrar.Register<IQueueStrategy>(queueStrategyType);
            //registrar.Register<IMessageBuilder<Message>, MsmqMessageBuilder>();
            //_locator.Register<IMsmqTransportAction, ErrorAction>(Guid.NewGuid().ToString(), new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new InjectionParameter<int>(this._config.NumberOfRetries), new ResolvedParameter<IQueueStrategy>() }) });
            //_locator.Register<ISubscriptionStorage, MsmqSubscriptionStorage>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<IReflection>(), new ResolvedParameter<IMessageSerializer>(), new InjectionParameter<Uri>(this._config.Endpoint), new ResolvedParameter<IEndpointRouter>(), new ResolvedParameter<IQueueStrategy>() }) });
            //_locator.Register<ITransport, MsmqTransport>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<IMessageSerializer>(), new ResolvedParameter<IQueueStrategy>(), new InjectionParameter<Uri>(this._config.Endpoint), new InjectionParameter<int>(this._config.ThreadCount), new ResolvedParameter<IMsmqTransportAction[]>(), new ResolvedParameter<IEndpointRouter>(), new InjectionParameter<IsolationLevel>(this._config.IsolationLevel), new InjectionParameter<TransactionalOptions>(this._config.Transactional), new InjectionParameter<bool>(this._config.ConsumeInTransaction), new ResolvedParameter<IMessageBuilder<Message>>() }) });
            //_locator.RegisterTypesFromAssembly<IMsmqTransportAction>(typeof(IMsmqTransportAction).Assembly, new Type[] { typeof(ErrorAction) });
        }

        public void RegisterNoSecurity()
        {
            //_locator.RegisterType<IValueConvertor<WireEcryptedString>, ThrowingWireEcryptedStringConvertor>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
            //_locator.RegisterType<IElementSerializationBehavior, ThrowingWireEncryptedMessageConvertor>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public void RegisterPrimaryLoadBalancer()
        {
            //var config = (LoadBalancerConfiguration)_config;
            //_locator.RegisterType<MsmqLoadBalancer>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<IMessageSerializer>(), new ResolvedParameter<IQueueStrategy>(), new ResolvedParameter<IEndpointRouter>(), new InjectionParameter<Uri>(config.Endpoint), new InjectionParameter<int>(config.ThreadCount), new InjectionParameter<Uri>(config.SecondaryLoadBalancer), new InjectionParameter<TransactionalOptions>(config.Transactional), new ResolvedParameter<IMessageBuilder<Message>>() }), new InjectionProperty("ReadyForWorkListener") }).RegisterType<IStartable, MsmqLoadBalancer>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
            //_locator.RegisterType<IDeploymentAction, CreateLoadBalancerQueuesAction>(Guid.NewGuid().ToString(), new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public void RegisterQueueCreation()
        {
            //_locator.RegisterType<IServiceBusAware, QueueCreationModule>(typeof(QueueCreationModule).FullName, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public void RegisterReadyForWork()
        {
            //var config = (LoadBalancerConfiguration)_config;
            //_locator.RegisterType<MsmqReadyForWorkListener>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<IQueueStrategy>(), new InjectionParameter<Uri>(config.ReadyForWork), new InjectionParameter<int>(config.ThreadCount), new ResolvedParameter<IMessageSerializer>(), new ResolvedParameter<IEndpointRouter>(), new InjectionParameter<TransactionalOptions>(config.Transactional), new ResolvedParameter<IMessageBuilder<Message>>() }) });
            //_locator.RegisterType<IDeploymentAction, CreateReadyForWorkQueuesAction>(Guid.NewGuid().ToString(), new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public void RegisterRhinoQueuesOneWay()
        {
            //var config = (OnewayRhinoServiceBusConfiguration)_config;
            //var bus = _config.ConfigurationSection.Bus;
            //_locator.RegisterType<IMessageBuilder<MessagePayload>, RhinoQueuesMessageBuilder>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
            //_locator.RegisterType<IOnewayBus, RhinoQueuesOneWayBus>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new InjectionParameter<MessageOwner[]>(config.MessageOwners), new ResolvedParameter<IMessageSerializer>(), new InjectionParameter<string>(bus.QueuePath), new InjectionParameter<bool>(bus.EnablePerformanceCounters), new ResolvedParameter<IMessageBuilder<MessagePayload>>() }) });
        }

        public void RegisterRhinoQueuesTransport()
        {
            //var bus = _config.ConfigurationSection.Bus;
            //_locator.RegisterType<ISubscriptionStorage, PhtSubscriptionStorage>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new InjectionParameter<string>(bus.SubscriptionPath), new ResolvedParameter<IMessageSerializer>(), new ResolvedParameter<IReflection>() }) });
            //_locator.RegisterType<ITransport, RhinoQueuesTransport>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new InjectionParameter<Uri>(this._config.Endpoint), new ResolvedParameter<IEndpointRouter>(), new ResolvedParameter<IMessageSerializer>(), new InjectionParameter<int>(this._config.ThreadCount), new InjectionParameter<string>(bus.QueuePath), new InjectionParameter<IsolationLevel>(this._config.IsolationLevel), new InjectionParameter<int>(this._config.NumberOfRetries), new InjectionParameter<bool>(bus.EnablePerformanceCounters), new ResolvedParameter<IMessageBuilder<MessagePayload>>() }) });
            //_locator.RegisterType<IMessageBuilder<MessagePayload>, RhinoQueuesMessageBuilder>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public void RegisterSecondaryLoadBalancer()
        {
            //var config = (LoadBalancerConfiguration)_config;
            //_locator.RegisterType<MsmqSecondaryLoadBalancer>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<IMessageSerializer>(), new ResolvedParameter<IQueueStrategy>(), new ResolvedParameter<IEndpointRouter>(), new InjectionParameter<Uri>(config.Endpoint), new InjectionParameter<Uri>(config.PrimaryLoadBalancer), new InjectionParameter<int>(config.ThreadCount), new InjectionParameter<TransactionalOptions>(config.Transactional), new ResolvedParameter<IMessageBuilder<Message>>() }) }).RegisterType<IStartable, MsmqSecondaryLoadBalancer>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
            //_locator.RegisterType<IDeploymentAction, CreateLoadBalancerQueuesAction>(Guid.NewGuid().ToString(), new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public void RegisterSecurity(byte[] key)
        {
            //_locator.RegisterType<IEncryptionService, RijndaelEncryptionService>("esb.security", new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new InjectionParameter<byte[]>(key) }) });
            //_locator.RegisterType<IValueConvertor<WireEcryptedString>, WireEcryptedStringConvertor>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<IEncryptionService>("esb.security") }) });
            //_locator.RegisterType<IElementSerializationBehavior, WireEncryptedMessageConvertor>(new ContainerControlledLifetimeManager(), new InjectionMember[] { new InjectionConstructor(new object[] { new ResolvedParameter<IEncryptionService>("esb.security") }) });
        }

        public void WithInterceptor(IConsumerInterceptor interceptor)
        {
            //_locator.AddExtension(new ConsumerExtension(interceptor));
        }
    }
}