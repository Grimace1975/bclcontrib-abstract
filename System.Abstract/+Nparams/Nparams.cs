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
    public abstract class Nparams //: IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        public static Nparams Parse(Nparams args) { return (args != null ? args : null); }
        public static Nparams Parse(string[] args) { return (args != null ? (Nparams)null : null); }
        public static Nparams Parse(object args) { return (args != null ? (Nparams)null : null); }
        //
        public abstract string[] ToStringArray();
        public abstract int Count { get; }
        public abstract IEnumerable<string> Names { get; }
        public abstract bool Exists(string name);
        public abstract T Slice<T>(string name, T defaultValue);
        public abstract T Value<T>(string name, T defaultValue);
    }

    /// <summary>
    /// NparamsExtensions
    /// </summary>
    public static class NparamsExtensions
    {
        public static T Slice<T>(this Nparams param, string name) { return param.Slice<T>(name, default(T)); }
        public static T Value<T>(this Nparams param, string name) { return param.Value<T>(name, default(T)); }
        public static T Get<T>(this Nparams param)
        {
            return default(T);
        }
    }
}
