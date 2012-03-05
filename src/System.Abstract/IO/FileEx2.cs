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
using System.Text;
using System.Security.AccessControl;
namespace System.IO
{
    /// <summary>
    /// FileEx2
    /// </summary>
    public static partial class FileEx2
    {
        [ThreadStatic]
        private static MockBase _mock;

        public static MockBase Mock
        {
            get { return _mock; }
            set { _mock = value; }
        }

        public static void AppendAllText(string path, string contents) { _mock.AppendAllText(path, contents); }
        public static void AppendAllText(string path, string contents, Encoding encoding) { _mock.AppendAllText(path, contents, encoding); }
        public static StreamWriter AppendText(string path) { return _mock.AppendText(path); }
        public static void Copy(string sourceFileName, string destFileName) { _mock.Copy(sourceFileName, destFileName); }
        public static void Copy(string sourceFileName, string destFileName, bool overwrite) { _mock.Copy(sourceFileName, destFileName, overwrite); }
        public static FileStream Create(string path) { return _mock.Create(path); }
        public static FileStream Create(string path, int bufferSize) { return _mock.Create(path, bufferSize); }
        public static FileStream Create(string path, int bufferSize, FileOptions options) { return _mock.Create(path, bufferSize, options); }
        public static FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity) { return _mock.Create(path, bufferSize, options, fileSecurity); }
        public static StreamWriter CreateText(string path) { return _mock.CreateText(path); }
        public static void Decrypt(string path) { _mock.Decrypt(path); }
        public static void Delete(string path) { _mock.Delete(path); }
        public static void Encrypt(string path) { _mock.Encrypt(path); }
        public static bool Exists(string path) { return _mock.Exists(path); }
        public static FileSecurity GetAccessControl(string path) { return _mock.GetAccessControl(path); }
        public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections) { return _mock.GetAccessControl(path, includeSections); }
        public static FileAttributes GetAttributes(string path) { return _mock.GetAttributes(path); }
        public static DateTime GetCreationTime(string path) { return _mock.GetCreationTime(path); }
        public static DateTime GetCreationTimeUtc(string path) { return _mock.GetCreationTimeUtc(path); }
        public static DateTime GetLastAccessTime(string path) { return _mock.GetLastAccessTime(path); }
        public static DateTime GetLastAccessTimeUtc(string path) { return _mock.GetLastAccessTimeUtc(path); }
        public static DateTime GetLastWriteTime(string path) { return _mock.GetLastWriteTime(path); }
        public static DateTime GetLastWriteTimeUtc(string path) { return _mock.GetLastWriteTimeUtc(path); }
        public static void Move(string sourceFileName, string destFileName) { _mock.Move(sourceFileName, destFileName); }
        public static FileStream Open(string path, FileMode mode) { return _mock.Open(path, mode); }
        public static FileStream Open(string path, FileMode mode, FileAccess access) { return _mock.Open(path, mode, access); }
        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) { return _mock.Open(path, mode, access, share); }
        public static FileStream OpenRead(string path) { return _mock.OpenRead(path); }
        public static StreamReader OpenText(string path) { return _mock.OpenText(path); }
        public static FileStream OpenWrite(string path) { return _mock.OpenWrite(path); }
        public static byte[] ReadAllBytes(string path) { return _mock.ReadAllBytes(path); }
        public static string[] ReadAllLines(string path) { return _mock.ReadAllLines(path); }
        public static string[] ReadAllLines(string path, Encoding encoding) { return _mock.ReadAllLines(path); }
        public static string ReadAllText(string path) { return _mock.ReadAllText(path); }
        public static string ReadAllText(string path, Encoding encoding) { return _mock.ReadAllText(path); }
        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName) { _mock.Replace(sourceFileName, destinationFileName, destinationBackupFileName); }
        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) { _mock.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors); }
        public static void SetAccessControl(string path, FileSecurity fileSecurity) { _mock.SetAccessControl(path, fileSecurity); }
        public static void SetAttributes(string path, FileAttributes fileAttributes) { _mock.SetAttributes(path, fileAttributes); }
        public static void SetCreationTime(string path, DateTime creationTime) { _mock.SetCreationTime(path, creationTime); }
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc) { _mock.SetCreationTimeUtc(path, creationTimeUtc); }
        public static void SetLastAccessTime(string path, DateTime lastAccessTime) { _mock.SetLastAccessTime(path, lastAccessTime); }
        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) { _mock.SetLastAccessTimeUtc(path, lastAccessTimeUtc); }
        public static void SetLastWriteTime(string path, DateTime lastWriteTime) { _mock.SetLastWriteTime(path, lastWriteTime); }
        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) { _mock.SetLastWriteTimeUtc(path, lastWriteTimeUtc); }
        public static void WriteAllBytes(string path, byte[] bytes) { _mock.WriteAllBytes(path, bytes); }
        public static void WriteAllLines(string path, string[] contents) { _mock.WriteAllLines(path, contents); }
        public static void WriteAllLines(string path, string[] contents, Encoding encoding) { _mock.WriteAllLines(path, contents, encoding); }
        public static void WriteAllText(string path, string contents) { _mock.WriteAllText(path, contents); }
        public static void WriteAllText(string path, string contents, Encoding encoding) { _mock.WriteAllText(path, contents, encoding); }
    }
}
