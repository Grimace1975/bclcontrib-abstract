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
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
namespace System.Abstract.EventSourcing
{
    /// <summary>
    /// AggregateRootExtensions
    /// </summary>
    public static class AggregateRootExtensions
    {
        public static TAggregateRoot GetById<TAggregateRoot>(this IAggregateRootRepository repository, object aggregateId)
            where TAggregateRoot : AggregateRoot { return repository.GetById<TAggregateRoot>(aggregateId, 0); }
        public static object GetById(this IAggregateRootRepository repository, Type aggregateType, object aggregateId) { return repository.GetById<AggregateRoot>(aggregateId, 0); }
        public static AggregateRoot GetById(this IAggregateRootRepository repository, Type aggregateType, object aggregateId, AggregateRootQueryOptions queryOptions) { return repository.GetById<AggregateRoot>(aggregateId, queryOptions); }
        public static IEnumerable<AggregateRoot> GetManyByIds(this IAggregateRootRepository repository, IEnumerable<object> aggregateIds, Type aggregateType, AggregateRootQueryOptions queryOptions) { return repository.GetManyByIds<AggregateRoot>(aggregateIds, queryOptions); }
        public static void MakeSnapshot(this IAggregateRootRepository repository, AggregateRoot aggregate) { repository.MakeSnapshot(aggregate, null); }
    }
}