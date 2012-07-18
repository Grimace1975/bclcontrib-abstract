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
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Abstract;
namespace Contoso.Abstract.Mvc
{
    /// <summary>
    /// ServiceLocatorActionInvoker
    /// </summary>
    public class ServiceLocatorActionInvoker : ControllerActionInvoker
    {
        private static readonly object _lock = new object();
        private static IFilterInfoFinder _filterInfoFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorActionInvoker"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public ServiceLocatorActionInvoker(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            ServiceLocator = serviceLocator;
        }

        /// <summary>
        /// Makes the mergeable filter info.
        /// </summary>
        /// <returns></returns>
        protected virtual MergeableFilterInfo MakeMergeableFilterInfo() { return new MergeableFilterInfo(); }

        /// <summary>
        /// Finds the information about the action method.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <returns>
        /// Information about the action method.
        /// </returns>
        protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName)
        {
            ActionDescriptor descriptor;
            try { descriptor = base.FindAction(controllerContext, controllerDescriptor, actionName); }
            catch { descriptor = null; }
            return (descriptor ?? new InferredActionDescriptor(actionName, controllerDescriptor, FindInferredAction(controllerDescriptor, actionName)));
        }

        /// <summary>
        /// Finds the inferred action.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        protected virtual InferredAction FindInferredAction(ControllerDescriptor controllerDescriptor, string actionName)
        {
            return null;
        }

        /// <summary>
        /// Gets the filter info finder.
        /// </summary>
        /// <returns></returns>
        protected virtual IFilterInfoFinder GetFilterInfoFinder()
        {
            if (_filterInfoFinder == null)
                lock (_lock)
                    if (_filterInfoFinder == null)
                        try { _filterInfoFinder = ServiceLocator.Resolve<IFilterInfoFinder>(); }
                        catch (ServiceLocatorResolutionException) { _filterInfoFinder = new ServiceLocatorFilterInfoFinder(ServiceLocator); }
            return _filterInfoFinder;
        }

        /// <summary>
        /// Retrieves information about the action filters.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>
        /// Information about the action filters.
        /// </returns>
        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filter = (base.GetFilters(controllerContext, actionDescriptor) ?? new FilterInfo());
            InjectDependencies(filter);
            return MakeMergeableFilterInfo()
                .Merge(filter)
                .Merge(GetFilterInfoFinder().FindFilters(actionDescriptor));
        }

        /// <summary>
        /// Injects the dependencies.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public virtual void InjectDependencies(FilterInfo filter)
        {
            var serviceLocator = ServiceLocator;
            foreach (var f in filter.ActionFilters.Where(x => !(x is IController)))
                serviceLocator.Inject<IActionFilter>(f);
            foreach (var f in filter.AuthorizationFilters.Where(x => !(x is IController)))
                serviceLocator.Inject<IAuthorizationFilter>(f);
            foreach (var f in filter.ResultFilters.Where(x => !(x is IController)))
                serviceLocator.Inject<IResultFilter>(f);
            foreach (var f in filter.ExceptionFilters.Where(x => !(x is IController)))
                serviceLocator.Inject<IExceptionFilter>(f);
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }
    }
}
#endif