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
using System.Abstract;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
namespace Contoso.Abstract.Mvc
{
    /// <summary>
    /// ServiceLocatorModelBinder
    /// </summary>
    public class ServiceLocatorModelBinder : DefaultModelBinder
    {
        private static readonly object _lock = new object();
        private static IEnumerable<IInjectableModelBinder> _modelBinders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorModelBinder"/> class.
        /// </summary>
        public ServiceLocatorModelBinder()
            : this(ServiceLocatorManager.Current) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorModelBinder"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public ServiceLocatorModelBinder(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            ServiceLocator = serviceLocator;
        }

        /// <summary>
        /// Binds the model by using the specified controller context and binding context.
        /// </summary>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param>
        /// <param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param>
        /// <returns>
        /// The bound object.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="bindingContext "/>parameter is null.</exception>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var controllerType = controllerContext.Controller.GetType();
            IEnumerable<IInjectableModelBinder> modelBinders;
            if (!ServiceLocatorManager.HasIgnoreServiceLocator(controllerType) && (modelBinders = GetModelBinders()) != null)
                foreach (var modelBinder in modelBinders.Where(x => x.InjectForModelType(bindingContext.ModelType)))
                    return modelBinder.BindModel(controllerContext, bindingContext);
            return base.BindModel(controllerContext, bindingContext);
        }

        /// <summary>
        /// Gets the model binders.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IInjectableModelBinder> GetModelBinders()
        {
            if (_modelBinders == null)
                lock (_lock)
                    if (_modelBinders == null)
                        _modelBinders = ServiceLocator.ResolveAll<IInjectableModelBinder>();
            return _modelBinders;
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }
    }
}
#endif