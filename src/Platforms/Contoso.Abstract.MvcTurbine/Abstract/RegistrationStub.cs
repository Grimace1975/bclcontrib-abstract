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
using MvcTurbine.ComponentModel;
using System.Collections.Generic;

namespace Contoso.Abstract
{
    /// <summary>
    /// RegistrationStub
    /// </summary>
    internal sealed class RegistrationStub : IServiceRegistrar, IDisposable
    {
        public void Dispose() { }
        public void Register<Interface, Implementation>()
            where Implementation : class, Interface { throw new NotImplementedException(); }
        public void Register<Interface>(Func<Interface> factoryMethod)
            where Interface : class { throw new NotImplementedException(); }
        public void Register<Interface, Implementation>(string key)
            where Implementation : class, Interface { throw new NotImplementedException(); }
        public void Register<Interface>(Type implType)
            where Interface : class { throw new NotImplementedException(); }
        public void Register<Interface>(Interface instance)
            where Interface : class { throw new NotImplementedException(); }
        public void Register(string key, Type type) { throw new NotImplementedException(); }
        public void Register(Type serviceType, Type implType) { throw new NotImplementedException(); }
        public void Register(Type serviceType, Type implType, string key) { throw new NotImplementedException(); }
        public void RegisterAll<Interface>() { throw new NotImplementedException(); }
    }
}
