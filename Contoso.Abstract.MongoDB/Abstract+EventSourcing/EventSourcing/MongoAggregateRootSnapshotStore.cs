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
using System.Reflection;
using System.Abstract;
using System.Abstract.EventSourcing;
namespace System.Quality.EventSourcing
{
    //public class MongoAggregateRootSnapshotStore : IAggregateRootSnapshotStore
    //{
    //    private readonly IMongoDatabase _database;
    //    private readonly Func<object, object, bool> _aggregateKeyEqualityComparer;

    //    public MongoAggregateRootSnapshotStore(string connectionString, ITypeCatalog snapshotTypeCatalog, Func<object, object, bool> aggregateKeyEqualityComparer)
    //    {
    //        var connectionStringBuilder = new MongoConnectionStringBuilder(connectionString);
    //        var mongo = new Mongo(BuildMongoConfiguration(connectionString, snapshotTypeCatalog));
    //        mongo.Connect();
    //        _database = mongo.GetDatabase(connectionStringBuilder.Database);
    //        _aggregateKeyEqualityComparer = aggregateKeyEqualityComparer;
    //    }

    //    private static MongoConfiguration BuildMongoConfiguration(string connectionString, ITypeCatalog snapshotTypeCatalog)
    //    {
    //        var configurationBuilder = new MongoConfigurationBuilder();
    //        configurationBuilder.ConnectionString(connectionString);
    //        configurationBuilder.Mapping(mapping =>
    //        {
    //            mapping.DefaultProfile(profile => profile.SubClassesAre(type => type.IsSubclassOf(typeof(AggregateRootSnapshot))));
    //            snapshotTypeCatalog.GetDerivedTypes(typeof(AggregateRootSnapshot), true)
    //                .Yield(type => MongoBuilder.MapType(type, mapping));
    //        });
    //        return configurationBuilder.BuildConfiguration();
    //    }

    //    public AggregateRootSnapshot GetLatestSnapshot<TAggregateRoot>(object aggregateId)
    //        where TAggregateRoot : AggregateRoot
    //    {
    //        return _database.GetCollection<AggregateRootSnapshot>("snapshots")
    //            .Linq()
    //            .Where(x => x.AggregateId.Equals(aggregateId))
    //            .SingleOrDefault();
    //    }

    //    public void SaveSnapshot(Type aggregateType, AggregateRootSnapshot snapshot)
    //    {
    //        var monoSnapshots = _database.GetCollection<AggregateRootSnapshot>("snapshots");
    //        monoSnapshots.Update(snapshot, UpdateFlags.Upsert);
    //    }

    //    public Func<IAggregateRootRepository, AggregateRoot, bool> InlineSnapshotPredicate { get; set; }
    //}
}