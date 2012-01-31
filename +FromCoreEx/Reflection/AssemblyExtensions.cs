﻿#region License
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
using System.Collections.Generic;
namespace System.Reflection
{
    /// <summary>
    /// AssemblyExtensions
    /// </summary>
#if COREINTERNAL
    internal
#else
    public
#endif
 static class AssemblyExtensions
    {
        public static IEnumerable<Type> AsTypes(this Assembly assembly, Predicate<Type> predicate)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            foreach (var type in assembly.GetTypes())
                if (predicate == null || predicate(type))
                    yield return type;
        }

        public static IEnumerable<Type> AsTypes<TBasedOn>(this Assembly assembly) { return AsTypes(assembly, typeof(TBasedOn), null); }
        public static IEnumerable<Type> AsTypes<TBasedOn>(this Assembly assembly, Predicate<Type> predicate) { return AsTypes(assembly, typeof(TBasedOn), null); }
        public static IEnumerable<Type> AsTypes(this Assembly assembly, Type basedOnType) { return AsTypes(assembly, basedOnType, null); }
        public static IEnumerable<Type> AsTypes(this Assembly assembly, Type basedOnType, Predicate<Type> predicate)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            foreach (var t in assembly.GetTypes())
                if (basedOnType.IsAssignableFrom(t) && !t.Equals(basedOnType) && !t.IsInterface && !t.IsAbstract && (predicate == null || predicate(t)))
                    yield return t;
        }
    }
}