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
using IServiceBus = System.Abstract.IServiceBus;
namespace Contoso.Abstract
{
    /// <summary>
    /// BootstrapMTServiceBusHost
    /// </summary>
    public abstract class BootstrapMTServiceBusHost : IServiceBusHostBootstrap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapMTServiceBusHost"/> class.
        /// </summary>
        protected BootstrapMTServiceBusHost() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapMTServiceBusHost"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        protected BootstrapMTServiceBusHost(IServiceLocator locator)
        {
            ServiceBusManager.SetProvider(() => new MTServiceBusAbstractor(locator));
        }

        //public virtual void Start(MassTransit.IServiceBus bus) { Bus = bus; }
        //public virtual void Stop() { }
        //public virtual void Dispose()
        //{
        //    if (Bus != null) { Bus.Dispose(); Bus = null; }
        //}

        /// <summary>
        /// Gets or sets the bus.
        /// </summary>
        /// <value>
        /// The bus.
        /// </value>
        public MassTransit.IServiceBus Bus { get; set; }

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
    }
}
