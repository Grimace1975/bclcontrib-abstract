using System.Abstract.EventSourcing;
using Contoso.Abstract.EventSourcing;
namespace Example.MongoDB
{
    public class ConstructorExamples
    {
        public static void Constructor()
        {
            var aggregateRoot = new AggregateRootRepository(new MongoDBEventStore("server=127.0.0.1;database=test"), new MongoDBAggregateRootSnapshotStore("server=127.0.0.1;database=test"));
            var myAggregate = aggregateRoot.GetByID<MyAggregate>("ID");
            if (myAggregate.HasChanged)
                aggregateRoot.Save(myAggregate);
        }
    }
}
