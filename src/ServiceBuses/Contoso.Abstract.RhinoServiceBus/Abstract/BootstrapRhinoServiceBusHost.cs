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
using Contoso.Abstract.RhinoServiceBus;
namespace Contoso.Abstract
{
    /// <summary>
    /// BootstrapRhinoServiceBusHost
    /// </summary>
    public abstract class BootstrapRhinoServiceBusHost : ServiceLocatorBootStrapper, IServiceBusHostBootstrap
    {
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapRhinoServiceBusHost"/> class.
        /// </summary>
        protected BootstrapRhinoServiceBusHost()
            : base(ServiceLocatorManager.Current) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapRhinoServiceBusHost"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        protected BootstrapRhinoServiceBusHost(IServiceLocator locator)
            : base(locator) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapRhinoServiceBusHost"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        protected BootstrapRhinoServiceBusHost(IServiceLocator locator, string name)
            : base(locator) { _name = name; }

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

        /// <summary>
        /// Initializes the container.
        /// </summary>
        public override void InitializeContainer() { Initialize(); base.InitializeContainer(); }
        /// <summary>
        /// Called when [end start].
        /// </summary>
        protected override void OnEndStart()
        {
            base.OnEndStart();
            ServiceBusManager.SetProvider(() => new RhinoServiceBusAbstractor(_locator, GetInstance<Rhino.ServiceBus.IServiceBus>()))
                .RegisterWithServiceLocator(_locator, _name);
            // also opens to connection, thus registering IServiceBus
            Open(ServiceBusManager.Current);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public override void Dispose() { base.Dispose(); Close(); }
    }
}
