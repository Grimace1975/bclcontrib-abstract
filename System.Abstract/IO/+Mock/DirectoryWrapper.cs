﻿#region License
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
    public static partial class DirectoryEx
    {
        /// <summary>
        /// DirectoryWrapper
        /// </summary>
        public class DirectoryWrapper : MockBase
        {
            public override DirectoryInfo CreateDirectory(string path) { return Directory.CreateDirectory(path); }
            public override DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity) { return Directory.CreateDirectory(path, directorySecurity); }
            public override void Delete(string path) { Directory.Delete(path); }
            public override void Delete(string path, bool recursive) { Directory.Delete(path, recursive); }
            public override bool Exists(string path) { return Directory.Exists(path); }
            public override DirectorySecurity GetAccessControl(string path) { return Directory.GetAccessControl(path); }
            public override DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections) { return Directory.GetAccessControl(path, includeSections); }
            public override DateTime GetCreationTime(string path) { return Directory.GetCreationTime(path); }
            public override DateTime GetCreationTimeUtc(string path) { return Directory.GetCreationTimeUtc(path); }
            public override string GetCurrentDirectory() { return Directory.GetCurrentDirectory(); }
            public override string[] GetDirectories(string path) { return Directory.GetDirectories(path); }
            public override string[] GetDirectories(string path, string searchPattern) { return Directory.GetDirectories(path, searchPattern); }
            public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) { return Directory.GetDirectories(path, searchPattern, searchOption); }
            public override string GetDirectoryRoot(string path) { return Directory.GetDirectoryRoot(path); }
            public override string[] GetFiles(string path) { return Directory.GetFiles(path); }
            public override string[] GetFiles(string path, string searchPattern) { return Directory.GetFiles(path, searchPattern); }
            public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption) { return Directory.GetFiles(path, searchPattern, searchOption); }
            public override string[] GetFileSystemEntries(string path) { return Directory.GetFileSystemEntries(path); }
            public override string[] GetFileSystemEntries(string path, string searchPattern) { return Directory.GetFileSystemEntries(path, searchPattern); }
            public override DateTime GetLastAccessTime(string path) { return Directory.GetLastAccessTime(path); }
            public override DateTime GetLastAccessTimeUtc(string path) { return Directory.GetLastAccessTimeUtc(path); }
            public override DateTime GetLastWriteTime(string path) { return Directory.GetLastWriteTime(path); }
            public override DateTime GetLastWriteTimeUtc(string path) { return Directory.GetLastWriteTimeUtc(path); }
            public override string[] GetLogicalDrives() { return Directory.GetLogicalDrives(); }
            public override DirectoryInfo GetParent(string path) { return Directory.GetParent(path); }
            public override void Move(string sourceDirName, string destDirName) { Directory.Move(sourceDirName, destDirName); }
            public override void SetAccessControl(string path, DirectorySecurity directorySecurity) { Directory.SetAccessControl(path, directorySecurity); }
            public override void SetCreationTime(string path, DateTime creationTime) { Directory.SetCreationTime(path, creationTime); }
            public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc) { Directory.SetCreationTimeUtc(path, creationTimeUtc); }
            public override void SetCurrentDirectory(string path) { Directory.SetCurrentDirectory(path); }
            public override void SetLastAccessTime(string path, DateTime lastAccessTime) { Directory.SetLastAccessTime(path, lastAccessTime); }
            public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) { Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc); }
            public override void SetLastWriteTime(string path, DateTime lastWriteTime) { Directory.SetLastWriteTime(path, lastWriteTime); }
            public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) { Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc); }
        }
    }
}
