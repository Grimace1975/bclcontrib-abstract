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
using System.Reflection;
using System.Collections.Generic;
namespace System.Abstract
{
    /// <summary>
    /// ServiceDependencyAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ServiceDependencyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDependencyAttribute"/> class.
        /// </summary>
        public ServiceDependencyAttribute()
            : this(null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDependencyAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServiceDependencyAttribute(string name) { Name = name; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the service dependencies.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public static IEnumerable<ServiceDependencyAttribute> GetServiceDependencies(ParameterInfo parameter)
        {
            return parameter.GetCustomAttributes(false)
                .OfType<ServiceDependencyAttribute>();
        }
        /// <summary>
        /// Gets the service dependencies.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static IEnumerable<ServiceDependencyAttribute> GetServiceDependencies(PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(ServiceDependencyAttribute), false)
                .OfType<ServiceDependencyAttribute>();
        }
    }
}
