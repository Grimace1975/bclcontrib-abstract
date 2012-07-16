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
    /// <summary>
    /// DirectoryEx2
    /// </summary>
    public static partial class DirectoryEx2
    {
        [ThreadStatic]
        private static MockBase _mock;

        /// <summary>
        /// Gets or sets the mock.
        /// </summary>
        /// <value>
        /// The mock.
        /// </value>
        public static MockBase Mock
        {
            get { return _mock; }
            set { _mock = value; }
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DirectoryInfo CreateDirectory(string path) { return _mock.CreateDirectory(path); }
        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="directorySecurity">The directory security.</param>
        /// <returns></returns>
        public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity) { return _mock.CreateDirectory(path, directorySecurity); }
        /// <summary>
        /// Deletes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void Delete(string path) { _mock.Delete(path); }
        /// <summary>
        /// Deletes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public static void Delete(string path, bool recursive) { _mock.Delete(path, recursive); }
        /// <summary>
        /// Existses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool Exists(string path) { return _mock.Exists(path); }
        /// <summary>
        /// Gets the access control.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DirectorySecurity GetAccessControl(string path) { return _mock.GetAccessControl(path); }
        /// <summary>
        /// Gets the access control.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="includeSections">The include sections.</param>
        /// <returns></returns>
        public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections) { return _mock.GetAccessControl(path, includeSections); }
        /// <summary>
        /// Gets the creation time.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DateTime GetCreationTime(string path) { return _mock.GetCreationTime(path); }
        /// <summary>
        /// Gets the creation time UTC.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DateTime GetCreationTimeUtc(string path) { return _mock.GetCreationTimeUtc(path); }
        /// <summary>
        /// Gets the current directory.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory() { return _mock.GetCurrentDirectory(); }
        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string[] GetDirectories(string path) { return _mock.GetDirectories(path); }
        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static string[] GetDirectories(string path, string searchPattern) { return _mock.GetDirectories(path, searchPattern); }
        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns></returns>
        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) { return _mock.GetDirectories(path, searchPattern, searchOption); }
        /// <summary>
        /// Gets the directory root.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string GetDirectoryRoot(string path) { return _mock.GetDirectoryRoot(path); }
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string[] GetFiles(string path) { return _mock.GetFiles(path); }
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static string[] GetFiles(string path, string searchPattern) { return _mock.GetFiles(path, searchPattern); }
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns></returns>
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption) { return _mock.GetFiles(path, searchPattern, searchOption); }
        /// <summary>
        /// Gets the file system entries.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string[] GetFileSystemEntries(string path) { return _mock.GetFileSystemEntries(path); }
        /// <summary>
        /// Gets the file system entries.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static string[] GetFileSystemEntries(string path, string searchPattern) { return _mock.GetFileSystemEntries(path, searchPattern); }
        /// <summary>
        /// Gets the last access time.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DateTime GetLastAccessTime(string path) { return _mock.GetLastAccessTime(path); }
        /// <summary>
        /// Gets the last access time UTC.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DateTime GetLastAccessTimeUtc(string path) { return _mock.GetLastAccessTimeUtc(path); }
        /// <summary>
        /// Gets the last write time.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DateTime GetLastWriteTime(string path) { return _mock.GetLastWriteTime(path); }
        /// <summary>
        /// Gets the last write time UTC.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DateTime GetLastWriteTimeUtc(string path) { return _mock.GetLastWriteTimeUtc(path); }
        /// <summary>
        /// Gets the logical drives.
        /// </summary>
        /// <returns></returns>
        public static string[] GetLogicalDrives() { return _mock.GetLogicalDrives(); }
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static DirectoryInfo GetParent(string path) { return _mock.GetParent(path); }
        /// <summary>
        /// Moves the specified source dir name.
        /// </summary>
        /// <param name="sourceDirName">Name of the source dir.</param>
        /// <param name="destDirName">Name of the dest dir.</param>
        public static void Move(string sourceDirName, string destDirName) { _mock.Move(sourceDirName, destDirName); }
        /// <summary>
        /// Sets the access control.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="directorySecurity">The directory security.</param>
        public static void SetAccessControl(string path, DirectorySecurity directorySecurity) { _mock.SetAccessControl(path, directorySecurity); }
        /// <summary>
        /// Sets the creation time.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="creationTime">The creation time.</param>
        public static void SetCreationTime(string path, DateTime creationTime) { _mock.SetCreationTime(path, creationTime); }
        /// <summary>
        /// Sets the creation time UTC.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="creationTimeUtc">The creation time UTC.</param>
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc) { _mock.SetCreationTimeUtc(path, creationTimeUtc); }
        /// <summary>
        /// Sets the current directory.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void SetCurrentDirectory(string path) { _mock.SetCurrentDirectory(path); }
        /// <summary>
        /// Sets the last access time.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lastAccessTime">The last access time.</param>
        public static void SetLastAccessTime(string path, DateTime lastAccessTime) { _mock.SetLastAccessTime(path, lastAccessTime); }
        /// <summary>
        /// Sets the last access time UTC.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lastAccessTimeUtc">The last access time UTC.</param>
        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) { _mock.SetLastAccessTimeUtc(path, lastAccessTimeUtc); }
        /// <summary>
        /// Sets the last write time.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lastWriteTime">The last write time.</param>
        public static void SetLastWriteTime(string path, DateTime lastWriteTime) { _mock.SetLastWriteTime(path, lastWriteTime); }
        /// <summary>
        /// Sets the last write time UTC.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lastWriteTimeUtc">The last write time UTC.</param>
        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) { _mock.SetLastWriteTimeUtc(path, lastWriteTimeUtc); }
    }
}
