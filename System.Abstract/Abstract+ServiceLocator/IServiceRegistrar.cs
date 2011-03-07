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
namespace System.Abstract
{
    /// <summary>
    /// IServiceRegistrar
    /// </summary>
    public interface IServiceRegistrar
    {
        // locator
        IServiceLocator GetLocator();
        TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator;

        // register implementation
        void Register<TService, TImplementation>()
            where TImplementation : class, TService;
        void Register<TService, TImplementation>(string id)
            where TImplementation : class, TService;
        void Register<TService>(Type implementationType)
            where TService : class;
        void Register<TService>(Type implementationType, string id)
            where TService : class;
        void Register(Type serviceType, Type implementationType);
        void Register(Type serviceType, Type implementationType, string id);

        // register id
        void Register(Type serviceType, string id);

        // register instance
        void Register<TService>(TService instance)
            where TService : class;
        void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class;
    }
}
