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
namespace System
{
    /// <summary>
    /// NparamsExtensions
    /// </summary>
    public static class NparamsExtensions
    {
        /// <summary>
        /// Adds the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Add(this Nparams source, string key, object value) { if (source == null) throw new ArgumentNullException("source"); source._base.Add(key, value); }
        /// <summary>
        /// Determines whether the specified source contains key.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified source contains key; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsKey(this Nparams source, string key) { return (source == null ? false : source._base.ContainsKey(key)); }
        /// <summary>
        /// Removes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static bool Remove(this Nparams source, string key) { return (source == null ? false : source._base.Remove(key)); }
        /// <summary>
        /// Determines whether the specified source has value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <c>true</c> if the specified source has value; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasValue(this Nparams source) { return (source == null ? false : source._base.Count > 0); }
        /// <summary>
        /// Counts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static int Count(this Nparams source) { return (source == null ? 0 : source._base.Count); }
        /// <summary>
        /// Keyses the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<string> Keys(this Nparams source) { return (source == null ? Enumerable.Empty<string>() : source._base.Keys); }
        /// <summary>
        /// Valueses the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<T> Values<T>(this Nparams source) { return (source == null ? Enumerable.Empty<T>() : source._base.Values.Cast<T>()); }
        /// <summary>
        /// Valueses the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<object> Values(this Nparams source) { return (source == null ? Enumerable.Empty<object>() : source._base.Values); }
        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this Nparams source, string key, out T value)
        {
            if (source == null) { value = default(T); return false; }
            object valueAsObject;
            var r = source._base.TryGetValue(key, out valueAsObject);
            value = (T)valueAsObject;
            return r;
        }
        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryGetValue(this Nparams source, string key, out object value)
        {
            if (source == null) { value = null; return false; }
            return source._base.TryGetValue(key, out value);
        }
        /// <summary>
        /// Gets the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static T Get<T>(this Nparams source) { return (source == null ? default(T) : (T)source._base[typeof(T).Name]); }
        /// <summary>
        /// Gets the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Get<T>(this Nparams source, string key) { return (source == null ? default(T) : (T)source._base[key]); }
        /// <summary>
        /// Gets the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T Get<T>(this Nparams source, string key, T defaultValue) { return (source == null ? defaultValue : (T)source._base[key]); }
        /// <summary>
        /// Gets the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static object Get(this Nparams source, string key, object defaultValue) { return (source == null ? defaultValue : source._base[key]); }
        /// <summary>
        /// Gets the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object Get(this Nparams source, string key) { return (source == null ? null : source._base[key]); }
        /// <summary>
        /// Sets the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        public static void Set<T>(this Nparams source, T value) { if (source == null) throw new ArgumentNullException("source"); source._base[typeof(T).Name] = value; }
        /// <summary>
        /// Sets the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Set<T>(this Nparams source, string key, T value) { if (source == null) throw new ArgumentNullException("source"); source._base[key] = value; }
        /// <summary>
        /// Sets the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Set(this Nparams source, string key, object value) { if (source == null) throw new ArgumentNullException("source"); source._base[key] = value; }
        /// <summary>
        /// Clears the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Clear(this Nparams source) { if (source != null) source._base.Clear(); }

        // extensions
        /// <summary>
        /// Toes the dictionary.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this Nparams source) { return (source == null ? new Dictionary<string, object>() : (IDictionary<string, object>)source._base); }
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The args.</param>
        public static void AddRange(this Nparams source, Nparams args) { if (source == null) throw new ArgumentNullException("source"); if (args != null) source._base.AddRange(args._base); }
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The args.</param>
        public static void AddRange(this Nparams source, IDictionary<string, object> args) { if (source == null) throw new ArgumentNullException("source"); source._base.AddRange(args); }
        /// <summary>
        /// Toes the string array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string[] ToStringArray(this Nparams source) { return (source == null ? new[] { string.Empty } : source._base.ToStringArray()); }
        /// <summary>
        /// Slices the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Slice<T>(this Nparams source, string key) { return (source == null ? default(T) : source._base.Slice<T>(key, default(T))); }
        /// <summary>
        /// Slices the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T Slice<T>(this Nparams source, string key, T defaultValue) { return (source == null ? default(T) : source._base.Slice<T>(key, defaultValue)); }
    }
}
