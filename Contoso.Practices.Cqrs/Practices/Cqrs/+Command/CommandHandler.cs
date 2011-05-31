//#region License
///*
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
//    public abstract class CommandHandler<TCommand> : IHandleCommands<TCommand>
//        where TCommand : ICommand
//    {
//        private ICommandHandlingContext<TCommand> context;

//        void IHandleCommands<TCommand>.Handle(ICommandHandlingContext<TCommand> handlingContext)
//        {
//            context = handlingContext;
//            Handle(handlingContext.Command);
//        }

//        public abstract void Handle(TCommand command);

//        protected void Return(int value)
//        {
//            context.Return(value);
//        }

//        protected void Return(Enum value)
//        {
//            context.Return(Convert.ToInt32(value));
//        }
//    }
//}