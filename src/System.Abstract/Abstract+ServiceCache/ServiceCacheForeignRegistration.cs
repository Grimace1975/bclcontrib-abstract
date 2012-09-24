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
    /// ServiceCacheForeignRegistration
    /// </summary>
    public class ServiceCacheForeignRegistration : IServiceCacheRegistration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCacheForeignRegistration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServiceCacheForeignRegistration(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCacheForeignRegistration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="foreignType">Type of the foreign.</param>
        /// <param name="foreignName">Name of the foreign.</param>
        public ServiceCacheForeignRegistration(string name, Type foreignType, string foreignName)
        {
            Name = name;
            ForeignName = foreignName;
            ForeignType = foreignType;
        }

        /// <summary>
        /// Gets the name of the absolute.
        /// </summary>
        /// <value>
        /// The name of the absolute.
        /// </value>
        public string AbsoluteName { get; internal set; }
        /// <summary>
        /// Gets the use headers.
        /// </summary>
        /// <value>
        /// The use headers.
        /// </value>
        public bool UseHeaders { get { return false; } }
        /// <summary>
        /// Gets the registrar.
        /// </summary>
        /// <value>
        /// The registrar.
        /// </value>
        public ServiceCacheRegistrar Registrar { get; internal set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the foreign type.
        /// </summary>
        /// <value>
        /// The type of the foreign.
        /// </value>
        public Type ForeignType { get; set; }

        /// <summary>
        /// Gets or sets the name of the foreign.
        /// </summary>
        /// <value>
        /// The name of the foreign.
        /// </value>
        public string ForeignName { get; set; }

        /// <summary>
        /// Attaches the registrar.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="absoluteName">Name of the absolute.</param>
        public void AttachRegistrar(ServiceCacheRegistrar registrar, string absoluteName)
        {
            Registrar = registrar;
            AbsoluteName = absoluteName;
        }
    }
}
