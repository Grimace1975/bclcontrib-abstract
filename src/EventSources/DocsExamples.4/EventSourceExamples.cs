using System.Abstract.EventSourcing;
using Contoso.Abstract.EventSourcing;
namespace Example
{
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

    class EventSourceExamples
    {
        private void Main()
        {
            var aggregateRoot = new AggregateRootRepository(new MSSqlEventStore("connectionString"), new MSSqlAggregateRootSnapshotStore("connectionString"));
            var myAggregate = aggregateRoot.GetByID<MyAggregate>("ID");
            if (myAggregate.HasChanged)
                aggregateRoot.Save(myAggregate);
        }
    }
}
