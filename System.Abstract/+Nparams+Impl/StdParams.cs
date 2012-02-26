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
using System.Reflection;
using System.ComponentModel;
namespace System
{
    /// <summary>
    /// StdParams
    /// </summary>
    public class StdParams : INparams
    {
        private static FieldInfo _comparerField = typeof(Dictionary<string, object>).GetField("comparer", BindingFlags.Instance | BindingFlags.NonPublic);
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public StdParams()
        {
            _comparerField.SetValue(_values, StringComparer.OrdinalIgnoreCase);
        }
        public StdParams(INparams args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            _comparerField.SetValue(_values, StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, object> pair in args)
                Add(pair.Key, pair.Value);
        }
        public StdParams(IDictionary<string, object> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            _comparerField.SetValue(_values, StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, object> pair in args)
                Add(pair.Key, pair.Value);
        }
        public StdParams(object args)
        {
            _comparerField.SetValue(_values, StringComparer.OrdinalIgnoreCase);
            AddValues(args);
        }

        #region Interface

        public void Add(string key, object value) { _values.Add(key, value); }
        public bool ContainsKey(string key) { return _values.ContainsKey(key); }

        public ICollection<string> Keys
        {
            get { return _values.Keys; }
        }

        public bool Remove(string key) { return _values.Remove(key); }
        public bool TryGetValue(string key, out object value) { return _values.TryGetValue(key, out value); }

        public ICollection<object> Values
        {
            get { return _values.Values; }
        }

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        public void Clear() { _values.Clear(); }
        public int Count
        {
            get { return _values.Count; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() { return _values.GetEnumerator(); }

        public bool IsReadOnly
        {
            get { return false; }
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() { return ((INparams)this).GetEnumerator(); }

        public void Add(KeyValuePair<string, object> item) { throw new NotImplementedException(); }
        public bool Contains(KeyValuePair<string, object> item) { throw new NotImplementedException(); }
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) { throw new NotImplementedException(); }
        public bool Remove(KeyValuePair<string, object> item) { throw new NotImplementedException(); }

        // added
        public void AddRange(IDictionary<string, object> args)
        {
            if (args != null)
                foreach (var pair in args)
                    Add(pair.Key, pair.Value);
        }

        public string[] ToStringArray()
        {
            return _values.Select(x =>
            {
                return x.Key + "=" + x.Value.ToString();
            }).ToArray();
        }

        public T Slice<T>(string key, T defaultValue)
        {
            object value;
            if (_values.TryGetValue(key, out value) && value is T)
            {
                _values.Remove(key);
                return (T)value;
            }
            return defaultValue;
        }

        #endregion

        private void AddValues(object values)
        {
            if (values != null)
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
                    Add(descriptor.Name, descriptor.GetValue(values));
        }
    }
}
