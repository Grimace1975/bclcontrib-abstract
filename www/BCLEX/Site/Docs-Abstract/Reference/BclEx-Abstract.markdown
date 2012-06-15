#summary System Abstract Interfaces and Bases.

#summary Implementation of an Application Bus.

== ApplicationBus ==
_this is a simple in memory service bus for interconnecting separate services within the same process._<br />
The service messages, service handlers, and registration of an Application-bus service bus, must be done before consuming its services.
   setup the message:
   {{{
// a simple poco object representing the information to be transferred
public class MyMessage : IServiceMessage
{
   public string Value { get; set; }
}
   }}}
   setup the message handler:
   {{{
public class MyServiceHandler : IApplicationServiceMessageHandler<MyMessage>
{
   // handle incoming message defined above
   public void Handle(MyMessage message)
   {
      Console.Write(message.Value);
   }
}
   }}}
   creating the ApplicationBus service bus:
   {{{
// set service bus provider
ServiceBusManager.SetProvider(() => new ApplicationServiceBus()
   .Add<MyServiceHandler>()
   .Add<...>()
   .Add<...>()
);
   }}}


Consuming a service bus
   from a singleton:
   {{{
// sending from an instance
ServiceBus.Send(new MyMessage { Value = "Value" });
// sending from a builder
ServiceBus.Send<MyMessage>(x => { x.Value = "Value"; });
   }}}
   from an injected dependency:
   {{{
public MyClass(IServiceBus bus)
{
   // sending from an instance
   bus.Send(new MyMessage { Value = "Value" });
   // sending from a builder
   bus.Send<MyMessage>(x => { x.Value = "Value"; });
}
   }}}

== Extended Methods ==
IApplicationServiceBus extended methods:
|| {{{IApplicationServiceBus Add<TMessageHandler>()}}} || Registers a Message-handler for the application bus. ||
|| {{{IApplicationServiceBus Add(Type messageHandlerType)}}} || Registers a Message-handler for the application bus. ||