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
    public static partial class PathEx2
    {
        /// <summary>
        /// PathWrapper
        /// </summary>
        public class PathWrapper : MockBase
        {
            public virtual string ChangeExtension(string path, string extension) { return Path.ChangeExtension(path, extension); }
            public virtual string Combine(string path1, string path2) { return Path.Combine(path1, path2); }
            public virtual string GetDirectoryName(string path) { return Path.GetDirectoryName(path); }
            public virtual string GetExtension(string path) { return Path.GetExtension(path); }
            public virtual string GetFileName(string path) { return Path.GetFileName(path); }
            public virtual string GetFileNameWithoutExtension(string path) { return Path.GetFileNameWithoutExtension(path); }
            public virtual string GetFullPath(string path) { return Path.GetFullPath(path); }
            public virtual char[] GetInvalidFileNameChars() { return Path.GetInvalidFileNameChars(); }
            public virtual char[] GetInvalidPathChars() { return Path.GetInvalidPathChars(); }
            public virtual string GetPathRoot(string path) { return Path.GetPathRoot(path); }
            public virtual string GetRandomFileName() { return Path.GetRandomFileName(); }
            public virtual string GetTempFileName() { return Path.GetTempFileName(); }
            public virtual string GetTempPath() { return Path.GetTempPath(); }
            public virtual bool HasExtension(string path) { return Path.HasExtension(path); }
            public virtual bool IsPathRooted(string path) { return Path.IsPathRooted(path); }
        }
    }
}
