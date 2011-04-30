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
    /// ServiceLocatorManager
    /// </summary>
    public class ServiceLocatorManager
    {
        private static readonly Type s_wantToSkipServiceLocatorType = typeof(IWantToSkipServiceLocator);

        private static readonly ServiceLocatorInstance _instance = new ServiceLocatorInstance();

        public static IServiceLocatorSetup SetLocatorProvider(Func<IServiceLocator> provider) { return _instance.SetLocatorProvider(provider); }
        public static IServiceLocatorSetup SetLocatorProvider(Func<IServiceLocator> provider, IServiceLocatorSetup setup) { return _instance.SetLocatorProvider(provider, setup); }

        public static IServiceLocatorSetup Setup
        {
            get { return _instance.Setup; }
        }

        public static IServiceLocator Current
        {
            get { return _instance.Current; }
        }

        public static bool GetWantsToSkipLocator(object instance) { return ((instance == null) || (GetWantsToSkipLocator(instance.GetType()))); }
        public static bool GetWantsToSkipLocator<TService>() { return GetWantsToSkipLocator(typeof(TService)); }
        public static bool GetWantsToSkipLocator(Type type)
        {
            return ((type == null) || (s_wantToSkipServiceLocatorType.IsAssignableFrom(type)));
        }
    }
}
