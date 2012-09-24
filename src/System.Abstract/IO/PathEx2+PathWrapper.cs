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
            /// <summary>
            /// Changes the extension.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="extension">The extension.</param>
            /// <returns></returns>
            public override string ChangeExtension(string path, string extension) { return Path.ChangeExtension(path, extension); }
            /// <summary>
            /// Combines the specified path1.
            /// </summary>
            /// <param name="path1">The path1.</param>
            /// <param name="path2">The path2.</param>
            /// <returns></returns>
            public override string Combine(string path1, string path2) { return Path.Combine(path1, path2); }
            /// <summary>
            /// Gets the name of the directory.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string GetDirectoryName(string path) { return Path.GetDirectoryName(path); }
            /// <summary>
            /// Gets the extension.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string GetExtension(string path) { return Path.GetExtension(path); }
            /// <summary>
            /// Gets the name of the file.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string GetFileName(string path) { return Path.GetFileName(path); }
            /// <summary>
            /// Gets the file name without extension.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string GetFileNameWithoutExtension(string path) { return Path.GetFileNameWithoutExtension(path); }
            /// <summary>
            /// Gets the full path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string GetFullPath(string path) { return Path.GetFullPath(path); }
            /// <summary>
            /// Gets the invalid file name chars.
            /// </summary>
            /// <returns></returns>
            public override char[] GetInvalidFileNameChars() { return Path.GetInvalidFileNameChars(); }
            /// <summary>
            /// Gets the invalid path chars.
            /// </summary>
            /// <returns></returns>
            public override char[] GetInvalidPathChars() { return Path.GetInvalidPathChars(); }
            /// <summary>
            /// Gets the path root.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string GetPathRoot(string path) { return Path.GetPathRoot(path); }
            /// <summary>
            /// Gets the random name of the file.
            /// </summary>
            /// <returns></returns>
            public override string GetRandomFileName() { return Path.GetRandomFileName(); }
            /// <summary>
            /// Gets the name of the temp file.
            /// </summary>
            /// <returns></returns>
            public override string GetTempFileName() { return Path.GetTempFileName(); }
            /// <summary>
            /// Gets the temp path.
            /// </summary>
            /// <returns></returns>
            public override string GetTempPath() { return Path.GetTempPath(); }
            /// <summary>
            /// Determines whether the specified path has extension.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>
            ///   <c>true</c> if the specified path has extension; otherwise, <c>false</c>.
            /// </returns>
            public override bool HasExtension(string path) { return Path.HasExtension(path); }
            /// <summary>
            /// Determines whether [is path rooted] [the specified path].
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>
            ///   <c>true</c> if [is path rooted] [the specified path]; otherwise, <c>false</c>.
            /// </returns>
            public override bool IsPathRooted(string path) { return Path.IsPathRooted(path); }
        }
    }
}
