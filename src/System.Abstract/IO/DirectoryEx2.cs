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

        public static MockBase Mock
        {
            get { return _mock; }
            set { _mock = value; }
        }

        public static DirectoryInfo CreateDirectory(string path) { return _mock.CreateDirectory(path); }
        public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity) { return _mock.CreateDirectory(path, directorySecurity); }
        public static void Delete(string path) { _mock.Delete(path); }
        public static void Delete(string path, bool recursive) { _mock.Delete(path, recursive); }
        public static bool Exists(string path) { return _mock.Exists(path); }
        public static DirectorySecurity GetAccessControl(string path) { return _mock.GetAccessControl(path); }
        public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections) { return _mock.GetAccessControl(path, includeSections); }
        public static DateTime GetCreationTime(string path) { return _mock.GetCreationTime(path); }
        public static DateTime GetCreationTimeUtc(string path) { return _mock.GetCreationTimeUtc(path); }
        public static string GetCurrentDirectory() { return _mock.GetCurrentDirectory(); }
        public static string[] GetDirectories(string path) { return _mock.GetDirectories(path); }
        public static string[] GetDirectories(string path, string searchPattern) { return _mock.GetDirectories(path, searchPattern); }
        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) { return _mock.GetDirectories(path, searchPattern, searchOption); }
        public static string GetDirectoryRoot(string path) { return _mock.GetDirectoryRoot(path); }
        public static string[] GetFiles(string path) { return _mock.GetFiles(path); }
        public static string[] GetFiles(string path, string searchPattern) { return _mock.GetFiles(path, searchPattern); }
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption) { return _mock.GetFiles(path, searchPattern, searchOption); }
        public static string[] GetFileSystemEntries(string path) { return _mock.GetFileSystemEntries(path); }
        public static string[] GetFileSystemEntries(string path, string searchPattern) { return _mock.GetFileSystemEntries(path, searchPattern); }
        public static DateTime GetLastAccessTime(string path) { return _mock.GetLastAccessTime(path); }
        public static DateTime GetLastAccessTimeUtc(string path) { return _mock.GetLastAccessTimeUtc(path); }
        public static DateTime GetLastWriteTime(string path) { return _mock.GetLastWriteTime(path); }
        public static DateTime GetLastWriteTimeUtc(string path) { return _mock.GetLastWriteTimeUtc(path); }
        public static string[] GetLogicalDrives() { return _mock.GetLogicalDrives(); }
        public static DirectoryInfo GetParent(string path) { return _mock.GetParent(path); }
        public static void Move(string sourceDirName, string destDirName) { _mock.Move(sourceDirName, destDirName); }
        public static void SetAccessControl(string path, DirectorySecurity directorySecurity) { _mock.SetAccessControl(path, directorySecurity); }
        public static void SetCreationTime(string path, DateTime creationTime) { _mock.SetCreationTime(path, creationTime); }
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc) { _mock.SetCreationTimeUtc(path, creationTimeUtc); }
        public static void SetCurrentDirectory(string path) { _mock.SetCurrentDirectory(path); }
        public static void SetLastAccessTime(string path, DateTime lastAccessTime) { _mock.SetLastAccessTime(path, lastAccessTime); }
        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) { _mock.SetLastAccessTimeUtc(path, lastAccessTimeUtc); }
        public static void SetLastWriteTime(string path, DateTime lastWriteTime) { _mock.SetLastWriteTime(path, lastWriteTime); }
        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) { _mock.SetLastWriteTimeUtc(path, lastWriteTimeUtc); }
    }
}
