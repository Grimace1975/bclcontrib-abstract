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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
namespace Contoso.Abstract
{
    /// <summary>
    /// AssemblyResourcePathProvider
    /// </summary>
    public class AssemblyResourcePathProvider : VirtualPathProvider
    {
        private readonly Assembly _assembly;
        private Dictionary<string, File> _assemblyFiles;

        private struct File
        {
            public File(string name, string path) { Name = name; Path = path; }
            public string Name;
            public string Path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourcePathProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="selector">The selector.</param>
        public AssemblyResourcePathProvider(Assembly assembly, Func<string, string> selector)
            : this("/", assembly, selector) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourcePathProvider"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="selector">The selector.</param>
        public AssemblyResourcePathProvider(string path, Assembly assembly, Func<string, string> selector)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (!path.StartsWith("/"))
                throw new ArgumentOutOfRangeException("path", path, "Must start with /");
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            _assembly = assembly;
            _assemblyFiles = _assembly.GetManifestResourceNames()
                .Select(x => { var newPath = selector(x); return (!string.IsNullOrEmpty(newPath) ? new File(x, path + newPath) : new File(null, null)); })
                .Where(x => x.Path != null)
                .ToDictionary(x => x.Path);
        }

        /// <summary>
        /// Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <returns>
        /// true if the file exists in the virtual file system; otherwise, false.
        /// </returns>
        public override bool FileExists(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            if (_assemblyFiles.ContainsKey(absolutePath)) return true;
            return Previous.FileExists(absolutePath);
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <param name="virtualPathDependencies">An array of paths to other resources required by the primary virtual resource.</param>
        /// <param name="utcStart">The UTC time at which the virtual resources were read.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency"/> object for the specified virtual resources.
        /// </returns>
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            if (_assemblyFiles.ContainsKey(absolutePath)) return null;
            return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        /// <summary>
        /// Gets a virtual file from the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <returns>
        /// A descendent of the <see cref="T:System.Web.Hosting.VirtualFile"/> class that represents a file in the virtual file system.
        /// </returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            File item;
            if (_assemblyFiles.TryGetValue(absolutePath, out item)) return new AssemblyResourceFile(_assembly, item.Name, virtualPath);
            return Previous.GetFile(virtualPath);
        }
    }
}


