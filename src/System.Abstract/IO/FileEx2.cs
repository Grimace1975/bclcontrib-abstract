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
        /// Appends all text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        public static void AppendAllText(string path, string contents) { _mock.AppendAllText(path, contents); }
        /// <summary>
        /// Appends all text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="encoding">The encoding.</param>
        public static void AppendAllText(string path, string contents, Encoding encoding) { _mock.AppendAllText(path, contents, encoding); }
        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static StreamWriter AppendText(string path) { return _mock.AppendText(path); }
        /// <summary>
        /// Copies the specified source file name.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="destFileName">Name of the dest file.</param>
        public static void Copy(string sourceFileName, string destFileName) { _mock.Copy(sourceFileName, destFileName); }
        /// <summary>
        /// Copies the specified source file name.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="destFileName">Name of the dest file.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite) { _mock.Copy(sourceFileName, destFileName, overwrite); }
        /// <summary>
        /// Creates the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static FileStream Create(string path) { return _mock.Create(path); }
        /// <summary>
        /// Creates the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns></returns>
        public static FileStream Create(string path, int bufferSize) { return _mock.Create(path, bufferSize); }
        /// <summary>
        /// Creates the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static FileStream Create(string path, int bufferSize, FileOptions options) { return _mock.Create(path, bufferSize, options); }
        /// <summary>
        /// Creates the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="options">The options.</param>
        /// <param name="fileSecurity">The file security.</param>
        /// <returns></returns>
        public static FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity) { return _mock.Create(path, bufferSize, options, fileSecurity); }
        /// <summary>
        /// Creates the text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static StreamWriter CreateText(string path) { return _mock.CreateText(path); }
        /// <summary>
        /// Decrypts the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void Decrypt(string path) { _mock.Decrypt(path); }
        /// <summary>
        /// Deletes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void Delete(string path) { _mock.Delete(path); }
        /// <summary>
        /// Encrypts the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void Encrypt(string path) { _mock.Encrypt(path); }
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
        public static FileSecurity GetAccessControl(string path) { return _mock.GetAccessControl(path); }
        /// <summary>
        /// Gets the access control.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="includeSections">The include sections.</param>
        /// <returns></returns>
        public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections) { return _mock.GetAccessControl(path, includeSections); }
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static FileAttributes GetAttributes(string path) { return _mock.GetAttributes(path); }
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
        /// Moves the specified source file name.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="destFileName">Name of the dest file.</param>
        public static void Move(string sourceFileName, string destFileName) { _mock.Move(sourceFileName, destFileName); }
        /// <summary>
        /// Opens the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public static FileStream Open(string path, FileMode mode) { return _mock.Open(path, mode); }
        /// <summary>
        /// Opens the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <returns></returns>
        public static FileStream Open(string path, FileMode mode, FileAccess access) { return _mock.Open(path, mode, access); }
        /// <summary>
        /// Opens the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <param name="share">The share.</param>
        /// <returns></returns>
        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) { return _mock.Open(path, mode, access, share); }
        /// <summary>
        /// Opens the read.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static FileStream OpenRead(string path) { return _mock.OpenRead(path); }
        /// <summary>
        /// Opens the text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static StreamReader OpenText(string path) { return _mock.OpenText(path); }
        /// <summary>
        /// Opens the write.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static FileStream OpenWrite(string path) { return _mock.OpenWrite(path); }
        /// <summary>
        /// Reads all bytes.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(string path) { return _mock.ReadAllBytes(path); }
        /// <summary>
        /// Reads all lines.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string[] ReadAllLines(string path) { return _mock.ReadAllLines(path); }
        /// <summary>
        /// Reads all lines.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string[] ReadAllLines(string path, Encoding encoding) { return _mock.ReadAllLines(path); }
        /// <summary>
        /// Reads all text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string ReadAllText(string path) { return _mock.ReadAllText(path); }
        /// <summary>
        /// Reads all text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string ReadAllText(string path, Encoding encoding) { return _mock.ReadAllText(path); }
        /// <summary>
        /// Replaces the specified source file name.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="destinationFileName">Name of the destination file.</param>
        /// <param name="destinationBackupFileName">Name of the destination backup file.</param>
        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName) { _mock.Replace(sourceFileName, destinationFileName, destinationBackupFileName); }
        /// <summary>
        /// Replaces the specified source file name.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="destinationFileName">Name of the destination file.</param>
        /// <param name="destinationBackupFileName">Name of the destination backup file.</param>
        /// <param name="ignoreMetadataErrors">if set to <c>true</c> [ignore metadata errors].</param>
        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) { _mock.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors); }
        /// <summary>
        /// Sets the access control.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileSecurity">The file security.</param>
        public static void SetAccessControl(string path, FileSecurity fileSecurity) { _mock.SetAccessControl(path, fileSecurity); }
        /// <summary>
        /// Sets the attributes.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileAttributes">The file attributes.</param>
        public static void SetAttributes(string path, FileAttributes fileAttributes) { _mock.SetAttributes(path, fileAttributes); }
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
        /// <summary>
        /// Writes all bytes.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="bytes">The bytes.</param>
        public static void WriteAllBytes(string path, byte[] bytes) { _mock.WriteAllBytes(path, bytes); }
        /// <summary>
        /// Writes all lines.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        public static void WriteAllLines(string path, string[] contents) { _mock.WriteAllLines(path, contents); }
        /// <summary>
        /// Writes all lines.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="encoding">The encoding.</param>
        public static void WriteAllLines(string path, string[] contents, Encoding encoding) { _mock.WriteAllLines(path, contents, encoding); }
        /// <summary>
        /// Writes all text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        public static void WriteAllText(string path, string contents) { _mock.WriteAllText(path, contents); }
        /// <summary>
        /// Writes all text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="encoding">The encoding.</param>
        public static void WriteAllText(string path, string contents, Encoding encoding) { _mock.WriteAllText(path, contents, encoding); }
    }
}
