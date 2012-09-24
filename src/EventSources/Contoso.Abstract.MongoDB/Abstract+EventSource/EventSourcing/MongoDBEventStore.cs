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
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
namespace Contoso.Abstract.EventSourcing
{
    /// <summary>
    /// MongoEventStore
    /// </summary>
    public class MongoDBEventStore : IEventStore
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly Func<object, BsonValue> _makeAggregateID;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MongoDBEventStore(string connectionString)
            : this(connectionString, "AggregateEvent", BsonValue.Create) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        public MongoDBEventStore(string connectionString, string tableName)
            : this(connectionString, tableName, BsonValue.Create) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="makeAggregateID">The make aggregate ID.</param>
        public MongoDBEventStore(string connectionString, Func<object, BsonValue> makeAggregateID)
            : this(connectionString, "AggregateEvent", makeAggregateID) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="makeAggregateID">The make aggregate ID.</param>
        public MongoDBEventStore(string connectionString, string tableName, Func<object, BsonValue> makeAggregateID)
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

        /// <summary>
        /// Gets the events by ID.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="startSequence">The start sequence.</param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByID(object aggregateID, int startSequence)
        {
            var connectionStringBuilder = new MongoConnectionStringBuilder(_connectionString);
            var database = MongoServer.Create(connectionStringBuilder).GetDatabase(connectionStringBuilder.DatabaseName);
            var query = Query.And(
                Query.EQ("AggregateID", _makeAggregateID(aggregateID)),
                Query.GT("EventSequence", startSequence));
            return database.GetCollection<Event>(_tableName).Find(query);
        }

        /// <summary>
        /// Saves the events.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="events">The events.</param>
        public void SaveEvents(object aggregateID, IEnumerable<Event> events)
        {
            var connectionStringBuilder = new MongoConnectionStringBuilder(_connectionString);
            var database = MongoServer.Create(connectionStringBuilder).GetDatabase(connectionStringBuilder.DatabaseName);
            database.GetCollection<Event>(_tableName).Insert(events);
        }
    }
}