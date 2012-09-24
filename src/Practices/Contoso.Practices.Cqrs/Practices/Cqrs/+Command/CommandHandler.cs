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
using System;
using System.Abstract;
namespace Contoso.Practices.Cqrs
{
    /// <summary>
    /// ICommandHandler
    /// </summary>
    public interface ICommandHandler<TCommand> : IServiceMessageHandler<TCommand>
        where TCommand : class, ICommand { }

    /// <summary>
    /// CommandHandler
    /// </summary>
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler&lt;TCommand&gt;"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CommandHandler(ICqrsContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Context = context;
        }

        void IServiceMessageHandler<TCommand>.Handle(TCommand command)
        {
            var validateResult = Validate(command);
            if (validateResult == null)
                Handle(command);
            else
                HandleError(command, HandlerErrorIntent.ValidationFailure, validateResult);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public ICqrsContext Context { get; private set; }

        /// <summary>
        /// Validates the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        protected virtual object Validate(TCommand command) { return null; }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public abstract void Handle(TCommand command);
        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="errorIntent">The error intent.</param>
        /// <param name="value">The value.</param>
        protected virtual void HandleError(TCommand command, HandlerErrorIntent errorIntent, object value) { throw new InvalidOperationException(); }
    }
}