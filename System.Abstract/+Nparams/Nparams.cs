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
        void AddRange(IDictionary<string, object> args);
        string[] ToStringArray();
        T Slice<T>(string key, T defaultValue);
    }

    /// <summary>
    /// Nparams
    /// </summary>
    public class Nparams : Collections.IEnumerable, IEnumerable<KeyValuePair<string, object>>
    {
        internal INparams _base;

        public static Nparams Create() { return NparamsManager.Create(); }
        public static Nparams Parse(Nparams args) { return args; }
        public static Nparams Parse(IDictionary<string, object> args) { return (args != null ? NparamsManager.Parse(args) : null); }
        public static Nparams Parse(string[] args) { return (args != null && args.Length > 0 ? NparamsManager.Parse(args) : null); }
        public static Nparams Parse(object args) { return (args != null ? NparamsManager.Parse(args) : null); }

        public Nparams(INparams @base) { _base = @base; }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() { return (_base != null ? _base.GetEnumerator() : null); }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() { return (_base != null ? _base.GetEnumerator() : null); }

        #region Properties

        public int Count
        {
            get { return (_base != null ? _base.Count : 0); }
        }

        public bool IsReadOnly
        {
            get { return (_base != null ? _base.IsReadOnly : false); }
        }

        public object this[string key]
        {
            get { return (_base != null ? _base[key] : null); }
            set { if (_base == null) throw new InvalidOperationException(); _base[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return (_base != null ? _base.Keys : null); }
        }

        public ICollection<object> Values
        {
            get { return (_base != null ? _base.Values : null); }
        }

        #endregion
    }
}
