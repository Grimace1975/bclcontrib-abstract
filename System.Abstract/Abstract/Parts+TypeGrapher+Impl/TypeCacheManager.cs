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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
namespace System.Abstract.Parts
{
    /// <summary>
    /// TypeCacheManager
    /// </summary>
    public class TypeCacheManager
    {
        private static IEnumerable<Type> FilterTypesInAssemblies(IBuildManager buildManager, Predicate<Type> predicate)
        {
            IEnumerable<Type> emptyTypes = Type.EmptyTypes;
            foreach (Assembly assembly in buildManager.GetReferencedAssemblies())
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException exception) { types = exception.Types; }
                emptyTypes = emptyTypes.Concat(types);
            }
            return emptyTypes.Where(type => TypeIsPublicClass(type) && predicate(type));
        }

        public static IEnumerable<Type> GetFilteredTypesFromAssemblies(IBuildManager buildManager, string cacheName, Predicate<Type> predicate) { return GetFilteredTypesFromAssemblies(buildManager, cacheName, predicate, new TypeCacheSerializer()); }
        public static IEnumerable<Type> GetFilteredTypesFromAssemblies(IBuildManager buildManager, string cacheName, Predicate<Type> predicate, ITypeCacheSerializer serializer)
        {
            var matchingTypes = ReadTypesFromCache(buildManager, cacheName, predicate, serializer);
            if (matchingTypes == null)
            {
                matchingTypes = FilterTypesInAssemblies(buildManager, predicate).ToList();
                SaveTypesToCache(buildManager, cacheName, matchingTypes, serializer);
            }
            return matchingTypes;
        }

        private static IEnumerable<Type> ReadTypesFromCache(IBuildManager buildManager, string cacheName, Predicate<Type> predicate, ITypeCacheSerializer serializer)
        {
            Func<Type, bool> func = null;
            try
            {
                using (var s = buildManager.ReadCachedFile(cacheName))
                    if (s != null)
                        using (var r = new StreamReader(s))
                        {
                            var source = serializer.DeserializeTypes(r);
                            if (source != null)
                            {
                                if (func == null)
                                    func = type => (TypeIsPublicClass(type) && predicate(type));
                                if (source.All(func))
                                    return source;
                            }
                        }
            }
            catch { }
            return null;
        }

        private static void SaveTypesToCache(IBuildManager buildManager, string cacheName, IEnumerable<Type> matchingTypes, ITypeCacheSerializer serializer)
        {
            try
            {
                using (var s = buildManager.CreateCachedFile(cacheName))
                    if (s != null)
                        using (var w = new StreamWriter(s))
                            serializer.SerializeTypes(matchingTypes, w);
            }
            catch { }
        }

        private static bool TypeIsPublicClass(Type type)
        {
            return ((type != null) && type.IsPublic && type.IsClass && !type.IsAbstract);
        }
    }
}