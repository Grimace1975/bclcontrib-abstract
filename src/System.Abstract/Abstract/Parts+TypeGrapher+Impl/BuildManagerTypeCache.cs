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
    /// BuildManagerTypeCache
    /// </summary>
    public class BuildManagerTypeCache : ITypeCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildManagerTypeCache"/> class.
        /// </summary>
        /// <param name="buildManager">The build manager.</param>
        public BuildManagerTypeCache(IBuildManager buildManager)
            : this(buildManager, new TypeCacheSerializer()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildManagerTypeCache"/> class.
        /// </summary>
        /// <param name="buildManager">The build manager.</param>
        /// <param name="serializer">The serializer.</param>
        public BuildManagerTypeCache(IBuildManager buildManager, ITypeCacheSerializer serializer)
        {
            if (buildManager == null)
                throw new ArgumentNullException("buildManager");
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            BuildManager = buildManager;
            Serializer = serializer;
        }

        /// <summary>
        /// Gets the build manager.
        /// </summary>
        public IBuildManager BuildManager { get; private set; }
        /// <summary>
        /// Gets the serializer.
        /// </summary>
        public ITypeCacheSerializer Serializer { get; private set; }

        /// <summary>
        /// Gets the filtered types from assemblies.
        /// </summary>
        /// <param name="cacheName">Name of the cache.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IEnumerable<Type> GetFilteredTypesFromAssemblies(string cacheName, Predicate<Type> predicate) { return GetFilteredTypesFromAssemblies(cacheName, predicate, BuildManager.GetReferencedAssemblies()); }
        /// <summary>
        /// Gets the filtered types from assemblies.
        /// </summary>
        /// <param name="cacheName">Name of the cache.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public IEnumerable<Type> GetFilteredTypesFromAssemblies(string cacheName, Predicate<Type> predicate, IEnumerable<Assembly> assemblies)
        {
            var matchingTypes = TypeCacheManager.ReadTypesFromCache(BuildManager, cacheName, predicate, Serializer);
            if (matchingTypes == null)
            {
                matchingTypes = FilterTypesInAssemblies(BuildManager, predicate, assemblies)
                    .ToList();
                TypeCacheManager.SaveTypesToCache(BuildManager, cacheName, matchingTypes, Serializer);
            }
            return matchingTypes;
        }

        private static IEnumerable<Type> FilterTypesInAssemblies(IBuildManager buildManager, Predicate<Type> predicate, IEnumerable<Assembly> assemblies)
        {
            IEnumerable<Type> foundTypes = Type.EmptyTypes;
            foreach (var assembly in assemblies)
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException exception) { types = exception.Types; }
                foundTypes = foundTypes.Concat(types.Where(type => TypeCacheManager.TypeIsPublicClass(type) && predicate(type)));
            }
            return foundTypes;
        }
    }
}