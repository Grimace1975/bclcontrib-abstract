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
        /// DirectoryWrapper
        /// </summary>
        public class DirectoryWrapper : MockBase
        {
            /// <summary>
            /// Creates the directory.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DirectoryInfo CreateDirectory(string path) { return Directory.CreateDirectory(path); }
            /// <summary>
            /// Creates the directory.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="directorySecurity">The directory security.</param>
            /// <returns></returns>
            public override DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity) { return Directory.CreateDirectory(path, directorySecurity); }
            /// <summary>
            /// Deletes the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            public override void Delete(string path) { Directory.Delete(path); }
            /// <summary>
            /// Deletes the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="recursive">if set to <c>true</c> [recursive].</param>
            public override void Delete(string path, bool recursive) { Directory.Delete(path, recursive); }
            /// <summary>
            /// Existses the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override bool Exists(string path) { return Directory.Exists(path); }
            /// <summary>
            /// Gets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DirectorySecurity GetAccessControl(string path) { return Directory.GetAccessControl(path); }
            /// <summary>
            /// Gets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="includeSections">The include sections.</param>
            /// <returns></returns>
            public override DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections) { return Directory.GetAccessControl(path, includeSections); }
            /// <summary>
            /// Gets the creation time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetCreationTime(string path) { return Directory.GetCreationTime(path); }
            /// <summary>
            /// Gets the creation time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetCreationTimeUtc(string path) { return Directory.GetCreationTimeUtc(path); }
            /// <summary>
            /// Gets the current directory.
            /// </summary>
            /// <returns></returns>
            public override string GetCurrentDirectory() { return Directory.GetCurrentDirectory(); }
            /// <summary>
            /// Gets the directories.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string[] GetDirectories(string path) { return Directory.GetDirectories(path); }
            /// <summary>
            /// Gets the directories.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <returns></returns>
            public override string[] GetDirectories(string path, string searchPattern) { return Directory.GetDirectories(path, searchPattern); }
            /// <summary>
            /// Gets the directories.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <param name="searchOption">The search option.</param>
            /// <returns></returns>
            public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) { return Directory.GetDirectories(path, searchPattern, searchOption); }
            /// <summary>
            /// Gets the directory root.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string GetDirectoryRoot(string path) { return Directory.GetDirectoryRoot(path); }
            /// <summary>
            /// Gets the files.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string[] GetFiles(string path) { return Directory.GetFiles(path); }
            /// <summary>
            /// Gets the files.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <returns></returns>
            public override string[] GetFiles(string path, string searchPattern) { return Directory.GetFiles(path, searchPattern); }
            /// <summary>
            /// Gets the files.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <param name="searchOption">The search option.</param>
            /// <returns></returns>
            public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption) { return Directory.GetFiles(path, searchPattern, searchOption); }
            /// <summary>
            /// Gets the file system entries.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string[] GetFileSystemEntries(string path) { return Directory.GetFileSystemEntries(path); }
            /// <summary>
            /// Gets the file system entries.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchPattern">The search pattern.</param>
            /// <returns></returns>
            public override string[] GetFileSystemEntries(string path, string searchPattern) { return Directory.GetFileSystemEntries(path, searchPattern); }
            /// <summary>
            /// Gets the last access time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetLastAccessTime(string path) { return Directory.GetLastAccessTime(path); }
            /// <summary>
            /// Gets the last access time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetLastAccessTimeUtc(string path) { return Directory.GetLastAccessTimeUtc(path); }
            /// <summary>
            /// Gets the last write time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetLastWriteTime(string path) { return Directory.GetLastWriteTime(path); }
            /// <summary>
            /// Gets the last write time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetLastWriteTimeUtc(string path) { return Directory.GetLastWriteTimeUtc(path); }
            /// <summary>
            /// Gets the logical drives.
            /// </summary>
            /// <returns></returns>
            public override string[] GetLogicalDrives() { return Directory.GetLogicalDrives(); }
            /// <summary>
            /// Gets the parent.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DirectoryInfo GetParent(string path) { return Directory.GetParent(path); }
            /// <summary>
            /// Moves the specified source dir name.
            /// </summary>
            /// <param name="sourceDirName">Name of the source dir.</param>
            /// <param name="destDirName">Name of the dest dir.</param>
            public override void Move(string sourceDirName, string destDirName) { Directory.Move(sourceDirName, destDirName); }
            /// <summary>
            /// Sets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="directorySecurity">The directory security.</param>
            public override void SetAccessControl(string path, DirectorySecurity directorySecurity) { Directory.SetAccessControl(path, directorySecurity); }
            /// <summary>
            /// Sets the creation time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="creationTime">The creation time.</param>
            public override void SetCreationTime(string path, DateTime creationTime) { Directory.SetCreationTime(path, creationTime); }
            /// <summary>
            /// Sets the creation time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="creationTimeUtc">The creation time UTC.</param>
            public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc) { Directory.SetCreationTimeUtc(path, creationTimeUtc); }
            /// <summary>
            /// Sets the current directory.
            /// </summary>
            /// <param name="path">The path.</param>
            public override void SetCurrentDirectory(string path) { Directory.SetCurrentDirectory(path); }
            /// <summary>
            /// Sets the last access time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastAccessTime">The last access time.</param>
            public override void SetLastAccessTime(string path, DateTime lastAccessTime) { Directory.SetLastAccessTime(path, lastAccessTime); }
            /// <summary>
            /// Sets the last access time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastAccessTimeUtc">The last access time UTC.</param>
            public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) { Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc); }
            /// <summary>
            /// Sets the last write time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastWriteTime">The last write time.</param>
            public override void SetLastWriteTime(string path, DateTime lastWriteTime) { Directory.SetLastWriteTime(path, lastWriteTime); }
            /// <summary>
            /// Sets the last write time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastWriteTimeUtc">The last write time UTC.</param>
            public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) { Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc); }
        }
    }
}
