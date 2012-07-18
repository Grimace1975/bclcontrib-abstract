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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.SharePoint.Common;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
namespace Contoso.Abstract.SPG2010
{
    /// <summary>
    /// ActivatingServiceLocatorFactory
    /// </summary>
    public class ActivatingServiceLocatorFactory : IServiceLocatorFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public IServiceLocator Create()
        {
            return new ActivatingServiceLocator();
        }

        /// <summary>
        /// Loads the type mappings.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="typeMappings">The type mappings.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void LoadTypeMappings(IServiceLocator serviceLocator, IEnumerable<TypeMapping> typeMappings)
        {
            if (typeMappings == null)
                return;
            Validation.ArgumentNotNull(serviceLocator, "serviceLocator");
            Validation.TypeIsAssignable(typeof(ActivatingServiceLocator), serviceLocator.GetType(), "serviceLocator");
            ActivatingServiceLocator activatingServiceLocator = serviceLocator as ActivatingServiceLocator;
            foreach (TypeMapping typeMapping in typeMappings)
                activatingServiceLocator.RegisterTypeMapping(typeMapping);
        }
    }
}
