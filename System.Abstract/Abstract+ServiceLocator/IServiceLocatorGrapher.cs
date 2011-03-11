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
using System.Reflection;
using System.Diagnostics;
namespace System.Abstract
{
    /// <summary>
    /// IServiceLocatorGrapher
    /// </summary>
    public interface IServiceLocatorGrapher
    {
        IServiceLocatorGrapher Do(Action<IServiceRegistrar, IServiceLocator> action);
        IServiceLocatorGrapher RegisterFromAssemblies(Predicate<Type> predicate, params Assembly[] assemblies);
        IServiceLocatorGrapher RegisterFromAssembliesByNameConvention(Predicate<Type> predicate, params Assembly[] assemblies);
        void Finally(IServiceRegistrar registrar, IServiceLocator locator);
    }

    /// <summary>
    /// IServiceLocatorGrapherExtensions
    /// </summary>
    public static class IServiceLocatorGrapherExtensions
    {
        public static IServiceLocatorGrapher RegisterByIServiceRegistration(this IServiceLocatorGrapher grapher, params Assembly[] assemblies) { return grapher.RegisterFromAssemblies(null, assemblies); }
        public static IServiceLocatorGrapher RegisterByNamingConvention(this IServiceLocatorGrapher grapher) { return grapher.RegisterFromAssembliesByNameConvention(null, new[] { GetPreviousCallingMethodsAssembly() }); }
        public static IServiceLocatorGrapher RegisterByNamingConvention(this IServiceLocatorGrapher grapher, params Assembly[] assemblies) { return grapher.RegisterFromAssembliesByNameConvention(null, assemblies); }
        public static IServiceLocatorGrapher RegisterByNamingConvention(this IServiceLocatorGrapher grapher, Predicate<Type> predicate) { return grapher.RegisterFromAssembliesByNameConvention(predicate, new[] { GetPreviousCallingMethodsAssembly() }); }

        private static Assembly GetPreviousCallingMethodsAssembly()
        {
            var frame = new StackTrace().GetFrame(2);
            var method = frame.GetMethod();
            return (method != null ? method.ReflectedType.Assembly : null);
        }
    }
}