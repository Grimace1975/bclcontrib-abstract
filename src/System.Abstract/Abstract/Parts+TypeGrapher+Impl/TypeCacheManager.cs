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
        /// <summary>
        /// Reads the types from cache.
        /// </summary>
        /// <param name="buildManager">The build manager.</param>
        /// <param name="cacheName">Name of the cache.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public static IEnumerable<Type> ReadTypesFromCache(IBuildManager buildManager, string cacheName, Predicate<Type> predicate, ITypeCacheSerializer serializer)
        {
            Func<Type, bool> func = type => (TypeIsPublicClass(type) && predicate(type));
            try
            {
                using (var s = buildManager.ReadCachedFile(cacheName))
                    if (s != null)
                        using (var r = new StreamReader(s))
                        {
                            var source = serializer.DeserializeTypes(r);
                            if (source != null)
                                if (source.All(func))
                                    return source;
                        }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Saves the types to cache.
        /// </summary>
        /// <param name="buildManager">The build manager.</param>
        /// <param name="cacheName">Name of the cache.</param>
        /// <param name="matchingTypes">The matching types.</param>
        /// <param name="serializer">The serializer.</param>
        public static void SaveTypesToCache(IBuildManager buildManager, string cacheName, IEnumerable<Type> matchingTypes, ITypeCacheSerializer serializer)
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

        /// <summary>
        /// Types the is public class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool TypeIsPublicClass(Type type)
        {
            return (type != null && type.IsPublic && type.IsClass && !type.IsAbstract);
        }
    }
}