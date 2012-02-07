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
    /// NparamsExtensions
    /// </summary>
    public static class NparamsExtensions
    {
        public static void Add(this Nparams source, string key, object value) { if (source == null) throw new ArgumentNullException("source"); source._base.Add(key, value); }
        public static bool ContainsKey(this Nparams source, string key) { return (source == null ? false : source._base.ContainsKey(key)); }
        public static bool Remove(this Nparams source, string key) { return (source == null ? false : source._base.Remove(key)); }
        public static bool TryGetValue<T>(this Nparams source, string key, out T value)
        {
            if (source == null) { value = default(T); return false; }
            object valueAsObject;
            var r = source._base.TryGetValue(key, out valueAsObject);
            value = (T)valueAsObject;
            return r;
        }
        public static bool TryGetValue(this Nparams source, string key, out object value)
        {
            if (source == null) { value = null; return false; }
            return source._base.TryGetValue(key, out value);
        }
        public static T Get<T>(this Nparams source) { return (source == null ? default(T) : (T)source._base[typeof(T).Name]); }
        public static T Get<T>(this Nparams source, string key) { return (source == null ? default(T) : (T)source._base[key]); }
        public static object Get(this Nparams source, string key) { return (source == null ? null : source._base[key]); }
        public static void Set<T>(this Nparams source, T value) { if (source == null) throw new ArgumentNullException("source"); source._base[typeof(T).Name] = value; }
        public static void Set<T>(this Nparams source, string key, T value) { if (source == null) throw new ArgumentNullException("source"); source._base[key] = value; }
        public static void Set(this Nparams source, string key, object value) { if (source == null) throw new ArgumentNullException("source"); source._base[key] = value; }
        public static void Clear(this Nparams source) { if (source != null) source._base.Clear(); }

        // extensions
        public static void AddRange(this Nparams source, IDictionary<string, object> dictionary) { if (source == null) throw new ArgumentNullException("source"); source._base.AddRange(dictionary); }
        public static string[] ToStringArray(this Nparams source) { return (source == null ? new[] { string.Empty } : source._base.ToStringArray()); }
        public static T Slice<T>(this Nparams source, string key) { return (source == null ? default(T) : source._base.Slice<T>(key, default(T))); }
        public static T Slice<T>(this Nparams source, string key, T defaultValue) { return (source == null ? default(T) : source._base.Slice<T>(key, defaultValue)); }
    }
}
