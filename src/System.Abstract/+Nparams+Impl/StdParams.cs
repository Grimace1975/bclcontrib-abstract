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

        /// <summary>
        /// Initializes a new instance of the <see cref="StdParams"/> class.
        /// </summary>
        public StdParams()
        {
            _comparerField.SetValue(_values, StringComparer.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="StdParams"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        public StdParams(INparams args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            _comparerField.SetValue(_values, StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, object> pair in args)
                Add(pair.Key, pair.Value);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="StdParams"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        public StdParams(IDictionary<string, object> args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            _comparerField.SetValue(_values, StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, object> pair in args)
                Add(pair.Key, pair.Value);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="StdParams"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        public StdParams(object args)
        {
            _comparerField.SetValue(_values, StringComparer.OrdinalIgnoreCase);
            AddValues(args);
        }

        #region Interface

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentException">
        /// An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///   </exception>
        public void Add(string key, object value) { _values.Add(key, value); }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///   </exception>
        public bool ContainsKey(string key) { return _values.ContainsKey(key); }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///   </returns>
        public ICollection<string> Keys
        {
            get { return _values.Keys; }
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///   </exception>
        public bool Remove(string key) { return _values.Remove(key); }
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///   </exception>
        public bool TryGetValue(string key, out object value) { return _values.TryGetValue(key, out value); }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///   </returns>
        public ICollection<object> Values
        {
            get { return _values.Values; }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        ///   </returns>
        ///   
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        /// The property is retrieved and <paramref name="key"/> is not found.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///   </exception>
        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///   </exception>
        public void Clear() { _values.Clear(); }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///   </returns>
        public int Count
        {
            get { return _values.Count; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() { return _values.GetEnumerator(); }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        ///   </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() { return ((INparams)this).GetEnumerator(); }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///   </exception>
        public void Add(KeyValuePair<string, object> item) { throw new NotImplementedException(); }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<string, object> item) { throw new NotImplementedException(); }
        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) { throw new NotImplementedException(); }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///   </exception>
        public bool Remove(KeyValuePair<string, object> item) { throw new NotImplementedException(); }

        // added
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="args">The args.</param>
        public void AddRange(IDictionary<string, object> args)
        {
            if (args != null)
                foreach (var pair in args)
                    Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Toes the string array.
        /// </summary>
        /// <returns></returns>
        public string[] ToStringArray()
        {
            return _values.Select(x =>
            {
                return x.Key + "=" + x.Value.ToString();
            }).ToArray();
        }

        /// <summary>
        /// Slices the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
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
