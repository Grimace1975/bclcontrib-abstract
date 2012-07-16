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
using System;
using System.IO;
using System.Reflection;
using System.Web.Hosting;
namespace Contoso.Abstract
{
    /// <summary>
    /// AssemblyResourceFile
    /// </summary>
    public class AssemblyResourceFile : VirtualFile
    {
        private readonly Assembly _assembly;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourceFile"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="name">The name.</param>
        /// <param name="virtualPath">The virtual path.</param>
        public AssemblyResourceFile(Assembly assembly, string name, string virtualPath)
            : base(virtualPath)
        {
            if (assembly == null)
                throw new ArgumentNullException("view");
            if (name == null)
                throw new ArgumentNullException("name");
            _assembly = assembly;
            _name = name;
        }

        /// <summary>
        /// When overridden in a derived class, returns a read-only stream to the virtual resource.
        /// </summary>
        /// <returns>
        /// A read-only stream to the virtual file.
        /// </returns>
        public override Stream Open() { return _assembly.GetManifestResourceStream(_name); }
    }
}


