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
namespace System.IO
{
    /// <summary>
    /// WrapTextReader
    /// </summary>
#if !COREINTERNAL
    public
#endif
 class WrapTextReader : TextReader
    {
        public enum WrapOptions
        {
            Default = 0,
        }

        protected WrapTextReader(TextReader r, WrapOptions options)
        {
            R = r;
            Options = options;
        }

        protected TextReader R { get; set; }
        protected WrapOptions Options { get; set; }

        public static TextReader Wrap(TextReader r) { return Wrap(r, WrapOptions.Default); }
        public static TextReader Wrap(TextReader r, WrapOptions options)
        {
            var rAsWrap = (r as WrapTextReader);
            return (rAsWrap != null && rAsWrap.Options == options ? r : new WrapTextReader(r, options));
        }

        public override void Close() { R.Close(); }
        protected override void Dispose(bool disposing) { if (disposing) R.Dispose(); }
        public override bool Equals(object obj) { return R.Equals(obj); }
        public override int GetHashCode() { return R.GetHashCode(); }

        private int? _pendingC;

        public override int Peek()
        {
            var c = R.Peek();
            if (c == -1)
                _pendingC = (int?)(c = R.Read());
            return c;
        }

        public override int Read()
        {
            if (!_pendingC.HasValue)
                return R.Read();
            var c = _pendingC.Value; _pendingC = null; return c;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", EnvironmentEx2.GetResourceString("ArgumentNull_Buffer"));
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", EnvironmentEx2.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", EnvironmentEx2.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (buffer.Length - index < count)
                throw new ArgumentException(EnvironmentEx2.GetResourceString("Argument_InvalidOffLen"));
            if (!_pendingC.HasValue)
                return R.Read(buffer, index, count);
            var c = _pendingC.Value; _pendingC = null;
            if (c == -1)
                return 0;
            buffer[index] = (char)c;
            return (true ? R.Read(buffer, index + 1, count - 1) + 1 : 1);
        }
    }
}
