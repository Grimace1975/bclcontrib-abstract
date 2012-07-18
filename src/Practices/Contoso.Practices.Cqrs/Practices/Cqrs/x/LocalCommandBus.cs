//#region License
// *
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//namespace Contoso.Practices.Cqrs.Command
//{
//    public class LocalCommandBus : ICommandBus
//    {
//        private readonly IDictionary<Type, CommandHandlerInvoker> commandInvokers;
        
//        public LocalCommandBus(ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
//        {
//            ThrowIfNoCommandHandlers = true;
//            commandInvokers =
//                CommandInvokerDictionaryBuilderHelpers.CreateADictionaryOfCommandInvokers(typeCatalog, serviceLocator);
//        }

//        public bool ThrowIfNoCommandHandlers { get; set; }

//        public int Execute<TCommand>(TCommand command) where TCommand : ICommand
//        {
//            var commandHandler = GetTheCommandHandler(command);
//            return commandHandler.Execute(command);
//        }

//        public void Send<TCommand>(TCommand command) where TCommand : ICommand
//        {
//            var commandHandler = GetTheCommandHandler(command);
//            commandHandler.Send(command);
//        }

//        private CommandHandlerInvoker GetTheCommandHandler(ICommand command)
//        {
//            CommandHandlerInvoker commandInvoker;
//            if(!commandInvokers.TryGetValue(command.GetType(), out commandInvoker) && ThrowIfNoCommandHandlers)
//                throw new CommandHandlerNotFoundException(command.GetType());
//            return commandInvoker;
//        }

//        public class CommandHandlerInvoker
//        {
//            private readonly Type commandHandlerType;
//            private readonly Type commandType;
//            private readonly IServiceLocator serviceLocator;

//            public CommandHandlerInvoker(IServiceLocator serviceLocator, Type commandType, Type commandHandlerType)
//            {
//                this.serviceLocator = serviceLocator;
//                this.commandType = commandType;
//                this.commandHandlerType = commandHandlerType;
//            }

//            public int Execute(ICommand command)
//            {
//                var handlingContext = CreateTheCommandHandlingContext(command);
//                ExecuteTheCommandHandler(handlingContext);
//                return ((ICommandHandlingContext)handlingContext).ReturnValue;
//            }

//            public void Send(ICommand command)
//            {
//                var handlingContext = CreateTheCommandHandlingContext(command);
//                ExecuteTheCommandHandler(handlingContext);
//            }

//            private void ExecuteTheCommandHandler(ICommandHandlingContext<ICommand> handlingContext)
//            {
//                var handleMethod = GetTheHandleMethod();
//                var commandHandler = CreateTheCommandHandler();
//                handleMethod.Invoke(commandHandler, new object[] {handlingContext});
//            }

//            private ICommandHandlingContext<ICommand> CreateTheCommandHandlingContext(ICommand command)
//            {
//                var handlingContextType = typeof(CommandHandlingContext<>).MakeGenericType(commandType);
//                return (ICommandHandlingContext<ICommand>)Activator.CreateInstance(handlingContextType, command);
//            }

//            private object CreateTheCommandHandler()
//            {
//                return serviceLocator.Resolve(commandHandlerType);
//            }

//            private MethodInfo GetTheHandleMethod()
//            {
//                return typeof(IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
//            }
//        }

//        private interface ICommandHandlingContext
//        {
//            ICommand Command { get; }
//            int ReturnValue { get; }
//        }

//        private class CommandHandlingContext<TCommand> : ICommandHandlingContext, ICommandHandlingContext<TCommand>
//            where TCommand : ICommand
//        {
//            public CommandHandlingContext(TCommand command)
//            {
//                Command = command;
//            }

//            public TCommand Command { get; private set; }

//            ICommand ICommandHandlingContext.Command
//            {
//                get { return Command; }
//            }

//            public int ReturnValue { get; private set; }

//            public void Return(int value)
//            {
//                ReturnValue = value;
//            }
//        }
//    }
//}