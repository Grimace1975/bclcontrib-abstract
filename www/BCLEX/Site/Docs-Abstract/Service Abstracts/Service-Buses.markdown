# Service Buses
> abstracts an [enterprise service bus](http://en.wikipedia.org/wiki/Enterprise_service_bus "Enterprise service bus") pattern. this messaging architecture is implemented by various providers.




# Defining the Provider

## Registering a Service Bus

as a singleton:
   
	ServiceBusManager.SetProvider(() => new AppServiceBus());

## Consuming a Service Bus

from a singleton:

	// sending from an instance
	ServiceBus.Send(new MyService { Value = "Value" });
	// sending from a builder
	ServiceBus.Send<MyService>(x => { x.Value = "Value"; });

from an injected dependency:

	public MyClass(IServiceBus bus)
	{
	   // sending from an instance
	   bus.Send(new MyService { Value = "Value" });
	   // sending from a builder
	   bus.Send<MyService>(x => { x.Value = "Value"; });
	}

## Handling a Service Message

	public class MyServiceHandler : IServiceMessageHandler<MyService>
	{
	   public void Handle(MyService message)
	   {
		  // DO WORK
	   }
	}




# Working with IServiceBus