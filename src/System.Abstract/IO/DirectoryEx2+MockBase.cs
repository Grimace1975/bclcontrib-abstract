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
using System.Security.AccessControl;
namespace System.IO
{
    public static partial class DirectoryEx2
    {
        /// <summary>
        /// MockBase
        /// </summary>
        public abstract class MockBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockBase"/> class.
            /// </summary>
            protected MockBase() { }

            /// <summary>
            /// Creates the directory.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DirectoryInfo CreateDirectory(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Creates the directory.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="directorySecurity">The directory security.</param>
            /// <returns></returns>
            public virtual DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity) { throw new NotImplementedException(); }
            /// <summary>
            /// Deletes the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            public virtual void Delete(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Deletes the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="recursive">if set to <c>true</c> [recursive].</param>
            public virtual void Delete(string path, bool recursive) { throw new NotImplementedException(); }
            /// <summary>
            /// Existses the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual bool Exists(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DirectorySecurity GetAccessControl(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="includeSections">The include sections.</param>
            /// <returns></returns>
            public virtual DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the creation time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DateTime GetCreationTime(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the creation time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DateTime GetCreationTimeUtc(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the current directory.
            /// </summary>
            /// <returns></returns>
            public virtual string GetCurrentDirectory() { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the directories.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual string[] GetDirectories(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the directories.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <returns></returns>
            public virtual string[] GetDirectories(string path, string searchPattern) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the directories.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <param name="searchOption">The search option.</param>
            /// <returns></returns>
            public virtual string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the directory root.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual string GetDirectoryRoot(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the files.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual string[] GetFiles(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the files.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <returns></returns>
            public virtual string[] GetFiles(string path, string searchPattern) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the files.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <param name="searchOption">The search option.</param>
            /// <returns></returns>
            public virtual string[] GetFiles(string path, string searchPattern, SearchOption searchOption) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the file system entries.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual string[] GetFileSystemEntries(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the file system entries.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <returns></returns>
            public virtual string[] GetFileSystemEntries(string path, string searchPattern) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the last access time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DateTime GetLastAccessTime(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the last access time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DateTime GetLastAccessTimeUtc(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the last write time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DateTime GetLastWriteTime(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the last write time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DateTime GetLastWriteTimeUtc(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the logical drives.
            /// </summary>
            /// <returns></returns>
            public virtual string[] GetLogicalDrives() { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the parent.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual DirectoryInfo GetParent(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Moves the specified source dir name.
            /// </summary>
            /// <param name="sourceDirName">Name of the source dir.</param>
            /// <param name="destDirName">Name of the dest dir.</param>
            public virtual void Move(string sourceDirName, string destDirName) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="directorySecurity">The directory security.</param>
            public virtual void SetAccessControl(string path, DirectorySecurity directorySecurity) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the creation time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="creationTime">The creation time.</param>
            public virtual void SetCreationTime(string path, DateTime creationTime) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the creation time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="creationTimeUtc">The creation time UTC.</param>
            public virtual void SetCreationTimeUtc(string path, DateTime creationTimeUtc) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the current directory.
            /// </summary>
            /// <param name="path">The path.</param>
            public virtual void SetCurrentDirectory(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the last access time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastAccessTime">The last access time.</param>
            public virtual void SetLastAccessTime(string path, DateTime lastAccessTime) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the last access time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastAccessTimeUtc">The last access time UTC.</param>
            public virtual void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the last write time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastWriteTime">The last write time.</param>
            public virtual void SetLastWriteTime(string path, DateTime lastWriteTime) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the last write time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastWriteTimeUtc">The last write time UTC.</param>
            public virtual void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) { throw new NotImplementedException(); }
        }
    }
}
