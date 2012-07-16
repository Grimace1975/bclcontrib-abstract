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
    public static partial class FileEx2
    {
        /// <summary>
        /// FileWrapper
        /// </summary>
        public class FileWrapper : MockBase
        {
            /// <summary>
            /// Appends all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            public override void AppendAllText(string path, string contents) { File.AppendAllText(path, contents); }
            /// <summary>
            /// Appends all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            /// <param name="encoding">The encoding.</param>
            public override void AppendAllText(string path, string contents, Encoding encoding) { File.AppendAllText(path, contents, encoding); }
            /// <summary>
            /// Appends the text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override StreamWriter AppendText(string path) { return File.AppendText(path); }
            /// <summary>
            /// Copies the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destFileName">Name of the dest file.</param>
            public override void Copy(string sourceFileName, string destFileName) { File.Copy(sourceFileName, destFileName); }
            /// <summary>
            /// Copies the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destFileName">Name of the dest file.</param>
            /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
            public override void Copy(string sourceFileName, string destFileName, bool overwrite) { File.Copy(sourceFileName, destFileName, overwrite); }
            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override FileStream Create(string path) { return File.Create(path); }
            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="bufferSize">Size of the buffer.</param>
            /// <returns></returns>
            public override FileStream Create(string path, int bufferSize) { return File.Create(path, bufferSize); }
            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="bufferSize">Size of the buffer.</param>
            /// <param name="options">The options.</param>
            /// <returns></returns>
            public override FileStream Create(string path, int bufferSize, FileOptions options) { return File.Create(path, bufferSize, options); }
            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="bufferSize">Size of the buffer.</param>
            /// <param name="options">The options.</param>
            /// <param name="fileSecurity">The file security.</param>
            /// <returns></returns>
            public override FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity) { return File.Create(path, bufferSize, options, fileSecurity); }
            /// <summary>
            /// Creates the text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override StreamWriter CreateText(string path) { return File.CreateText(path); }
            /// <summary>
            /// Decrypts the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            public override void Decrypt(string path) { File.Decrypt(path); }
            /// <summary>
            /// Deletes the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            public override void Delete(string path) { File.Delete(path); }
            /// <summary>
            /// Encrypts the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            public override void Encrypt(string path) { File.Encrypt(path); }
            /// <summary>
            /// Existses the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override bool Exists(string path) { return File.Exists(path); }
            /// <summary>
            /// Gets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override FileSecurity GetAccessControl(string path) { return File.GetAccessControl(path); }
            /// <summary>
            /// Gets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="includeSections">The include sections.</param>
            /// <returns></returns>
            public override FileSecurity GetAccessControl(string path, AccessControlSections includeSections) { return File.GetAccessControl(path, includeSections); }
            /// <summary>
            /// Gets the attributes.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override FileAttributes GetAttributes(string path) { return File.GetAttributes(path); }
            /// <summary>
            /// Gets the creation time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetCreationTime(string path) { return File.GetCreationTime(path); }
            /// <summary>
            /// Gets the creation time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetCreationTimeUtc(string path) { return File.GetCreationTimeUtc(path); }
            /// <summary>
            /// Gets the last access time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetLastAccessTime(string path) { return File.GetLastAccessTime(path); }
            /// <summary>
            /// Gets the last access time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetLastAccessTimeUtc(string path) { return File.GetLastAccessTimeUtc(path); }
            /// <summary>
            /// Gets the last write time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetLastWriteTime(string path) { return File.GetLastWriteTime(path); }
            /// <summary>
            /// Gets the last write time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override DateTime GetLastWriteTimeUtc(string path) { return File.GetLastWriteTimeUtc(path); }
            /// <summary>
            /// Moves the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destFileName">Name of the dest file.</param>
            public override void Move(string sourceFileName, string destFileName) { File.Move(sourceFileName, destFileName); }
            /// <summary>
            /// Opens the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="mode">The mode.</param>
            /// <returns></returns>
            public override FileStream Open(string path, FileMode mode) { return File.Open(path, mode); }
            /// <summary>
            /// Opens the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="access">The access.</param>
            /// <returns></returns>
            public override FileStream Open(string path, FileMode mode, FileAccess access) { return File.Open(path, mode, access); }
            /// <summary>
            /// Opens the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="access">The access.</param>
            /// <param name="share">The share.</param>
            /// <returns></returns>
            public override FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) { return File.Open(path, mode, access, share); }
            /// <summary>
            /// Opens the read.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override FileStream OpenRead(string path) { return File.OpenRead(path); }
            /// <summary>
            /// Opens the text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override StreamReader OpenText(string path) { return File.OpenText(path); }
            /// <summary>
            /// Opens the write.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override FileStream OpenWrite(string path) { return File.OpenWrite(path); }
            /// <summary>
            /// Reads all bytes.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override byte[] ReadAllBytes(string path) { return File.ReadAllBytes(path); }
            /// <summary>
            /// Reads all lines.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string[] ReadAllLines(string path) { return File.ReadAllLines(path); }
            /// <summary>
            /// Reads all lines.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="encoding">The encoding.</param>
            /// <returns></returns>
            public override string[] ReadAllLines(string path, Encoding encoding) { return File.ReadAllLines(path); }
            /// <summary>
            /// Reads all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public override string ReadAllText(string path) { return File.ReadAllText(path); }
            /// <summary>
            /// Reads all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="encoding">The encoding.</param>
            /// <returns></returns>
            public override string ReadAllText(string path, Encoding encoding) { return File.ReadAllText(path); }
            /// <summary>
            /// Replaces the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destinationFileName">Name of the destination file.</param>
            /// <param name="destinationBackupFileName">Name of the destination backup file.</param>
            public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName) { File.Replace(sourceFileName, destinationFileName, destinationBackupFileName); }
            /// <summary>
            /// Replaces the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destinationFileName">Name of the destination file.</param>
            /// <param name="destinationBackupFileName">Name of the destination backup file.</param>
            /// <param name="ignoreMetadataErrors">if set to <c>true</c> [ignore metadata errors].</param>
            public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) { File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors); }
            /// <summary>
            /// Sets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="fileSecurity">The file security.</param>
            public override void SetAccessControl(string path, FileSecurity fileSecurity) { File.SetAccessControl(path, fileSecurity); }
            /// <summary>
            /// Sets the attributes.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="fileAttributes">The file attributes.</param>
            public override void SetAttributes(string path, FileAttributes fileAttributes) { File.SetAttributes(path, fileAttributes); }
            /// <summary>
            /// Sets the creation time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="creationTime">The creation time.</param>
            public override void SetCreationTime(string path, DateTime creationTime) { File.SetCreationTime(path, creationTime); }
            /// <summary>
            /// Sets the creation time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="creationTimeUtc">The creation time UTC.</param>
            public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc) { File.SetCreationTimeUtc(path, creationTimeUtc); }
            /// <summary>
            /// Sets the last access time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastAccessTime">The last access time.</param>
            public override void SetLastAccessTime(string path, DateTime lastAccessTime) { File.SetLastAccessTime(path, lastAccessTime); }
            /// <summary>
            /// Sets the last access time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastAccessTimeUtc">The last access time UTC.</param>
            public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) { File.SetLastAccessTimeUtc(path, lastAccessTimeUtc); }
            /// <summary>
            /// Sets the last write time.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastWriteTime">The last write time.</param>
            public override void SetLastWriteTime(string path, DateTime lastWriteTime) { File.SetLastWriteTime(path, lastWriteTime); }
            /// <summary>
            /// Sets the last write time UTC.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="lastWriteTimeUtc">The last write time UTC.</param>
            public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) { File.SetLastWriteTimeUtc(path, lastWriteTimeUtc); }
            /// <summary>
            /// Writes all bytes.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="bytes">The bytes.</param>
            public override void WriteAllBytes(string path, byte[] bytes) { File.WriteAllBytes(path, bytes); }
            /// <summary>
            /// Writes all lines.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            public override void WriteAllLines(string path, string[] contents) { File.WriteAllLines(path, contents); }
            /// <summary>
            /// Writes all lines.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            /// <param name="encoding">The encoding.</param>
            public override void WriteAllLines(string path, string[] contents, Encoding encoding) { File.WriteAllLines(path, contents, encoding); }
            /// <summary>
            /// Writes all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            public override void WriteAllText(string path, string contents) { File.WriteAllText(path, contents); }
            /// <summary>
            /// Writes all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            /// <param name="encoding">The encoding.</param>
            public override void WriteAllText(string path, string contents, Encoding encoding) { File.WriteAllText(path, contents, encoding); }
        }
    }
}
