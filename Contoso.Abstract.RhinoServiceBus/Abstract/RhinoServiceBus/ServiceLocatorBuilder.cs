using System;
using System.Linq;
using System.Messaging;
using Rhino.Queues;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Actions;
using Rhino.ServiceBus.Config;
using Rhino.ServiceBus.Convertors;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.LoadBalancer;
using Rhino.ServiceBus.MessageModules;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Msmq.TransportActions;
using Rhino.ServiceBus.RhinoQueues;
using ErrorAction = Rhino.ServiceBus.Msmq.TransportActions.ErrorAction;
namespace Contoso.Abstract.RhinoServiceBus
{
    internal class ServiceLocatorBuilder : IBusContainerBuilder
    {
        private readonly AbstractRhinoServiceBusConfiguration _config;
        private readonly System.Abstract.IServiceLocator _locator;
        private readonly System.Abstract.IServiceRegistrar _registrar;

        public ServiceLocatorBuilder(System.Abstract.IServiceLocator locator, AbstractRhinoServiceBusConfiguration config)
        {
            _locator = locator;
            _registrar = locator.Registrar;
            _config = config;
            _config.BuildWith(this);
        }

        public void RegisterBus()
        {
            var config = (RhinoServiceBusConfiguration)_config;
            _registrar.Register<IDeploymentAction, CreateQueuesAction>(Guid.NewGuid().ToString());
            _registrar.Register<IStartableServiceBus>(l => new DefaultServiceBus(l.Resolve<IServiceLocator>(), l.Resolve<ITransport>(), l.Resolve<ISubscriptionStorage>(), l.Resolve<IReflection>(), l.ResolveAll<IMessageModule>().ToArray(), config.MessageOwners.ToArray(), l.Resolve<IEndpointRouter>()));
            //_registrar.Register<IStartable, IStartableServiceBus>();
            //_registrar.Register<IServiceBus, IStartableServiceBus>();
        }

        public void RegisterDefaultServices()
        {
            _registrar.Register<IServiceLocator, ServiceLocatorAdapter>();
            System.Abstract.ServiceLocatorExtensions.RegisterByTypeMatch<IBusConfigurationAware>(_registrar, typeof(IServiceBus).Assembly);
            foreach (var aware in _locator.ResolveAll<IBusConfigurationAware>())
                aware.Configure(_config, this);
            foreach (var messageModule in _config.MessageModules)
                if (!_registrar.HasRegistered(messageModule))
                    _registrar.Register<IMessageModule>(messageModule, messageModule.FullName);
            _registrar.Register<IReflection, DefaultReflection>();
            _registrar.Register<IMessageSerializer>(_config.SerializerType);
            _registrar.Register<IEndpointRouter, EndpointRouter>();
        }

        public void RegisterLoadBalancerEndpoint(Uri loadBalancerEndpoint)
        {
            _registrar.Register<LoadBalancerMessageModule>(l => new LoadBalancerMessageModule(loadBalancerEndpoint, l.Resolve<IEndpointRouter>()));
        }

        public void RegisterLoggingEndpoint(Uri logEndpoint)
        {
            _registrar.Register<MessageLoggingModule>(l => new MessageLoggingModule(l.Resolve<IEndpointRouter>(), logEndpoint));
            _registrar.Register<IDeploymentAction, CreateLogQueueAction>(Guid.NewGuid().ToString());
        }

        public void RegisterMsmqOneWay()
        {
            var config = (OnewayRhinoServiceBusConfiguration)_config;
            _registrar.Register<IMessageBuilder<Message>, MsmqMessageBuilder>();
            _registrar.Register<IOnewayBus>(l => new MsmqOnewayBus(config.MessageOwners, l.Resolve<IMessageBuilder<Message>>()));
        }

        public void RegisterMsmqTransport(Type queueStrategyType)
        {
            if (queueStrategyType.Equals(typeof(FlatQueueStrategy)))
                _registrar.Register<IQueueStrategy>(l => (IQueueStrategy)Activator.CreateInstance(queueStrategyType, l.Resolve<IEndpointRouter>(), _config.Endpoint));
            else
                _registrar.Register<IQueueStrategy>(queueStrategyType);
            _registrar.Register<IMessageBuilder<Message>, MsmqMessageBuilder>();
            _registrar.Register<IMsmqTransportAction>(l => new ErrorAction(_config.NumberOfRetries, l.Resolve<IQueueStrategy>()), Guid.NewGuid().ToString());
            _registrar.Register<ISubscriptionStorage>(l => new MsmqSubscriptionStorage(l.Resolve<IReflection>(), l.Resolve<IMessageSerializer>(), _config.Endpoint, l.Resolve<IEndpointRouter>(), l.Resolve<IQueueStrategy>()));
            _registrar.Register<ITransport>(l => new MsmqTransport(l.Resolve<IMessageSerializer>(), l.Resolve<IQueueStrategy>(), _config.Endpoint, _config.ThreadCount, l.Resolve<IMsmqTransportAction[]>(), l.Resolve<IEndpointRouter>(), _config.IsolationLevel, _config.Transactional, _config.ConsumeInTransaction, l.Resolve<IMessageBuilder<Message>>()));
            var exclude = typeof(ErrorAction);
            System.Abstract.ServiceLocatorExtensions.RegisterByTypeMatch<IMsmqTransportAction>(_registrar, t => t != exclude, typeof(IMsmqTransportAction).Assembly);
        }

