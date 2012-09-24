using System.Abstract.EventSourcing;
using Contoso.Abstract.EventSourcing;
using Contoso.Abstract.Parts;
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
            var aggregateRoot = new AggregateRootRepository(new MSSqlEventStore("connectionString", new JsonTypeSerializer()), new MSSqlAggregateRootSnapshotStore("connectionString", new JsonTypeSerializer()));
            var myAggregate = aggregateRoot.GetByID<MyAggregate>("ID");
            if (myAggregate.HasChanged)
                aggregateRoot.Save(myAggregate);
        }
    }
}
