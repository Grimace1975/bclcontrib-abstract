# Event Sourcing
> abstracts an [event sourcing](http://cqrsinfo.com/documents/events-as-storage-mechanism/ "Events as a Storage Mechanism") system. this event sourcing abstraction is implemented by various providers.

Event sourcing is a good way of storing state information in a version-able form. It is especially useful in updating other systems by differentials.




# Defining the Provider




# C# coding usage

## Defining an AggregateRoot:

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

## Creating an AggregateRoot Repository

	var aggregateRoot = new AggregateRootRepository(new SqlEventStore("connectionString"), new SqlAggregateRootSnapshotStore("connectionString"));

## Consuming an AggregateRoot Repository

	var myAggregate = aggregateRoot.GetById<MyAggregate>("ID");
	myAggregate.DoMethod();
	if (myAggregate.HasChanged)
		aggregateRoot.Save(myAggregate);

