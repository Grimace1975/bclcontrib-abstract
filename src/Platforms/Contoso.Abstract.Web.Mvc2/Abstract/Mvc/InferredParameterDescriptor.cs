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
#if !CLR4
using System;
using System.Web.Mvc;
namespace Contoso.Abstract.Mvc
{
    /// <summary>
    /// InferredParameterDescriptor
    /// </summary>
    public class InferredParameterDescriptor : ParameterDescriptor
    {
        private readonly ActionDescriptor _descriptor;
        private readonly string _parameterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="InferredParameterDescriptor"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public InferredParameterDescriptor(ActionDescriptor descriptor, string parameterName)
        {
            _descriptor = descriptor;
            _parameterName = parameterName;
        }

        /// <summary>
        /// Gets the action descriptor.
        /// </summary>
        /// <returns>The action descriptor.</returns>
        public override ActionDescriptor ActionDescriptor
        {
            get { return _descriptor; }
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <returns>The name of the parameter.</returns>
        public override string ParameterName
        {
            get { return _parameterName; }
        }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <returns>The type of the parameter.</returns>
        public override Type ParameterType
        {
            get { return typeof(string); }
        }
    }
}
#endif