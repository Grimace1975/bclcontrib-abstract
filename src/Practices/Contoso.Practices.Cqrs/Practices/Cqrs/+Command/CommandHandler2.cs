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
using System.Abstract.EventSourcing;
namespace Contoso.Practices.Cqrs
{
	/// <summary>
	/// CommandHandler
	/// </summary>
	public abstract class CommandHandler<TCommand, TAggregate> : CommandHandler<TCommand>
		where TCommand : class, ICommandWithAggregate
		where TAggregate : AggregateRoot
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler&lt;TCommand, TAggregate&gt;"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CommandHandler(ICqrsContext context)
            : base(context) { }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
		public override void Handle(TCommand command)
		{
			var aggregateRepository = GetAggregateRepository();
			if (aggregateRepository == null)
				throw new NullReferenceException("aggregateRepository");
			var aggregate = aggregateRepository.GetByID<TAggregate>(command.AggregateID);
			if (aggregate != null)
			{
				Handle(command, aggregate);
				if (aggregate.HasChanged)
					aggregateRepository.Save(aggregate);
			}
			HandleError(command, HandlerErrorIntent.NullAggregate, command.AggregateID);
		}

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="aggregate">The aggregate.</param>
		protected abstract void Handle(TCommand command, TAggregate aggregate);

		private IAggregateRootRepository GetAggregateRepository()
		{
            return Context.AggregateRepository;
        }
	}
}