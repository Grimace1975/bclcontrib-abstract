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
using System.Runtime.Serialization;
namespace System.Abstract
{
    /// <summary>
    /// ServiceLocatorResolutionException
    /// </summary>
    [Serializable]
    public class ServiceLocatorResolutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorResolutionException"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceLocatorResolutionException(Type service)
            : base(string.Format(Local.InvalidServiceTypeA, service)) { ServiceType = service; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorResolutionException"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="arg0">The arg0.</param>
        public ServiceLocatorResolutionException(Type service, string arg0)
            : base(string.Format(Local.InvalidServiceTypeAB, service, arg0)) { ServiceType = service; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorResolutionException"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceLocatorResolutionException(Type service, Exception innerException)
            : base(string.Format(Local.InvalidServiceTypeA, service), innerException) { ServiceType = service; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorResolutionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        ///   </exception>
        protected ServiceLocatorResolutionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type ServiceType { get; set; }
    }
}