        public void RegisterNoSecurity()
        {
            _registrar.Register<IValueConvertor<WireEcryptedString>, ThrowingWireEcryptedStringConvertor>();
            _registrar.Register<IElementSerializationBehavior, ThrowingWireEncryptedMessageConvertor>();
        }

        public void RegisterPrimaryLoadBalancer()
        {
            var config = (Rhino.ServiceBus.LoadBalancer.LoadBalancerConfiguration)_config;
            _registrar.Register<MsmqLoadBalancer>(l => new MsmqLoadBalancer(l.Resolve<IMessageSerializer>(), l.Resolve<IQueueStrategy>(), l.Resolve<IEndpointRouter>(), config.Endpoint, config.ThreadCount, config.SecondaryLoadBalancer, config.Transactional, l.Resolve<IMessageBuilder<Message>>()) { ReadyForWorkListener = l.Resolve<MsmqReadyForWorkListener>() });
            //_registrar.Register<IStartable, MsmqLoadBalancer>();
            _registrar.Register<IDeploymentAction, CreateLoadBalancerQueuesAction>(Guid.NewGuid().ToString());
        }

        public void RegisterQueueCreation()
        {
            _registrar.Register<IServiceBusAware, QueueCreationModule>(Guid.NewGuid().ToString());
        }

        public void RegisterReadyForWork()
        {
            var config = (Rhino.ServiceBus.LoadBalancer.LoadBalancerConfiguration)_config;
            _registrar.Register<MsmqReadyForWorkListener>(l => new MsmqReadyForWorkListener(l.Resolve<IQueueStrategy>(), config.ReadyForWork, config.ThreadCount, l.Resolve<IMessageSerializer>(), l.Resolve<IEndpointRouter>(), config.Transactional, l.Resolve<IMessageBuilder<Message>>()));
            _registrar.Register<IDeploymentAction, CreateReadyForWorkQueuesAction>(Guid.NewGuid().ToString());
        }

        public void RegisterRhinoQueuesOneWay()
        {
            var config = (OnewayRhinoServiceBusConfiguration)_config;
            var bus = _config.ConfigurationSection.Bus;
            _registrar.Register<IMessageBuilder<MessagePayload>, RhinoQueuesMessageBuilder>();
            _registrar.Register<IOnewayBus>(l => new RhinoQueuesOneWayBus(config.MessageOwners, l.Resolve<IMessageSerializer>(), bus.QueuePath, bus.EnablePerformanceCounters, l.Resolve<IMessageBuilder<MessagePayload>>()));
        }

        public void RegisterRhinoQueuesTransport()
        {
            var bus = _config.ConfigurationSection.Bus;
            _registrar.Register<ISubscriptionStorage>(l => new PhtSubscriptionStorage(bus.SubscriptionPath, l.Resolve<IMessageSerializer>(), l.Resolve<IReflection>()));
            _registrar.Register<ITransport>(l => new RhinoQueuesTransport(_config.Endpoint, l.Resolve<IEndpointRouter>(), l.Resolve<IMessageSerializer>(), _config.ThreadCount, bus.QueuePath, _config.IsolationLevel, _config.NumberOfRetries, bus.EnablePerformanceCounters, l.Resolve<IMessageBuilder<MessagePayload>>()));
            _registrar.Register<IMessageBuilder<MessagePayload>, RhinoQueuesMessageBuilder>();
        }

        public void RegisterSecondaryLoadBalancer()
        {
            var config = (Rhino.ServiceBus.LoadBalancer.LoadBalancerConfiguration)_config;
            _registrar.Register<MsmqSecondaryLoadBalancer>(l => new MsmqSecondaryLoadBalancer(l.Resolve<IMessageSerializer>(), l.Resolve<IQueueStrategy>(), l.Resolve<IEndpointRouter>(), config.Endpoint, config.PrimaryLoadBalancer, config.ThreadCount, config.Transactional, l.Resolve<IMessageBuilder<Message>>()));
            //_registrar.Register<IStartable, MsmqSecondaryLoadBalancer>();
            _registrar.Register<IDeploymentAction, CreateLoadBalancerQueuesAction>(Guid.NewGuid().ToString());
        }

        public void RegisterSecurity(byte[] key)
        {
            _registrar.Register<IEncryptionService>(l => new RijndaelEncryptionService(key), "esb.security");
            _registrar.Register<IValueConvertor<WireEcryptedString>>(l => new WireEcryptedStringConvertor(l.Resolve<IEncryptionService>("esb.security")));
            _registrar.Register<IElementSerializationBehavior>(l => new WireEncryptedMessageConvertor(l.Resolve<IEncryptionService>("esb.security")));
        }

        public void WithInterceptor(IConsumerInterceptor interceptor)
        {
            _registrar.RegisterInterceptor(new ConsumerInterceptorAdapter(interceptor));
        }
    }
}