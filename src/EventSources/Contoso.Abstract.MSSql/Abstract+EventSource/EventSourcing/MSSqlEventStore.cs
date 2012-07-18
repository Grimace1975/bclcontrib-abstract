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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Abstract.Parts;
using System.Abstract.EventSourcing;
using Contoso.Abstract.Parts;
namespace Contoso.Abstract.EventSourcing
{
    /// <summary>
    /// MSSqlEventStore
    /// </summary>
    public class MSSqlEventStore : IBatchedEventStore
    {
        internal readonly string _connectionString;
        internal readonly string _tableName;
        internal readonly Func<string, object> _makeAggregateID;
        private readonly ITypeSerializer _serializer;

        /// <summary>
        /// EventOrdinal
        /// </summary>
        protected class EventOrdinal
        {
            /// <summary>
            /// 
            /// </summary>
            public int AggregateID, Type, Blob;
            /// <summary>
            /// Initializes a new instance of the <see cref="EventOrdinal"/> class.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <param name="hasAggregateID">if set to <c>true</c> [has aggregate ID].</param>
            public EventOrdinal(IDataReader r, bool hasAggregateID)
            {
                if (hasAggregateID)
                    AggregateID = r.GetOrdinal("AggregateID");
                Type = r.GetOrdinal("Type");
                Blob = r.GetOrdinal("Blob");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="serializer">The serializer.</param>
        public MSSqlEventStore(string connectionString, ITypeSerializer serializer)
            : this(connectionString, "AggregateEvent", null, serializer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="serializer">The serializer.</param>
        public MSSqlEventStore(string connectionString, string tableName, ITypeSerializer serializer)
            : this(connectionString, tableName, null, serializer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="makeAggregateID">The make aggregate ID.</param>
        /// <param name="serializer">The serializer.</param>
        public MSSqlEventStore(string connectionString, Func<string, object> makeAggregateID, ITypeSerializer serializer)
            : this(connectionString, "AggregateEvent", makeAggregateID, serializer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="makeAggregateID">The make aggregate ID.</param>
        /// <param name="serializer">The serializer.</param>
        public MSSqlEventStore(string connectionString, string tableName, Func<string, object> makeAggregateID, ITypeSerializer serializer)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            _connectionString = connectionString;
            _tableName = tableName;
            _makeAggregateID = makeAggregateID;
            _serializer = serializer;
            TypeMappingTo = new Dictionary<string, Type>();
            TypeMappingFrom = new Dictionary<Type, string>();
        }

        /// <summary>
        /// Gets the type mapping to.
        /// </summary>
        public Dictionary<string, Type> TypeMappingTo { get; private set; }

        /// <summary>
        /// Gets the type mapping from.
        /// </summary>
        public Dictionary<Type, string> TypeMappingFrom { get; private set; }

        /// <summary>
        /// Gets the events by ID.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="startSequence">The start sequence.</param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByID(object aggregateID, int startSequence)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
Select Type, Blob
From dbo.[{0}]
	Where (AggregateID = @id)
	And (EventSequence > @sequence)
Order By EventSequence;", _tableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@id", SqlDbType = SqlDbType.NVarChar, Value = aggregateID },
                    new SqlParameter { ParameterName = "@sequence", SqlDbType = SqlDbType.Int, Value = startSequence } });
                connection.Open();
                using (var r = command.ExecuteReader())
                {
                    var ordinal = new EventOrdinal(r, false);
                    while (r.Read())
                        yield return MakeEvent(r, ordinal);
                }
            }
        }

