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
using System.IO;
using System.Text;
using System;
namespace Contoso.Abstract.Micro.Internal
{
    internal class JsonStringSerializer : JsonSerializer
    {
        public JsonStringSerializer()
            : base(JsonValueType.String, null) { }
        public JsonStringSerializer(string defaultFormat)
            : base(JsonValueType.String, defaultFormat) { }

        internal override object BaseDeserialize(TextReader r, string path)
        {
            if (JsonParserUtil.PeekIsNull(r, true, path))
                return null;
            var c = JsonParserUtil.ReadNextChar(r, true);
            if (c != '"')
                throw new JsonDeserializationException(string.Format("Expected '\"' at '{0}'", path));
            var escape = false;
            c = JsonParserUtil.ReadNextChar(r, true);
            var b = new StringBuilder();
            while (c != '"' || escape)
            {
                if (escape)
                    switch (c)
                    {
                        case 'b': c = '\b'; break;
                        case 'f': c = '\f'; break;
                        case 'n': c = '\n'; break;
                        case 'r': c = '\r'; break;
                        case 't': c = '\t'; break;
                        case 'v': c = '\v'; break;
                        case 'x': c = JsonParserUtil.ReadCharCode(r, 16, 2, 2); break;
                        case 'u': c = JsonParserUtil.ReadCharCode(r, 16, 4, 4); break;
                    }
                escape = !escape && (c == '\\');
                if (!escape)
                    b.Append(c);
                else if (char.IsDigit(JsonParserUtil.PeekNextChar(r, false)))
                    b.Append(JsonParserUtil.ReadCharCode(r, 8, 1, 3));
                c = JsonParserUtil.ReadNextChar(r, false);
            }
            return b.ToString();
        }

        internal override void BaseSerialize(TextWriter w, object obj, JsonOptions options, string format, int tabDepth)
        {
            if (obj != null)
            {
                if (string.IsNullOrEmpty(format))
                    format = DefaultFormat;
                var serialized = (string.IsNullOrEmpty(format) ? string.Format("{0}", obj) : string.Format("{0:" + format + "}", obj));
                w.Write('"');
                w.Write(serialized.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\n", "\\n"));
                w.Write('"');
            }
            else
                w.Write("null");
        }
    }
}
