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
namespace Contoso.Practices.Cqrs.Command
{
    /// <summary>
    /// RegistryCommandDispatcher
    /// </summary>
    public class RegistryCommandDispatcher : ICommandDispatcher
    {
        //public static IDictionary<Type, LocalCommandBus.CommandHandlerInvoker> CreateADictionaryOfCommandInvokers(
        //    ITypeCatalog typeCatalog, IServiceLocator serviceLocator)
        //{
        //    var types = GetAllCommandHandlerTypes(typeCatalog);
        //    return CreateCommandInvokersForTheseTypes(types, serviceLocator);
        //}

        //private static IEnumerable<Type> GetAllCommandHandlerTypes(ITypeCatalog typeCatalog)
        //{
        //    return typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>));
        //}

        //private static IDictionary<Type, LocalCommandBus.CommandHandlerInvoker> CreateCommandInvokersForTheseTypes(
        //    IEnumerable<Type> commandHandlerTypes, IServiceLocator serviceLocator)
        //{
        //    var commandInvokerDictionary = new Dictionary<Type, LocalCommandBus.CommandHandlerInvoker>();
        //    foreach (var commandHandlerType in commandHandlerTypes)
        //    {
        //        var commandTypes = GetCommandTypesForCommandHandler(commandHandlerType);
        //        foreach (var commandType in commandTypes)
        //        {
        //            if (commandInvokerDictionary.ContainsKey(commandType))
        //                throw new DuplicateCommandHandlersException(commandType);

        //            commandInvokerDictionary.Add(commandType,
        //                                         new LocalCommandBus.CommandHandlerInvoker(serviceLocator, commandType,
        //                                                                                   commandHandlerType));
        //        }
        //    }
        //    return commandInvokerDictionary;
        //}

        //private static IEnumerable<Type> GetCommandTypesForCommandHandler(Type commandHandlerType)
        //{
        //    return (from interfaceType in commandHandlerType.GetInterfaces()
        //            where
        //                interfaceType.IsGenericType &&
        //                interfaceType.GetGenericTypeDefinition() == typeof(IHandleCommands<>)
        //            select interfaceType.GetGenericArguments()[0]).ToArray();
        //}
    }
}