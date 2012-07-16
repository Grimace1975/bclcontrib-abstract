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
using System.Text;
namespace System.IO
{
    public static partial class FileEx2
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
            /// Appends all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            public virtual void AppendAllText(string path, string contents) { throw new NotImplementedException(); }
            /// <summary>
            /// Appends all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            /// <param name="encoding">The encoding.</param>
            public virtual void AppendAllText(string path, string contents, Encoding encoding) { throw new NotImplementedException(); }
            /// <summary>
            /// Appends the text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual StreamWriter AppendText(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Copies the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destFileName">Name of the dest file.</param>
            public virtual void Copy(string sourceFileName, string destFileName) { throw new NotImplementedException(); }
            /// <summary>
            /// Copies the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destFileName">Name of the dest file.</param>
            /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
            public virtual void Copy(string sourceFileName, string destFileName, bool overwrite) { throw new NotImplementedException(); }
            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual FileStream Create(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="bufferSize">Size of the buffer.</param>
            /// <returns></returns>
            public virtual FileStream Create(string path, int bufferSize) { throw new NotImplementedException(); }
            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="bufferSize">Size of the buffer.</param>
            /// <param name="options">The options.</param>
            /// <returns></returns>
            public virtual FileStream Create(string path, int bufferSize, FileOptions options) { throw new NotImplementedException(); }
            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="bufferSize">Size of the buffer.</param>
            /// <param name="options">The options.</param>
            /// <param name="fileSecurity">The file security.</param>
            /// <returns></returns>
            public virtual FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity) { throw new NotImplementedException(); }
            /// <summary>
            /// Creates the text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual StreamWriter CreateText(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Decrypts the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            public virtual void Decrypt(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Deletes the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            public virtual void Delete(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Encrypts the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            public virtual void Encrypt(string path) { throw new NotImplementedException(); }
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
            public virtual FileSecurity GetAccessControl(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="includeSections">The include sections.</param>
            /// <returns></returns>
            public virtual FileSecurity GetAccessControl(string path, AccessControlSections includeSections) { throw new NotImplementedException(); }
            /// <summary>
            /// Gets the attributes.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual FileAttributes GetAttributes(string path) { throw new NotImplementedException(); }
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
            /// Moves the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destFileName">Name of the dest file.</param>
            public virtual void Move(string sourceFileName, string destFileName) { throw new NotImplementedException(); }
            /// <summary>
            /// Opens the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="mode">The mode.</param>
            /// <returns></returns>
            public virtual FileStream Open(string path, FileMode mode) { throw new NotImplementedException(); }
            /// <summary>
            /// Opens the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="access">The access.</param>
            /// <returns></returns>
            public virtual FileStream Open(string path, FileMode mode, FileAccess access) { throw new NotImplementedException(); }
            /// <summary>
            /// Opens the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="access">The access.</param>
            /// <param name="share">The share.</param>
            /// <returns></returns>
            public virtual FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) { throw new NotImplementedException(); }
            /// <summary>
            /// Opens the read.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual FileStream OpenRead(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Opens the text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual StreamReader OpenText(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Opens the write.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual FileStream OpenWrite(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Reads all bytes.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual byte[] ReadAllBytes(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Reads all lines.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual string[] ReadAllLines(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Reads all lines.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="encoding">The encoding.</param>
            /// <returns></returns>
            public virtual string[] ReadAllLines(string path, Encoding encoding) { throw new NotImplementedException(); }
            /// <summary>
            /// Reads all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            public virtual string ReadAllText(string path) { throw new NotImplementedException(); }
            /// <summary>
            /// Reads all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="encoding">The encoding.</param>
            /// <returns></returns>
            public virtual string ReadAllText(string path, Encoding encoding) { throw new NotImplementedException(); }
            /// <summary>
            /// Replaces the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destinationFileName">Name of the destination file.</param>
            /// <param name="destinationBackupFileName">Name of the destination backup file.</param>
            public virtual void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName) { throw new NotImplementedException(); }
            /// <summary>
            /// Replaces the specified source file name.
            /// </summary>
            /// <param name="sourceFileName">Name of the source file.</param>
            /// <param name="destinationFileName">Name of the destination file.</param>
            /// <param name="destinationBackupFileName">Name of the destination backup file.</param>
            /// <param name="ignoreMetadataErrors">if set to <c>true</c> [ignore metadata errors].</param>
            public virtual void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the access control.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="fileSecurity">The file security.</param>
            public virtual void SetAccessControl(string path, FileSecurity fileSecurity) { throw new NotImplementedException(); }
            /// <summary>
            /// Sets the attributes.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="fileAttributes">The file attributes.</param>
            public virtual void SetAttributes(string path, FileAttributes fileAttributes) { throw new NotImplementedException(); }
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
            /// <summary>
            /// Writes all bytes.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="bytes">The bytes.</param>
            public virtual void WriteAllBytes(string path, byte[] bytes) { throw new NotImplementedException(); }
            /// <summary>
            /// Writes all lines.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            public virtual void WriteAllLines(string path, string[] contents) { throw new NotImplementedException(); }
            /// <summary>
            /// Writes all lines.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            /// <param name="encoding">The encoding.</param>
            public virtual void WriteAllLines(string path, string[] contents, Encoding encoding) { throw new NotImplementedException(); }
            /// <summary>
            /// Writes all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            public virtual void WriteAllText(string path, string contents) { throw new NotImplementedException(); }
            /// <summary>
            /// Writes all text.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="contents">The contents.</param>
            /// <param name="encoding">The encoding.</param>
            public virtual void WriteAllText(string path, string contents, Encoding encoding) { throw new NotImplementedException(); }
        }
    }
}
