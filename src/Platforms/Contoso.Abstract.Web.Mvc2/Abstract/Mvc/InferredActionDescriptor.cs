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
using System.Collections.Generic;
using System.Web.Mvc;
namespace Contoso.Abstract.Mvc
{
    /// <summary>
    /// InferredActionDescriptor
    /// </summary>
    public class InferredActionDescriptor : ActionDescriptor
    {
        private readonly string _actionName;
        private readonly ControllerDescriptor _controllerDescriptor;
        private readonly InferredAction _inferredAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="InferredActionDescriptor"/> class.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <param name="inferredAction">The inferred action.</param>
        public InferredActionDescriptor(string actionName, ControllerDescriptor controllerDescriptor, InferredAction inferredAction)
        {
            _actionName = actionName;
            _controllerDescriptor = controllerDescriptor;
            _inferredAction = inferredAction;
        }

        /// <summary>
        /// Executes the action method by using the specified parameters and controller context.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="parameters">The parameters of the action method.</param>
        /// <returns>
        /// The result of executing the action method.
        /// </returns>
        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            Action<ControllerContext> onExecuting;
            if (InferredAction != null && (onExecuting = InferredAction.OnExecuting) != null)
                onExecuting(controllerContext);
            return new InferredViewResult { ViewName = ActionName };
        }

        /// <summary>
        /// Returns the parameters of the action method.
        /// </summary>
        /// <returns>
        /// The parameters of the action method.
        /// </returns>
        public override ParameterDescriptor[] GetParameters() { return new ParameterDescriptor[] { new InferredParameterDescriptor(this, ActionName) }; }

        /// <summary>
        /// Gets the name of the action method.
        /// </summary>
        /// <returns>The name of the action method.</returns>
        public override string ActionName
        {
            get { return _actionName; }
        }

        /// <summary>
        /// Gets the controller descriptor.
        /// </summary>
        /// <returns>The controller descriptor.</returns>
        public override ControllerDescriptor ControllerDescriptor
        {
            get { return _controllerDescriptor; }
        }

        /// <summary>
        /// Gets the inferred action.
        /// </summary>
        public InferredAction InferredAction { get; private set; }
    }
}
#endif