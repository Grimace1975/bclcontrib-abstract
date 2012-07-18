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
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Abstract.Parts;
using System.Abstract.EventSourcing;
using Contoso.Abstract.Parts;
namespace Contoso.Abstract.EventSourcing
{
    /// <summary>
    /// MSSqlAggregateRootSnapshotStore
    /// </summary>
    public class MSSqlAggregateRootSnapshotStore : IBatchedAggregateRootSnapshotStore
    {
        private readonly string _connectionString;
        internal readonly string _tableName;
        private readonly Func<string, object> _makeAggregateID;
        private readonly ITypeSerializer _serializer;

        /// <summary>
        /// SnapshotOrdinal
        /// </summary>
        protected class SnapshotOrdinal
        {
            /// <summary>
            /// 
            /// </summary>
            public int AggregateID, Type, Blob;
            /// <summary>
            /// Initializes a new instance of the <see cref="SnapshotOrdinal"/> class.
            /// </summary>
            /// <param name="r">The r.</param>
            /// <param name="hasAggregateID">if set to <c>true</c> [has aggregate ID].</param>
            public SnapshotOrdinal(IDataReader r, bool hasAggregateID)
            {
                if (hasAggregateID)
                    AggregateID = r.GetOrdinal("AggregateID");
                Type = r.GetOrdinal("Type");
                Blob = r.GetOrdinal("Blob");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlAggregateRootSnapshotStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="serializer">The serializer.</param>
        public MSSqlAggregateRootSnapshotStore(string connectionString, ITypeSerializer serializer)
            : this(connectionString, "AggregateSnapshot", null, serializer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlAggregateRootSnapshotStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="serializer">The serializer.</param>
        public MSSqlAggregateRootSnapshotStore(string connectionString, string tableName, ITypeSerializer serializer)
            : this(connectionString, tableName, null, serializer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlAggregateRootSnapshotStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="makeAggregateID">The make aggregate ID.</param>
        /// <param name="serializer">The serializer.</param>
        public MSSqlAggregateRootSnapshotStore(string connectionString, Func<string, object> makeAggregateID, ITypeSerializer serializer)
            : this(connectionString, "AggregateSnapshot", makeAggregateID, serializer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSqlAggregateRootSnapshotStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="makeAggregateID">The make aggregate ID.</param>
        /// <param name="serializer">The serializer.</param>
        public MSSqlAggregateRootSnapshotStore(string connectionString, string tableName, Func<string, object> makeAggregateID, ITypeSerializer serializer)
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
        }

        /// <summary>
        /// Gets the latest snapshot.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <returns></returns>
        public AggregateRootSnapshot GetLatestSnapshot<TAggregateRoot>(object aggregateID)
            where TAggregateRoot : AggregateRoot
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
Select Top 1 Type, Blob
From dbo.[{0}]
	Where (AggregateID = @id)
    And (AggregateType = @aType);", _tableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@id", SqlDbType = SqlDbType.NVarChar, Value = aggregateID },
                    new SqlParameter { ParameterName = "@aType", SqlDbType = SqlDbType.NVarChar, Value = typeof(TAggregateRoot).AssemblyQualifiedName } });
                connection.Open();
                using (var r = command.ExecuteReader())
                {
                    var ordinal = new SnapshotOrdinal(r, false);
                    return (r.Read() ? MakeSnapshot(r, ordinal) : null);
                }
            }
        }

