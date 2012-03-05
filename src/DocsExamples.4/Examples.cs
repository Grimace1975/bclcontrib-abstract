using System;
using System.Abstract;
using Contoso.Abstract;
using System.Abstract.EventSourcing;
using Contoso.Abstract.EventSourcing;
namespace Example
{
    //class Examples
    //{
    //    private void Main()
    //    {
    //        ServiceLocatorManager.SetProvider(() => new UnityServiceLocator())
    //            .RegisterByIServiceRegistration(typeof(Program).Assembly)
    //            .RegisterByNamingConvention(typeof(Program).Assembly);
    //        //
    //        ServiceBusManager.SetProvider(() => new AppServiceBus())
    //            .RegisterWithServiceLocator();
    //    }
    //}

    public interface IMyService { }
    public class MyService : IMyService
    {
        public string Value { get; set; }
    }

    public class MyServiceHandler : IServiceMessageHandler<MyService>
    {
        public void Handle(MyService message)
        {
            // DO WORK
        }
    }

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

    public class MyClass
    {
        public MyClass(IServiceBus bus)
        {
            // sending from an instance
            ServiceBus.Send(new MyService { Value = "Value" });
            // sending from a builder
            ServiceBus.Send<MyService>(x => { x.Value = "Value"; });

            // sending from an instance
            bus.Send(new MyService { Value = "Value" });
            // sending from a builder
            bus.Send<MyService>(x => { x.Value = "Value"; });
        }

        public MyClass(IServiceLocator locator)
        {
            var myService = locator.Resolve<IMyService>();
        }

        public MyClass(IServiceRegistrar registrar)
        {
            // register as a type mapping
            registrar.Register<IMyService, MyService>();
            // register as a single instance
            registrar.RegisterInstance<IMyService>(new MyService { Value = "Value" });
            // register as a delegate
            registrar.Register<IMyService>(locator => new MyService { Value = "Value" });
        }

        //public void Foo()
        //{
        //    var aggregateRoot = new AggregateRootRepository(new MSSqlEventStore("connectionString"), new MSSqlAggregateRootSnapshotStore("connectionString"));
        //    var myAggregate = aggregateRoot.GetById<MyAggregate>("ID");
        //    if (myAggregate.HasChanged)
        //        aggregateRoot.Save(myAggregate);
        //}
    }
}
