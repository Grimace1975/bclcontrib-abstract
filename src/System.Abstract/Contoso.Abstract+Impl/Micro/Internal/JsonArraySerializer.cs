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
using System.Collections.Generic;
using System.IO;
namespace Contoso.Abstract.Micro.Internal
{
    internal class JsonArraySerializer<TEnumerable, TElement> : JsonSerializer
        where TEnumerable : IEnumerable<TElement>
    {
        private JsonSerializer<TElement> _elementSerializer = JsonSerializer<TElement>.CreateSerializer();

        public JsonArraySerializer()
            : base(JsonValueType.Array, null) { }

        internal override object BaseDeserialize(TextReader r, string path)
        {
            if (JsonParserUtil.PeekIsNull(r, true, path))
                return null;
            var result = new List<TElement>();
            var parens = JsonParserUtil.ReadStartArray(r);
            var c = JsonParserUtil.PeekNextChar(r, true);
            while (c != ']')
            {
                result.Add(_elementSerializer.Deserialize(r, path));
                c = JsonParserUtil.PeekNextChar(r, true);
                if (c != ',' && c != ']')
                    throw new JsonDeserializationException(string.Format("Expected ']' at '{0}'", path));
                else if (c == ',')
                    JsonParserUtil.ReadNextChar(r, true);
            }
            JsonParserUtil.ReadEndArray(r, parens);
            return (typeof(TEnumerable).IsArray ? result.ToArray() : Activator.CreateInstance(typeof(TEnumerable), result));
        }

        internal override void BaseSerialize(TextWriter w, object obj, JsonOptions options, string format, int tabDepth)
        {
            if (obj != null)
            {
                if ((options & JsonOptions.EnclosingParens) != 0)
                    w.Write('(');
                w.Write('[');
                var first = true;
                foreach (TElement element in (IEnumerable<TElement>)obj)
                {
                    if (!first)
                        w.Write(',');
                    first = false;
                    if ((options & JsonOptions.Formatted) != 0)
                    {
                        w.WriteLine();
                        w.Write(new String(' ', tabDepth * 2));
                    }
                    _elementSerializer.Serialize(w, element, options, null, tabDepth + 1);
                }
                w.Write(']');
                if ((options & JsonOptions.EnclosingParens) != 0)
                    w.Write(')');
            }
            else
                w.Write("null");
        }
    }
}
