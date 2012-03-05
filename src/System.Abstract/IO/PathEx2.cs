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
    /// PathEx2
    /// </summary>
    public static partial class PathEx2
    {
        [ThreadStatic]
        private static MockBase _mock;

        public static MockBase Mock
        {
            get { return _mock; }
            set { _mock = value; }
        }

        public static string ChangeExtension(string path, string extension) { return _mock.ChangeExtension(path, extension); }
        public static string Combine(string path1, string path2) { return _mock.Combine(path1, path2); }
        public static string GetDirectoryName(string path) { return _mock.GetDirectoryName(path); }
        public static string GetExtension(string path) { return _mock.GetExtension(path); }
        public static string GetFileName(string path) { return _mock.GetFileName(path); }
        public static string GetFileNameWithoutExtension(string path) { return _mock.GetFileNameWithoutExtension(path); }
        public static string GetFullPath(string path) { return _mock.GetFullPath(path); }
        public static char[] GetInvalidFileNameChars() { return _mock.GetInvalidFileNameChars(); }
        public static char[] GetInvalidPathChars() { return _mock.GetInvalidPathChars(); }
        public static string GetPathRoot(string path) { return _mock.GetPathRoot(path); }
        public static string GetRandomFileName() { return _mock.GetRandomFileName(); }
        public static string GetTempFileName() { return _mock.GetTempFileName(); }
        public static string GetTempPath() { return _mock.GetTempPath(); }
        public static bool HasExtension(string path) { return _mock.HasExtension(path); }
        public static bool IsPathRooted(string path) { return _mock.IsPathRooted(path); }
    }
}
