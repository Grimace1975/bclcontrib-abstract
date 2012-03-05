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
using System.Collections.Generic;
namespace System.Abstract.EventSourcing
{
    /// <summary>
    /// EventSourcingExtensions
    /// </summary>
    public static class EventSourcingExtensions
    {
        public static TAggregateRoot GetByID<TAggregateRoot>(this IAggregateRootRepository repository, object aggregateID)
            where TAggregateRoot : AggregateRoot { return repository.GetByID<TAggregateRoot>(aggregateID, 0); }
        public static object GetByID(this IAggregateRootRepository repository, Type aggregateType, object aggregateID) { return repository.GetByID<AggregateRoot>(aggregateID, 0); }
        public static AggregateRoot GetByID(this IAggregateRootRepository repository, Type aggregateType, object aggregateID, AggregateRootQueryOptions queryOptions) { return repository.GetByID<AggregateRoot>(aggregateID, queryOptions); }
        public static IEnumerable<AggregateRoot> GetManyByIDs(this IAggregateRootRepository repository, IEnumerable<object> aggregateIDs, Type aggregateType, AggregateRootQueryOptions queryOptions) { return repository.GetManyByIDs<AggregateRoot>(aggregateIDs, queryOptions); }
        public static void MakeSnapshot(this IAggregateRootRepository repository, AggregateRoot aggregate) { repository.MakeSnapshot(aggregate, null); }
    }
}