        /// <summary>
        /// Gets the events by I ds.
        /// </summary>
        /// <param name="aggregateIDs">The aggregate I ds.</param>
        /// <returns></returns>
        public IEnumerable<AggregateTuple<IEnumerable<Event>>> GetEventsByIDs(IEnumerable<AggregateTuple<int>> aggregateIDs)
        {
            var xml = new XElement("r", aggregateIDs
                .Select(x => new XElement("i",
                    new XAttribute("i", x.AggregateID),
                    new XAttribute("s", x.Item1)))
                );
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
Select AggregateId, Type, Blob
From dbo.[{0}]
    Inner Join @xml.nodes(N'/r/i') _xml(item)
    On (AggregateID = _xml.item.value(N'@i', N'nvarchar(100)'))
    And (EventSequence > _xml.item.value(N'@s', N'int'))
Order By AggregateID, EventSequence;", _tableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@xml", SqlDbType = SqlDbType.Xml, Value = xml.ToString() } });
                connection.Open();
                var events = new List<AggregateTuple<IEnumerable<Event>>>();
                using (var r = command.ExecuteReader())
                {
                    var eventGroup = new List<Event>();
                    string lastAggregateID = null;
                    var ordinal = new EventOrdinal(r, true);
                    while (r.Read())
                    {
                        var aggregateID = r.Field<string>(ordinal.AggregateID);
                        if (lastAggregateID != aggregateID)
                        {
                            events.Add(new AggregateTuple<IEnumerable<Event>>
                            {
                                AggregateID = lastAggregateID,
                                Item1 = eventGroup,
                            });
                            lastAggregateID = aggregateID;
                            eventGroup = new List<Event>();
                        }
                        eventGroup.Add(MakeEvent(r, ordinal));
                    }
                    if (eventGroup.Count > 0)
                        events.Add(new AggregateTuple<IEnumerable<Event>>
                        {
                            AggregateID = lastAggregateID,
                            Item1 = eventGroup,
                        });
                }
                return events;
            }
        }

        /// <summary>
        /// Saves the events.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="events">The events.</param>
        public void SaveEvents(object aggregateID, IEnumerable<Event> events)
        {
            var xml = new XElement("r", events
                .Select(x =>
                {
                    var eventType = x.GetType();
                    return new XElement("e",
                        (x.EventSequence.HasValue ? new XAttribute("s", x.EventSequence) : null),
                        new XAttribute("d", x.EventDate),
                        new XAttribute("t", TypeMapFrom(eventType)),
                        _serializer.WriteObject(eventType, x));
                }));
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
Insert dbo.[{0}] (AggregateID, EventSequence, EventDate, Type, Blob)
Select @id, _xml.item.value(N'@s', N'int'), _xml.item.value(N'@d', N'datetime'), _xml.item.value(N'@t', N'nvarchar(500)'), _xml.item.value(N'.', N'nvarchar(max)')
From @xml.nodes(N'/r/e') _xml(item);", _tableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@id", SqlDbType = SqlDbType.NVarChar, Value = aggregateID },
                    new SqlParameter { ParameterName = "@xml", SqlDbType = SqlDbType.Xml, Value = xml.ToString() } });
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Saves the events.
        /// </summary>
        /// <param name="events">The events.</param>
        public void SaveEvents(IEnumerable<AggregateTuple<IEnumerable<Event>>> events)
        {
            var xml = new XElement("r", events
                .SelectMany(x => x.Item1, (a, x) => new { a, x })
                .Select(m =>
                {
                    var x = m.x;
                    var eventType = x.GetType();
                    return new XElement("e",
                        new XAttribute("i", m.a),
                        (x.EventSequence.HasValue ? new XAttribute("s", x.EventSequence) : null),
                        new XAttribute("d", x.EventDate),
                        new XAttribute("t", TypeMapFrom(eventType)),
                        _serializer.WriteObject(eventType, x));
                }));
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
Insert dbo.[{0}] (AggregateID, EventSequence, EventDate, Type, Blob)
Select _xml.item.value(N'@i', N'nvarchar(100)'), _xml.item.value(N'@s', N'int'), _xml.item.value(N'@d', N'datetime'), _xml.item.value(N'@t', N'nvarchar(500)'), _xml.item.value(N'.', N'nvarchar(max)')
From @xml.nodes(N'/r/e') _xml(item);", _tableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@xml", SqlDbType = SqlDbType.Xml, Value = xml.ToString() } });
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private Event MakeEvent(SqlDataReader r, EventOrdinal ordinal)
        {
            var type = TypeMapTo(r.Field<string>(ordinal.Type));
            var blob = r.Field<string>(ordinal.Blob);
            return (type != null ? _serializer.ReadObject<Event>(type, blob) : null);
        }

        private Type TypeMapTo(string type)
        {
            Type v;
            return (TypeMappingTo.TryGetValue(type, out v) ? v : Type.GetType(type));
        }

        private string TypeMapFrom(Type type)
        {
            string v;
            return (TypeMappingFrom.TryGetValue(type, out v) ? v : type.AssemblyQualifiedName);
        }

        //        public IEnumerable<Event> GetEventsByEventTypes(IEnumerable<Type> eventTypes)
        //        {
        //            var eventTypesXml = new XElement("r", eventTypes
        //                .Select(x => new XElement("e",
        //                    new XAttribute("type", x.AssemblyQualifiedName)
        //                )));
        //            using (var connection = new SqlConnection(_connectionString))
        //            {
        //                var sql = string.Format(@"
        //Select Type, Blob
        //From dbo.[{0}]
        //	Inner Join @eventTypesXml.nodes(N'/r/e') _xml(item)
        //	On (AggregateEvent.Type = _xml.item.value(N'@type', N'nvarchar(500)'))
        //Order By AggregateId, EventSequence;", _tableName);
        //                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
        //                command.Parameters.AddRange(new[] {
        //                    new SqlParameter { ParameterName = "@eventTypesXml", SqlDbType = SqlDbType.Xml, Value = (eventTypesXml != null ? eventTypesXml.ToString() : string.Empty) } });
        //                connection.Open();
        //                var events = new List<Event>();
        //                using (var r = command.ExecuteReader())
        //                {
        //                    var ordinal = new EventOrdinal(r);
        //                    while (r.Read())
        //                        events.Add(MakeEvent(r, ordinal));
        //                }
        //                return events;
        //            }
        //        }
    }
}