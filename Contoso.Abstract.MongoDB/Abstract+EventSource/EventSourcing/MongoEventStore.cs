#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Abstract;
using System.Abstract.EventSourcing;
namespace Contoso.Abstract.EventSourcing
{
    //public class MongoEventStore : IEventStore
    //{
    //    private readonly IMongoDatabase _database;
    //    private readonly Func<object, object, bool> _aggregateKeyEqualityComparer;

    //    public MongoEventStore(string connectionString, ITypeCatalog eventTypeCatalog, Func<object, object, bool> aggregateKeyEqualityComparer)
    //    {
    //        var connectionStringBuilder = new MongoConnectionStringBuilder(connectionString);
    //        var mongo = new Mongo(BuildMongoConfiguration(connectionString, eventTypeCatalog));
    //        mongo.Connect();
    //        _database = mongo.GetDatabase(connectionStringBuilder.Database);
    //        _aggregateKeyEqualityComparer = aggregateKeyEqualityComparer;
    //    }

    //    private static MongoConfiguration BuildMongoConfiguration(string connectionString, ITypeCatalog eventTypeCatalog)
    //    {
    //        var configurationBuilder = new MongoConfigurationBuilder();
    //        configurationBuilder.ConnectionString(connectionString);
    //        configurationBuilder.Mapping(mapping =>
    //        {
    //            mapping.DefaultProfile(profile => profile.SubClassesAre(type => type.IsSubclassOf(typeof(Event))));
    //            eventTypeCatalog.GetDerivedTypes(typeof(Event), true)
    //                .Yield(type => MongoBuilder.MapType(type, mapping));
    //        });
    //        return configurationBuilder.BuildConfiguration();
    //    }

    //    public IEnumerable<Event> GetEventsById(object aggregateId, int startSequence)
    //    {
    //        return _database.GetCollection<Event>("events")
    //            .Linq()
    //            .Where(e => _aggregateKeyEqualityComparer(e.AggregateId, aggregateId))
    //            .Where(e => e.EventSequence > startSequence)
    //            .ToList();
    //    }

    //    public IEnumerable<Event> GetEventsByEventTypes(IEnumerable<Type> eventTypes)
    //    {
    //        var document = new Document { { "_t", new Document { { "$in", eventTypes.Select(t => t.Name).ToArray() } } } };
    //        var cursor = _database.GetCollection<Event>("events").Find(document);
    //        return cursor.Documents;
    //    }

    //    public void SaveEvents(object aggregateId, IEnumerable<Event> events)
    //    {
    //        var mogoEvents = _database.GetCollection<Event>("events");
    //        mogoEvents.Insert(events);
    //    }
    //}
}