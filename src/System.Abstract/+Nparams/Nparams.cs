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
using System.Linq;
namespace System
{
    /// <summary>
    /// INparams
    /// </summary>
    public interface INparams : IDictionary<string, object>
    {
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="args">The args.</param>
        void AddRange(IDictionary<string, object> args);
        /// <summary>
        /// Toes the string array.
        /// </summary>
        /// <returns></returns>
        string[] ToStringArray();
        /// <summary>
        /// Slices the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        T Slice<T>(string key, T defaultValue);
    }

    /// <summary>
    /// Nparams
    /// </summary>
    public class Nparams : Collections.IEnumerable, IEnumerable<KeyValuePair<string, object>>
    {
        internal INparams _base;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public static Nparams Create() { return NparamsManager.Create(); }
        /// <summary>
        /// Parses the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static Nparams Parse(Nparams args) { return args; }
        /// <summary>
        /// Parses the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static Nparams Parse(IDictionary<string, object> args) { return (args != null ? NparamsManager.Parse(args) : null); }
        /// <summary>
        /// Parses the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static Nparams Parse(string[] args) { return (args != null && args.Length > 0 ? NparamsManager.Parse(args) : null); }
        /// <summary>
        /// Parses the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static Nparams Parse(object args) { return (args != null ? NparamsManager.Parse(args) : null); }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nparams"/> class.
        /// </summary>
        /// <param name="base">The @base.</param>
        public Nparams(INparams @base) { _base = @base; }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() { return (_base != null ? _base.GetEnumerator() : null); }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() { return (_base != null ? _base.GetEnumerator() : null); }

        #region Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get { return (_base != null ? _base.Count : 0); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return (_base != null ? _base.IsReadOnly : false); }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        public object this[string key]
        {
            get { return (_base != null ? _base[key] : null); }
            set { if (_base == null) throw new InvalidOperationException(); _base[key] = value; }
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        public ICollection<string> Keys
        {
            get { return (_base != null ? _base.Keys : null); }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public ICollection<object> Values
        {
            get { return (_base != null ? _base.Values : null); }
        }

        #endregion
    }
}
