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
using System.Collections.Generic;
namespace System.Abstract
{
    /// <summary>
    /// IServiceLocator
    /// </summary>
    public interface IServiceLocator : IServiceProvider
    {
        /// <summary>
        /// Gets the underlying container.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <returns></returns>
        TContainer GetUnderlyingContainer<TContainer>()
            where TContainer : class;

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <returns></returns>
        IServiceLocator CreateChild(object tag);

        // registrar
        /// <summary>
        /// Gets the registrar.
        /// </summary>
        IServiceRegistrar Registrar { get; }

        // resolve
        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        TService Resolve<TService>()
            where TService : class;
        /// <summary>
        /// Resolves the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        TService Resolve<TService>(string name)
            where TService : class;
        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        object Resolve(Type serviceType);
        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        object Resolve(Type serviceType, string name);
        //
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        IEnumerable<TService> ResolveAll<TService>()
            where TService : class;
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        IEnumerable<object> ResolveAll(Type serviceType);

        // inject
        /// <summary>
        /// Injects the specified instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        TService Inject<TService>(TService instance)
            where TService : class;

        // release and teardown
        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void Release(object instance);
        /// <summary>
        /// Tears down.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        void TearDown<TService>(TService instance)
            where TService : class;
    }
}