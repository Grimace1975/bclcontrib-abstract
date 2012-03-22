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
using Microsoft.SharePoint;
namespace System.Abstract
{
    /// <summary>
    /// SPServiceLocatorExtensions
    /// </summary>
    public static class SPServiceLocatorExtensions
    {
        #region BehaveAs

        public static IServiceLocator BehaveAs(this IServiceLocator locator, SPSite site)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            var locatorAsCloneable = (locator as ICloneable);
            if (locatorAsCloneable == null)
                throw new ArgumentNullException("locator", "Provider must have ICloneable");
            var newLocator = (IServiceRegistrar)locatorAsCloneable.Clone();
            var newLocatorAsAccessor = (newLocator as ISPServiceBehaviorAccessor);
            if (newLocatorAsAccessor == null)
                throw new ArgumentNullException("locator", "Provider must have ISPServiceBehaviorAccessor");
            newLocatorAsAccessor.SetContainer(site);
            return (IServiceLocator)newLocator;
        }
        public static IServiceLocator BehaveAsFarm(this IServiceLocator locator)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            var locatorAsCloneable = (locator as ICloneable);
            if (locatorAsCloneable == null)
                throw new ArgumentNullException("locator", "Provider must have ICloneable");
            var newLocator = (IServiceRegistrar)locatorAsCloneable.Clone();
            var newLocatorAsAccessor = (newLocator as ISPServiceBehaviorAccessor);
            if (newLocatorAsAccessor == null)
                throw new ArgumentNullException("locator", "Provider must have ISPServiceBehaviorAccessor");
            newLocatorAsAccessor.SetContainerAsFarm();
            return (IServiceLocator)newLocator;
        }

        public static IServiceRegistrar BehaveAs(this IServiceRegistrar registrar, SPSite site)
        {
            if (registrar == null)
                throw new ArgumentNullException("registrar");
            var registrarAsCloneable = (registrar as ICloneable);
            if (registrarAsCloneable == null)
                throw new ArgumentNullException("registrar", "Provider must have ICloneable");
            var newRegistrar = (IServiceRegistrar)registrarAsCloneable.Clone();
            var newRegistrarAsAccessor = (newRegistrar as ISPServiceBehaviorAccessor);
            if (newRegistrarAsAccessor == null)
                throw new ArgumentNullException("registrar", "Provider must have ISPServiceBehaviorAccessor");
            newRegistrarAsAccessor.SetContainer(site);
            return (IServiceRegistrar)newRegistrar;
        }
        public static IServiceRegistrar BehaveAsFarm(this IServiceRegistrar registrar)
        {
            if (registrar == null)
                throw new ArgumentNullException("registrar");
            var registrarAsCloneable = (registrar as ICloneable);
            if (registrarAsCloneable == null)
                throw new ArgumentNullException("registrar", "Provider must have ICloneable");
            var newRegistrar = (IServiceRegistrar)registrarAsCloneable.Clone();
            var newRegistrarAsAccessor = (newRegistrar as ISPServiceBehaviorAccessor);
            if (newRegistrarAsAccessor == null)
                throw new ArgumentNullException("registrar", "Provider must have ISPServiceBehaviorAccessor");
            newRegistrarAsAccessor.SetContainerAsFarm();
            return (IServiceRegistrar)newRegistrar;
        }

        #endregion
    }
}