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
using System.Abstract.Parts;
using System.IO;
using System.Text;
namespace System.Abstract.Parts
{
    public static partial class PartsExtensions
    {
        public static T ReadObject<T>(this ITypeSerializer serializer, Type type, string text)
            where T : class { return ReadObject<T>(serializer, type, text, Encoding.Default); }
        public static T ReadObject<T>(this ITypeSerializer serializer, Type type, string text, Encoding encoding)
            where T : class
        {
            using (var s = new MemoryStream(encoding.GetBytes(text)))
                return serializer.ReadObject<T>(type, s);
        }
        public static T ReadObjectBase64<T>(this ITypeSerializer serializer, Type type, string text)
            where T : class
        {
            using (var s = new MemoryStream(Convert.FromBase64String(text)))
                return serializer.ReadObject<T>(type, s);
        }

        public static string WriteObject<T>(this ITypeSerializer serializer, Type type, T graph)
            where T : class { return WriteObject<T>(serializer, type, graph, Encoding.Default); }
        public static string WriteObject<T>(this ITypeSerializer serializer, Type type, T graph, Encoding encoding)
            where T : class
        {
            using (var s = new MemoryStream())
            {
                serializer.WriteObject<T>(type, s, graph);
                s.Flush(); s.Position = 0;
                return encoding.GetString(s.ToArray());
            }
        }
        public static string WriteObjectBase64<T>(this ITypeSerializer serializer, Type type, T graph)
            where T : class
        {
            using (var s = new MemoryStream())
            {
                serializer.WriteObject<T>(type, s, graph);
                s.Flush(); s.Position = 0;
                return Convert.ToBase64String(s.ToArray());
            }
        }
    }
}
