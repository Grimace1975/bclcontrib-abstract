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
namespace System.Abstract
{
    /// <summary>
    /// ITypeCatalog
    /// </summary>
    public interface ITypeCatalog
    {
        Type[] GetDerivedTypes(Type type, bool concretable);
        Type[] GetInterfaceImplementations(Type type, bool concretable);
        Type[] GetGenericInterfaceImplementations(Type type, bool concretable);
    }

    /// <summary>
    /// ITypeCatalogExtensions
    /// </summary>
    public static class ITypeCatalogExtensions
    {
        public static Type[] GetDerivedTypes<T>(this ITypeCatalog typeCatalog, bool concretable) { return typeCatalog.GetDerivedTypes(typeof(T), concretable); }
        public static Type[] GetInterfaceImplementations<T>(this ITypeCatalog typeCatalog, bool concretable) { return typeCatalog.GetInterfaceImplementations(typeof(T), concretable); }
        public static Type[] GetGenericInterfaceImplementations<T>(this ITypeCatalog typeCatalog, bool concretable) { return typeCatalog.GetGenericInterfaceImplementations(typeof(T), concretable); }
    }
}