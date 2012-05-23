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
using System.IO;
namespace Contoso.Abstract.Parts.X.Internal
{
    internal class GenericJsonSerializerAdapter<T> : JsonSerializer<T>
        //where T : new()
    {
        private JsonSerializer _innerSerializer;

        public GenericJsonSerializerAdapter(JsonSerializer innerSerializer)
            : base(false) { _innerSerializer = innerSerializer; }

        public override JavascriptType SerializerType
        {
            get { return _innerSerializer.SerializerType; }
        }

        public override string DefaultFormat
        {
            get { return _innerSerializer.DefaultFormat; }
        }

        public override T Deserialize(TextReader reader) { return (T)Convert.ChangeType(_innerSerializer.BaseDeserialize(reader), typeof(T)); }

        internal override void Serialize(TextWriter writer, T obj, JsonOptions options, string format, int tabDepth) { _innerSerializer.BaseSerialize(writer, (Object)obj, options, format, tabDepth); }
    }
}
