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
using System.Abstract.Parts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
namespace Contoso.Abstract.Parts
{
    /// <summary>
    /// BuildManagerWrapper
    /// </summary>
    public class BuildManagerWrapper : IBuildManager
    {
        private static readonly Func<string, Stream> _createCachedFileDelegate = CreateDelegate<Func<string, Stream>>(typeof(BuildManager), "CreateCachedFile", null);
        private static readonly Func<string, Stream> _readCachedFileDelegate = CreateDelegate<Func<string, Stream>>(typeof(BuildManager), "ReadCachedFile", null);

        Stream IBuildManager.CreateCachedFile(string fileName)
        {
            return (_createCachedFileDelegate == null ? null : _createCachedFileDelegate(fileName));
        }

        object IBuildManager.CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType)
        {
            return BuildManager.CreateInstanceFromVirtualPath(virtualPath, requiredBaseType);
        }

        IEnumerable<Assembly> IBuildManager.GetReferencedAssemblies()
        {
            return BuildManager.GetReferencedAssemblies().OfType<Assembly>();
        }

        Stream IBuildManager.ReadCachedFile(string fileName)
        {
            return (_readCachedFileDelegate == null ? null : _readCachedFileDelegate(fileName));
        }

        private static TDelegate CreateDelegate<TDelegate>(Type targetType, string methodName, object thisParameter)
            where TDelegate : class
        {
            var types = Array.ConvertAll<ParameterInfo, Type>(typeof(TDelegate).GetMethod("Invoke").GetParameters(), pInfo => pInfo.ParameterType);
            var method = targetType.GetMethod(methodName, types);
            return (method != null ? Delegate.CreateDelegate(typeof(TDelegate), thisParameter, method, false) as TDelegate : default(TDelegate));
        }
    }
}
