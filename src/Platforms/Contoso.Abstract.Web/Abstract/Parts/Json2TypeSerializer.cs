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
using System;
using System.Abstract.Parts;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
namespace Contoso.Abstract.Parts
{
    public class Json2TypeSerializer : ITypeSerializer
    {
        public T ReadObject<T>(Type type, Stream s)
            where T : class
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (s == null)
                throw new ArgumentNullException("s");
            var serializer = new JavaScriptSerializer();
            var r = new BinaryReader(s);
            return serializer.Deserialize<T>(r.ReadString());
        }

        public IEnumerable<T> ReadObjects<T>(Type type, Stream s)
            where T : class
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (s == null)
                throw new ArgumentNullException("s");
            var serializer = new JavaScriptSerializer();
            var r = new BinaryReader(s);
            return serializer.Deserialize<IEnumerable<T>>(r.ReadString());
        }

        public void WriteObject<T>(Type type, Stream s, T graph)
            where T : class
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (s == null)
                throw new ArgumentNullException("s");
            if (graph == null)
                throw new ArgumentNullException("graph");
            var serializer = new JavaScriptSerializer();
            var value = serializer.Serialize(graph);
            var w = new StreamWriter(s);
            w.Write(value.Length);
            w.Write(value);
        }

        public void WriteObjects<T>(Type type, Stream s, IEnumerable<T> graphs)
            where T : class
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (s == null)
                throw new ArgumentNullException("s");
            if (graphs == null)
                throw new ArgumentNullException("graphs");
            var serializer = new JavaScriptSerializer();
            var value = serializer.Serialize(graphs);
            var w = new StreamWriter(s);
            w.Write(value.Length);
            w.Write(value);
        }
    }
}