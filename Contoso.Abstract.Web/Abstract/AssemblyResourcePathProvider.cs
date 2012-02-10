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

        public AssemblyResourcePathProvider(Assembly assembly, Func<string, string> selector)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            _assembly = assembly;
            _assemblyFiles = _assembly.GetManifestResourceNames()
                .Select(x => new File(x, selector(x)))
                .Where(x => !string.IsNullOrEmpty(x.Path))
                .ToDictionary(x => x.Path);
        }

        public override bool FileExists(string virtualPath)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            if (_assemblyFiles.ContainsKey(absolutePath)) return true;
            return Previous.FileExists(absolutePath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            if (_assemblyFiles.ContainsKey(absolutePath)) return null;
            return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

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


