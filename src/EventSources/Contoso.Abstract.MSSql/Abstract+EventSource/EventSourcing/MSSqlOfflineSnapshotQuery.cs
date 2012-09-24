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
using System.Xml.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Abstract.EventSourcing;
namespace Contoso.Abstract.EventSourcing
{
    /// <summary>
    /// MSSqlOfflineSnapshotQuery
    /// </summary>
    public class MSSqlOfflineSnapshotQuery
    {
        private readonly string _connectionString;
        private readonly string _snapshotTableName;
        private readonly string _eventTableName;
        private readonly Func<string, object> _makeAggregateID;

        /// <summary>
        /// ItemOrdinal
        /// </summary>
        protected class ItemOrdinal
        {
            /// <summary>
            /// 
            /// </summary>
            public int AggregateType, AggregateID;
            /// <summary>
            /// Initializes a new instance of the <see cref="ItemOrdinal"/> class.
            /// </summary>
            /// <param name="r">The r.</param>
            public ItemOrdinal(IDataReader r)
            {
                AggregateType = r.GetOrdinal("AggregateType");
                AggregateID = r.GetOrdinal("AggregateID");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlOfflineSnapshotQuery"/> class.
        /// </summary>
        /// <param name="snapshotStore">The snapshot store.</param>
        /// <param name="eventStore">The event store.</param>
        public MSSqlOfflineSnapshotQuery(MSSqlAggregateRootSnapshotStore snapshotStore, MSSqlEventStore eventStore)
        {
            if (snapshotStore == null)
                throw new ArgumentNullException("snapshotStore");
            if (eventStore == null)
                throw new ArgumentNullException("eventStore");
            _snapshotTableName = snapshotStore._tableName;
            _eventTableName = eventStore._tableName;
            _makeAggregateID = eventStore._makeAggregateID;
            _connectionString = eventStore._connectionString;
        }

        /// <summary>
        /// Gets the aggregates to snapshot.
        /// </summary>
        /// <param name="aggregateTypes">The aggregate types.</param>
        /// <returns></returns>
        public IEnumerable<AggregateTuple<Type>> GetAggregatesToSnapshot(IEnumerable<Type> aggregateTypes)
        {
            var xml = new XElement("r", aggregateTypes
                .Select(x =>
                {
                    var eventType = x.GetType();
                    return new XElement("a",
                        new XAttribute("t", eventType.AssemblyQualifiedName));
                }));
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
With _Type As (
	Select _xml.item.value(N'@t', N'nvarchar(500)') As Type
	From @xml.nodes(N'/r/a') _xml(item)
), _Event As (
	Select AggregateID, Max(EventSequence) + {0} As EventSequence
	From dbo.[{2}]
	Group By AggregateID
)
Select AggregateID, AggregateType
From dbo.[{1}] _Snapshot
	Where (Exists(
		Select Top 1 *
		From _Event
			Where (_Snapshot.AggregateID = _Event.AggregateID)
			And (_Snapshot.LastEventSequence >= _Event.EventSequence)));", 10, _snapshotTableName, _eventTableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@xml", SqlDbType = SqlDbType.Xml, Value = xml.ToString() } });
                connection.Open();
                using (var r = command.ExecuteReader())
                {
                    var ordinal = new ItemOrdinal(r);
                    while (r.Read())
                        yield return MakeItem(r, ordinal);
                }
            }
        }

        private AggregateTuple<Type> MakeItem(SqlDataReader r, ItemOrdinal ordinal)
        {
            var aggregateType = Type.GetType(r.Field<string>(ordinal.AggregateType));
            var aggregateID = r.Field<string>(ordinal.AggregateID);
            return new AggregateTuple<Type>
            {
                AggregateID = (_makeAggregateID == null ? aggregateID : _makeAggregateID(aggregateID)),
                Item1 = aggregateType,
            };
        }
    }
}