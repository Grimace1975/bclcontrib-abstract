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
using System.Abstract;
using NServiceBus;
namespace Contoso.Abstract
{
    /// <summary>
    /// BootstrapNServiceBusHost
    /// </summary>
    public abstract class BootstrapNServiceBusHost : IServiceBusHostBootstrap, IConfigureThisEndpoint, AsA_Publisher, IWantToRunAtStartup, IWantCustomLogging
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapNServiceBusHost"/> class.
        /// </summary>
        protected BootstrapNServiceBusHost() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapNServiceBusHost"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        protected BootstrapNServiceBusHost(IServiceLocator locator)
        {
            ServiceBusManager.SetProvider(() => new NServiceBusAbstractor(locator));
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize() { }
        /// <summary>
        /// Opens the specified bus.
        /// </summary>
        /// <param name="bus">The bus.</param>
        public virtual void Open(IServiceBus bus) { }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void Close() { }

        void IWantToRunAtStartup.Run() { Open(ServiceBusManager.Current); }
        void IWantToRunAtStartup.Stop() { Close(); }
        void IWantCustomLogging.Init() { Initialize(); }
    }
}
