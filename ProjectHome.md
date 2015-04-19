note: these documented changes are for the next pending release this month.

# Introduction #

BCL Contrib-Abstract strives to provide standard interfaces or contracts for common services typicality re-implemented in various projects/assemblies. Utilizing a standards based type allows services to flow between otherwise disconnected assemblies.

Coherence is achieved by a reduction of a service's members to only those base enough to be common across multiple service implementations. Implementation specific extensions to the base interfaces enable access to the uncommon methods.

A simple example of this would be the COM implementation of the `IUnknown` interface, containing the `AddRef`, `QueryInterface` and `Release` methods. `IUnknown` provides a common type that all COM systems are aware of, allowing COM services to flow between various COM aware systems. A COM system would then be queried using the `QueryInterface` method to get the specific implementation requested. Of course with .Net, simple casting is all that is necessary to move from a common `IServiceLocator` to a [Unity](Unity.md) specific `IUnityServiceLocator`.

For greater adaptation and ease of use, implementations for some of the more common abstracted systems are included as multiple assemblies to optionally chose from.

Unit tests are held in a separate project: http://code.google.com/p/bclcontrib-tests/

### NuGet ###
_NuGet Packages:_
  * [Abstract](http://nuget.org/List/Packages/BclContrib-Abstract)
  * [Abstract.ApplicationBus](http://nuget.org/List/Packages/BclContrib-Abstract.ApplicationBus)
  * [Abstract.Autofac](http://nuget.org/List/Packages/BclContrib-Abstract.Autofac)
  * [Abstract.CastleWindsor](http://nuget.org/List/Packages/BclContrib-Abstract.CastleWindsor)
  * [Abstract.Hiro](http://nuget.org/List/Packages/BclContrib-Abstract.Hiro)
  * [Abstract.MongoDB](http://nuget.org/List/Packages/BclContrib-Abstract.MongoDB) (future)
  * [Abstract.Msmq](http://nuget.org/List/Packages/BclContrib-Abstract.Msmq) (future)
  * [Abstract.Ninject](http://nuget.org/List/Packages/BclContrib-Abstract.Ninject)
  * [Abstract.NServiceBus](http://nuget.org/List/Packages/BclContrib-Abstract.NServiceBus) (future)
  * [Abstract.RabbitMQ](http://nuget.org/List/Packages/BclContrib-Abstract.RabbitMQ) (future)
  * [Abstract.ServerAppFabric](http://nuget.org/List/Packages/BclContrib-Abstract.ServerAppFabric)
  * [Abstract.SpringNet](http://nuget.org/List/Packages/BclContrib-Abstract.SpringNet) (future)
  * [Abstract.Sql](http://nuget.org/List/Packages/BclContrib-Abstract.Sql)
  * [Abstract.StructureMap](http://nuget.org/List/Packages/BclContrib-Abstract.StructureMap)
  * [Abstract.TypeSerializer](http://nuget.org/List/Packages/BclContrib-Abstract.TypeSerializer)
  * [Abstract.Unity](http://nuget.org/List/Packages/BclContrib-Abstract.Unity)
  * [Abstract.Web](http://nuget.org/List/Packages/BclContrib-Abstract.Web)



### Abstracters for common systems ###
  * Service Locator: [Autofac](Autofac.md), [Ninject](Ninject.md), [Hiro](Hiro.md), [Spring.net](SpringNet.md), [StructureMap](StructureMap.md), [Unity](Unity.md), [Castle Windsor](Windsor.md)
  * Service Bus: [NServiceBus](NServiceBus.md)
  * Service Cache: [Server AppFabric](ServerAppFabric.md) (Velocity)
  * Service Log: none

### Abstracters implemented in `BclContrib-Abstract` ###
  * Event Sourcing: [Microsoft SQL Server](Sql.md), [MongoDB](MongoDB.md)
  * Service Bus: [Application Bus](ApplicationBus.md), [RabbitMQ](RabbitMQ.md), [MSMQ](Msmq.md)
  * Service Cache: Web Cache
  * Serializer: [TypeSerializer](TypeSerializer.md)



# Systems #

Something about these systems.

### Event Sourcing :: _Events as a Storage Mechanism_ ###
_abstracts an event sourcing system http://cqrsinfo.com/documents/events-as-storage-mechanism/._

  * Defining an AggregateRoot
```
// example event
public class MyEvent : Event { }

// example aggregate-root
public class MyAggregate : AggregateRoot
{
   // method exposed to the domain
   public void DoMethod() { ApplyEvent(new MyEvent()); }
   // apply the event
   private void Handle(MyEvent e) { }
}
```
  * Creating an AggregateRoot Repository
```
var aggregateRoot = new AggregateRootRepository(new SqlEventStore("connectionString"), new SqlAggregateRootSnapshotStore("connectionString"));
```
  * Consuming an AggregateRoot Repository
```
var myAggregate = aggregateRoot.GetById<MyAggregate>("ID");
myAggregate.DoMethod();
if (myAggregate.HasChanged)
   aggregateRoot.Save(myAggregate);
```


### Service Bus :: _Enterprise service bus_ ###
_abstracts an enterprise service bus pattern http://en.wikipedia.org/wiki/Enterprise_service_bus._

  * Creating a Service Bus
```
ServiceBusManager.SetProvider(() => new ApplicationServiceBus());
```

  * Consuming a Service Bus
> > from a singleton:
```
// sending from an instance
ServiceBus.Send(new MyService { Value = "Value" });
// sending from a builder
ServiceBus.Send<MyService>(x => { x.Value = "Value"; });
```
> > from an injected dependency:
```
public MyClass(IServiceBus bus)
{
   // sending from an instance
   bus.Send(new MyService { Value = "Value" });
   // sending from a builder
   bus.Send<MyService>(x => { x.Value = "Value"; });
}
```
  * Handling a Service Message
```
public class MyServiceHandler : IServiceMessageHandler<MyService>
{
   public void Handle(MyService message)
   {
      // DO WORK
   }
}
```


### Service Locator :: _Service locator pattern_ ###
_abstracts a service locator pattern http://en.wikipedia.org/wiki/Service_locator_pattern along the lines of Inversion of Control (IOC) http://en.wikipedia.org/wiki/Inversion_of_control._

  * Creating a Service Locator
```
ServiceLocatorManager.SetProvider(() => new UnityServiceLocator());
```

  * Registering a Service
> > from an assembly type scan during creation:
```
ServiceLocatorManager.SetProvider(() => new UnityServiceLocator())
   .RegisterByNamingConvention();
```
> > place registration in a service locator:
```
ServiceLocatorManager.SetProvider(() => new UnityServiceLocator())
   .RegisterByNamingConvention()
   .RegisterWithServiceLocator();
```
> > from an injected dependency:
```
public MyClass(IServiceRegistrar registrar)
{
   // register as a type mapping
   registrar.Register<IMyService, MyService>();
   // register as a single instance
   registrar.RegisterInstance<IMyService>(new MyService { Value = "Value" });
   // register as a delegate
   registrar.Register<IMyService>(locator => new MyService { Value = "Value" });
}
```

  * Consuming a Service Locator
> > from a singleton:
```
var myService = ServiceLocator.Resolve<IMyService>();
```
> > from an injected dependency:
```
public MyClass(IServiceLocator locator)
{
   var myService = locator.Resolve<IMyService>();
}
```


### Service Cache ###
_abstracts cache algorithms implementations http://en.wikipedia.org/wiki/Cache_algorithms._

  * Creating a Service Cache
```
ServiceCacheManager.SetProvider(() => new WebServiceCache());
```

  * Consuming a Service Cache
> > from a singleton:
```
var myService = ServiceCache.Get("MyService");
```
> > from an injected dependency:
```
public MyClass(IServiceCache cache)
{
   var myService = cache.Get("MyService");
}
```


### Service Log ###
_abstracts logging implementations._

  * Creating a Service Log
```
ServiceLogManager.SetProvider(() => new Log4NetServiceLog());
```

  * Consuming a Service Log
> > from a singleton:
```
TBD
```
> > from an injected dependency:
```
public MyClass(IServiceLog log)
{
   TBD
}
```