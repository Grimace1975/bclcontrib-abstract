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
using System.Threading;
namespace System
{
    /// <summary>
    /// Lazy
    /// </summary>
    [Serializable]
    public class Lazy<T, TMetadata> : Lazy<T>
    {
        private TMetadata _metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T, TMetadata&gt;"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public Lazy(TMetadata metadata) { _metadata = metadata; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T, TMetadata&gt;"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="metadata">The metadata.</param>
        public Lazy(Func<T> valueFactory, TMetadata metadata)
            : base(valueFactory) { _metadata = metadata; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T, TMetadata&gt;"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="isThreadSafe">if set to <c>true</c> [is thread safe].</param>
        public Lazy(TMetadata metadata, bool isThreadSafe)
            : base(isThreadSafe) { _metadata = metadata; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T, TMetadata&gt;"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="mode">The mode.</param>
        public Lazy(TMetadata metadata, LazyThreadSafetyMode mode)
            : base(mode) { _metadata = metadata; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T, TMetadata&gt;"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="isThreadSafe">if set to <c>true</c> [is thread safe].</param>
        public Lazy(Func<T> valueFactory, TMetadata metadata, bool isThreadSafe)
            : base(valueFactory, isThreadSafe) { _metadata = metadata; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T, TMetadata&gt;"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="mode">The mode.</param>
        public Lazy(Func<T> valueFactory, TMetadata metadata, LazyThreadSafetyMode mode)
            : base(valueFactory, mode) { _metadata = metadata; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        public TMetadata Metadata
        {
            get { return _metadata; }
        }
    }
}
