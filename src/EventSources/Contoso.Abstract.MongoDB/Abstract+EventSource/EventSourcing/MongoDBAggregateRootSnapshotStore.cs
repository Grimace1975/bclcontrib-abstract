﻿#region License
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
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
namespace System.Abstract.EventSourcing
{
    public class MongoDBAggregateRootSnapshotStore : IAggregateRootSnapshotStore
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly Func<object, BsonValue> _makeAggregateID;

        public MongoDBAggregateRootSnapshotStore(string connectionString)
            : this(connectionString, "AggregateSnapshot", BsonValue.Create) { }
        public MongoDBAggregateRootSnapshotStore(string connectionString, string tableName)
            : this(connectionString, tableName, BsonValue.Create) { }
        public MongoDBAggregateRootSnapshotStore(string connectionString, Func<object, BsonValue> makeAggregateID)
            : this(connectionString, "AggregateSnapshot", makeAggregateID) { }
        public MongoDBAggregateRootSnapshotStore(string connectionString, string tableName, Func<object, BsonValue> makeAggregateID)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");
            if (makeAggregateID == null)
                throw new ArgumentNullException("makeAggregateID");
            _connectionString = connectionString;
            _tableName = tableName;
            _makeAggregateID = makeAggregateID;
        }

        public AggregateRootSnapshot GetLatestSnapshot<TAggregateRoot>(object aggregateID)
            where TAggregateRoot : AggregateRoot
        {
            var connectionStringBuilder = new MongoConnectionStringBuilder(_connectionString);
            var database = MongoServer.Create(connectionStringBuilder).GetDatabase(connectionStringBuilder.DatabaseName);
            var query = Query.EQ("AggregateID", _makeAggregateID(aggregateID));
            return database.GetCollection<AggregateRootSnapshot>(_tableName).FindOne(query);
        }

        public void SaveSnapshot(Type aggregateType, AggregateRootSnapshot snapshot)
        {
            var connectionStringBuilder = new MongoConnectionStringBuilder(_connectionString);
            var database = MongoServer.Create(connectionStringBuilder).GetDatabase(connectionStringBuilder.DatabaseName);
            var query = Query.EQ("AggregateID", _makeAggregateID(snapshot.AggregateID));
            var update = new UpdateDocument
            {
                { "$set", snapshot.ToBson() }
            };
            database.GetCollection<AggregateRootSnapshot>(_tableName).Update(query, update, UpdateFlags.Upsert);
        }

        public Func<IAggregateRootRepository, AggregateRoot, bool> InlineSnapshotPredicate { get; set; }
    }
}