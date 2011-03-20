using System;
using System.Abstract;
using Contoso.Abstract;
using System.Abstract.EventSourcing;
using Contoso.Abstract.EventSourcing;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceLocatorManager.SetLocatorProvider(() => new UnityServiceLocator())
                .RegisterByIServiceRegistration(typeof(Program).Assembly)
                .RegisterByNamingConvention(typeof(Program).Assembly);
            //
            ServiceBusManager.SetBusProvider(() => new ApplicationServiceBus());
        }
    }

    public interface IMyService { }
    public class MyService : IMyService, IServiceMessage
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

    public class MyEvent : Event { }
    public class MyAggregate : AggregateRoot
    {
        public void DoMethod() { ApplyEvent(new MyEvent()); }
        public void Handle(MyEvent e) { }
    }

    public class MyClass
    {
        public MyClass(IServiceBus bus)
        {
            ServiceBus.Send(new MyService { Value = "Value" });
            bus.Send(new MyService { Value = "Value" });
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
            registrar.Register<IMyService>(new MyService { Value = "Value" });
            // register as a delegate
            registrar.Register<IMyService>(locator => new MyService { Value = "Value" });
        }

        public void Foo()
        {
            var aggregateRoot = new AggregateRootRepository(new SqlEventStore("connectionString"), new SqlAggregateRootSnapshotStore("connectionString"));
            var myAggregate = aggregateRoot.GetById<MyAggregate>("ID");
            if (myAggregate.HasChanged)
                aggregateRoot.Save(myAggregate);
        }
    }
}
