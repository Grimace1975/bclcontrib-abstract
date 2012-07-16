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
using System.Text;
namespace Contoso.Abstract.Micro.Internal
{
    internal static class JsonParserUtil
    {
        public static readonly char[] _radixTable = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        public static int ReadStartObject(TextReader r)
        {
            var parens = ReadNOptional(r, 0, true, '(');
            ReadExpected(r, true, '{');
            return parens;
        }

        public static int ReadStartArray(TextReader r)
        {
            var parens = ReadNOptional(r, 0, true, '(');
            ReadExpected(r, true, '[');
            return parens;
        }

        public static string ReadMemberName(TextReader r)
        {
            var b = new StringBuilder();
            var quoted = false;
            var c = ReadNextChar(r, true);
            if (c == '"')
                quoted = true;
            else
                b.Append(c);
            c = ReadNextChar(r, false);
            if (quoted)
            {
                var escaped = false;
                while (c != '"' || escaped)
                {
                    escaped = !escaped && (c == '\\');
                    b.Append(c);
                    c = ReadNextChar(r, false);
                }
                if (ReadNextChar(r, false) != ':')
                    throw new JsonDeserializationException("Expected ':'");
            }
            else
                while (c != ':')
                {
                    b.Append(c);
                    c = ReadNextChar(r, false);
                }
            return b.ToString();
        }

        public static void ReadEndObject(TextReader r, int parens)
        {
            ReadExpected(r, true, '}');
            if (ReadNOptional(r, 0, true, ')') != parens)
                throw new JsonDeserializationException("Expected ')'");
        }

        public static void ReadEndArray(TextReader r, int parens)
        {
            ReadExpected(r, true, ']');
            if (ReadNOptional(r, 0, true, ')') != parens)
                throw new JsonDeserializationException("Expected ')'");
        }

        public static char ReadNextChar(TextReader r, bool ignoreWhitespace)
        {
            var c = (char)r.Read();
            while (ignoreWhitespace && char.IsWhiteSpace(c))
                c = (char)r.Read();
            return c;
        }

        public static void ReadExpected(TextReader r, bool ignoreWhitespace, char expected)
        {
            var c = ReadNextChar(r, ignoreWhitespace);
            if (c != expected)
                throw new JsonDeserializationException(string.Format("Expected '{0}'", expected));
        }

        public static int ReadNOptional(TextReader r, int max, bool ignoreWhitespace, char optional)
        {
            var count = 0;
            var c = PeekNextChar(r, ignoreWhitespace);
            while (c == optional && (count < max || max == 0))
            {
                count++;
                ReadNextChar(r, ignoreWhitespace);
                c = PeekNextChar(r, ignoreWhitespace);
            }
            return count;
        }

        public static char PeekNextChar(TextReader r, bool ignoreWhitespace)
        {
            var c = (char)r.Peek();
            while (ignoreWhitespace && char.IsWhiteSpace(c))
            {
                r.Read();
                c = (char)r.Peek();
            }
            return c;
        }

        public static void SkipWhitespace(TextReader r)
        {
            var c = (char)r.Peek();
            while (char.IsWhiteSpace(c))
            {
                r.Read();
                c = (char)r.Peek();
            }
        }

        public static string GetNextToken(TextReader r)
        {
            var b = new StringBuilder();
            SkipWhitespace(r);
            var c = PeekNextChar(r, false);
            while (c != ',' && c != '}' && c != ';' && c != ']' && c != ')' && !char.IsWhiteSpace(c))
            {
                b.Append(c);
                ReadNextChar(r, false);
                c = PeekNextChar(r, false);
            }
            return b.ToString();
        }

        public static char ReadCharCode(TextReader r, int radix, int minDigits, int maxDigits)
        {
            var digits = new int[maxDigits];
            var digitCount = 0;
            while (digitCount < maxDigits)
            {
                var c = PeekNextChar(r, false);
                var val = Array.IndexOf(_radixTable, char.ToLowerInvariant(c));
                if (val == -1 || val >= radix)
                {
                    if (digitCount < minDigits)
                        throw new JsonDeserializationException("Expected Digit");
                    else
                        break;
                }
                else
                {
                    digits[digitCount] = val;
                    digitCount++;
                    ReadNextChar(r, false);
                }
            }
            var charCode = 0;
            var multiplier = 1;
            for (var i = digitCount - 1; i >= 0; i--, multiplier *= radix)
                charCode += digits[i] * multiplier;
            return (char)charCode;
        }

        public static bool PeekIsNull(TextReader r, bool ignoreWhitespace, string path)
        {
            var c = JsonParserUtil.PeekNextChar(r, ignoreWhitespace);
            if (char.ToLowerInvariant(c) == 'n')
            {
                r.Read();
                if (char.ToLowerInvariant((char)r.Read()) == 'u' && char.ToLowerInvariant((char)r.Read()) == 'l' && char.ToLowerInvariant((char)r.Read()) == 'l')
                    return true;
                throw new JsonDeserializationException(string.Format("Expected 'null' at '{0}'", path));
            }
            return false;
        }
    }
}
