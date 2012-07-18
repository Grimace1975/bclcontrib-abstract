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
using System.Abstract;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace Contoso.Abstract.Mvc
{
    /// <summary>
    /// ServiceLocatorControllerFactory
    /// </summary>
    public class ServiceLocatorControllerFactory : DefaultControllerFactory
    {
        private static readonly object _lock = new object();
        private static IActionInvoker _actionInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorControllerFactory"/> class.
        /// </summary>
        public ServiceLocatorControllerFactory()
            : this(ServiceLocatorManager.Current) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorControllerFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public ServiceLocatorControllerFactory(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            ServiceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets the action invoker.
        /// </summary>
        /// <returns></returns>
        protected virtual IActionInvoker GetActionInvoker()
        {
            if (_actionInvoker == null)
                lock (_lock)
                    if (_actionInvoker == null)
                        try { _actionInvoker = ServiceLocator.Resolve<IActionInvoker>(); }
                        catch (ServiceLocatorResolutionException) { _actionInvoker = new ServiceLocatorActionInvoker(ServiceLocator); }
            return _actionInvoker;
        }

        /// <summary>
        /// Retrieves the controller instance for the specified request context and controller type.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>
        /// The controller instance.
        /// </returns>
        /// <exception cref="T:System.Web.HttpException">
        ///   <paramref name="controllerType"/> is null.</exception>
        ///   
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="controllerType"/> cannot be assigned.</exception>
        ///   
        /// <exception cref="T:System.InvalidOperationException">An instance of <paramref name="controllerType"/> cannot be created.</exception>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            // skips and calls to base if controllerType = null, throw standard MVC exception
            if (!ServiceLocatorManager.HasIgnoreServiceLocator(controllerType))
            {
                var controller = ServiceLocator.Resolve<IController>(controllerType);
                var controllerAsController = (controller as Controller);
                if (controllerAsController != null)
                    controllerAsController.ActionInvoker = GetActionInvoker();
                return controller;
            }
            return base.GetControllerInstance(requestContext, controllerType);
        }

        /// <summary>
        /// Releases the specified controller.
        /// </summary>
        /// <param name="controller">The controller to release.</param>
        public override void ReleaseController(IController controller)
        {
            if (!ServiceLocatorManager.HasIgnoreServiceLocator(controller))
            {
                var disposable = (controller as IDisposable);
                if (disposable != null)
                    disposable.Dispose();
                ServiceLocator.Release(controller);
            }
            base.ReleaseController(controller);
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }
    }
}
#endif