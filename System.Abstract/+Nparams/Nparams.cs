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
namespace System
{
    /// <summary>
    /// Nparams
    /// </summary>
    public abstract class Nparams : IDictionary<string, object>
    {
        public static Nparams Parse(Nparams args) { return (args != null ? args : null); }
        public static Nparams Parse(IDictionary<string, object> args) { return (args != null ? NparamsManager.Parse(args) : null); }
        public static Nparams Parse(string[] args) { return (args != null ? NparamsManager.Parse(args) : null); }
        public static Nparams Parse(object args) { return (args != null ? NparamsManager.Parse(args) : null); }

        public abstract void Add(string key, object value);
        public abstract bool ContainsKey(string key);
        public abstract int Count { get; }
        public abstract ICollection<string> Keys { get; }
        public abstract bool Remove(string key);
        public abstract bool TryGetValue(string key, out object value);
        public abstract ICollection<object> Values { get; }
        public abstract object this[string key] { get; set; }
        public abstract void Clear();
        public bool IsReadOnly
        {
            get { return false; }
        }
        public abstract IEnumerator<KeyValuePair<string, object>> GetEnumerator();
        public abstract string[] ToStringArray();
        public T Slice<T>(string key) { return Slice<T>(key, default(T)); }
        public abstract T Slice<T>(string key, T defaultValue);
        //public T Value<T>(string key) { return Value<T>(key, default(T)); }
        //public abstract T Value<T>(string key, T defaultValue);
        public T Get<T>()
        {
            return (T)this[typeof(T).Name];
        }
        public void Set<T>(T value)
        {
            this[typeof(T).Name] = value;
        }
        //
        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() { return ((Nparams)this).GetEnumerator(); }
        //
        public void Add(KeyValuePair<string, object> item) { throw new NotImplementedException(); }
        public bool Contains(KeyValuePair<string, object> item) { throw new NotImplementedException(); }
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) { throw new NotImplementedException(); }
        public bool Remove(KeyValuePair<string, object> item) { throw new NotImplementedException(); }
        //
        public void AddRange(IDictionary<string, object> dictionary)
        {
            if (dictionary != null)
                foreach (var pair in dictionary)
                    Add(pair.Key, pair.Value);
        }
    }
}