        /// <summary>
        /// Gets the latest snapshots.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="aggregateIDs">The aggregate I ds.</param>
        /// <returns></returns>
        public IEnumerable<AggregateTuple<AggregateRootSnapshot>> GetLatestSnapshots<TAggregateRoot>(IEnumerable<object> aggregateIDs)
            where TAggregateRoot : AggregateRoot
        {
            var xml = new XElement("r", aggregateIDs
                .Select(x => new XElement("s",
                    new XAttribute("i", x.ToString())))
                );
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
Select Type, Blob
From dbo.[{0}]
    Inner Join @xml.nodes(N'/r/s') _xml(item)
    On (AggregateID = _xml.item.value(N'@i', N'nvarchar(100)'))
    And (AggregateType = @aType);", _tableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@aType", SqlDbType = SqlDbType.NVarChar, Value = typeof(TAggregateRoot).AssemblyQualifiedName },
                    new SqlParameter { ParameterName = "@xml", SqlDbType = SqlDbType.Xml, Value = xml.ToString()  } });
                connection.Open();
                var snapshots = new List<AggregateTuple<AggregateRootSnapshot>>();
                using (var r = command.ExecuteReader())
                {
                    var ordinal = new SnapshotOrdinal(r, true);
                    while (r.Read())
                        snapshots.Add(MakeSnapshotTuple(r, ordinal));
                }
                return snapshots;
            }
        }

        /// <summary>
        /// Saves the snapshot.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <param name="snapshot">The snapshot.</param>
        public void SaveSnapshot(Type aggregateType, AggregateRootSnapshot snapshot)
        {
            var snapshotType = snapshot.GetType();
            var snapshotBlob = _serializer.WriteObject(snapshotType, snapshot);
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
With _Target As (
	Select *
	From dbo.[{0}]
		Where (AggregateID = @id)
        And (AggregateType = @aType)
)
Merge _Target
Using (
	Select @id As AggregateID, @aType as AggregateType, @sequence as LastEventSequence,
    @type As Type, @blob As Blob) As _Source
On (_Target.AggregateID = _Source.AggregateID)
And (_Target.AggregateType = _Source.AggregateType)
When Matched Then
	Update Set Type = _Source.Type, Blob = _Source.Blob
When Not Matched By Target Then
	Insert (AggregateID, AggregateType, Type, Blob)
	Values (_Source.AggregateID, _Source.AggregateType, _Source.Type, _Source.Blob);", _tableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@id", SqlDbType = SqlDbType.NVarChar, Value = snapshot.AggregateID },
                    new SqlParameter { ParameterName = "@aType", SqlDbType = SqlDbType.NVarChar, Value = aggregateType.AssemblyQualifiedName },
                    new SqlParameter { ParameterName = "@sequence", SqlDbType = SqlDbType.Int, Value = snapshot.LastEventSequence },
                    new SqlParameter { ParameterName = "@type", SqlDbType = SqlDbType.NVarChar, Value = snapshotType.AssemblyQualifiedName },
                    new SqlParameter { ParameterName = "@blob", SqlDbType = SqlDbType.NVarChar, Value = snapshotBlob } });
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Saves the snapshots.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <param name="snapshots">The snapshots.</param>
        public void SaveSnapshots(Type aggregateType, IEnumerable<AggregateRootSnapshot> snapshots)
        {
            var xml = new XElement("r", snapshots
                .Select(x =>
                {
                    var snapshotType = x.GetType();
                    return new XElement("s",
                        new XAttribute("i", x.AggregateID),
                        new XAttribute("s", x.LastEventSequence),
                        new XAttribute("t", snapshotType.AssemblyQualifiedName),
                        _serializer.WriteObject(snapshotType, x));
                }));
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format(@"
With _Target As (
	Select *
	From dbo.[{0}]
		Where (AggregateType = @aType)
)
Merge _Target
Using (	
	Select _xml.item.value(N'@i', N'nvarchar(100)') As AggregateID, @aType as AggregateType, _xml.item.value(N'@s', N'int') As LastEventSequence
	, _xml.item.value(N'@t', N'nvarchar(400)') As Type, _xml.item.value(N'.', N'nvarchar(max)') As Blob
	From @xml.nodes(N'/r/s') _xml(item) ) As _Source
On (_Target.AggregateID = _Source.AggregateID)
And (_Target.AggregateType = _Source.AggregateType)
When Matched Then
	Update Set LastEventSequence = _Source.LastEventSequence, Type = _Source.Type, Blob = _Source.Blob
When Not Matched By Target Then
	Insert (AggregateID, AggregateType, LastEventSequence, Type, Blob)
	Values (_Source.AggregateID, _Source.AggregateType, _Source.LastEventSequence, _Source.Type, _Source.Blob);", _tableName);
                var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
                command.Parameters.AddRange(new[] {
                    new SqlParameter { ParameterName = "@aType", SqlDbType = SqlDbType.NVarChar, Value = aggregateType.AssemblyQualifiedName },
                    new SqlParameter { ParameterName = "@xml", SqlDbType = SqlDbType.NVarChar, Value = xml.ToString() } });
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private AggregateRootSnapshot MakeSnapshot(SqlDataReader r, SnapshotOrdinal ordinal)
        {
            var type = Type.GetType(r.Field<string>(ordinal.Type));
            var blob = r.Field<string>(ordinal.Blob);
            return (type != null ? _serializer.ReadObject<AggregateRootSnapshot>(type, blob) : null);
        }

        private AggregateTuple<AggregateRootSnapshot> MakeSnapshotTuple(SqlDataReader r, SnapshotOrdinal ordinal)
        {
            var aggregateID = r.Field<string>(ordinal.AggregateID);
            return new AggregateTuple<AggregateRootSnapshot>
            {
                AggregateID = (_makeAggregateID == null ? aggregateID : _makeAggregateID(aggregateID)),
                Item1 = MakeSnapshot(r, ordinal),
            };
        }

        /// <summary>
        /// Gets the inline snapshot predicate.
        /// </summary>
        public Func<IAggregateRootRepository, AggregateRoot, bool> InlineSnapshotPredicate { get; set; }
    }
}