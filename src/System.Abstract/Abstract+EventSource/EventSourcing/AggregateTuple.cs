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
namespace System.Abstract.EventSourcing
{
    /// <summary>
    /// AggregateTuple
    /// </summary>
    public class AggregateTuple<T1>
    {
        /// <summary>
        /// Gets or sets the aggregate ID.
        /// </summary>
        /// <value>
        /// The aggregate ID.
        /// </value>
        public object AggregateID { get; set; }
        /// <summary>
        /// Gets or sets the item1.
        /// </summary>
        /// <value>
        /// The item1.
        /// </value>
        public T1 Item1 { get; set; }
    }
    /// <summary>
    /// AggregateTuple
    /// </summary>
    public class AggregateTuple<T1, T2>
    {
        /// <summary>
        /// Gets or sets the aggregate ID.
        /// </summary>
        /// <value>
        /// The aggregate ID.
        /// </value>
        public object AggregateID { get; set; }
        /// <summary>
        /// Gets or sets the item1.
        /// </summary>
        /// <value>
        /// The item1.
        /// </value>
        public T1 Item1 { get; set; }
        /// <summary>
        /// Gets or sets the item2.
        /// </summary>
        /// <value>
        /// The item2.
        /// </value>
        public T2 Item2 { get; set; }
    }
}