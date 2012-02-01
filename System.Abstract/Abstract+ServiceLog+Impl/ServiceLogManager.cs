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
using System.Abstract.Parts;
using Contoso.Abstract;
namespace System.Abstract
{
    /// <summary>
    /// ServiceLogManager
    /// </summary>
    public class ServiceLogManager : ServiceManagerBase<IServiceLog, Action<IServiceLog>>
    {
        public static readonly Lazy<IServiceLog> EmptyServiceLog = new Lazy<IServiceLog>(() => new EmptyServiceLog());

        static ServiceLogManager()
        {
            Registration = new SetupRegistration
            {
                OnSetup = (service, descriptor) =>
                {
                    if (descriptor != null)
                        foreach (var action in descriptor.Actions)
                            action(service);
                    return service;
                },
            };
            // default provider
            if (Lazy == null)
                SetProvider(() => new ConsoleServiceLog("Default"));
        }

        public static IServiceLog Current
        {
            get
            {
                if (Lazy == null)
                    throw new InvalidOperationException("Service undefined. Ensure SetProvider");
                return Lazy.Value;
            }
        }

        public static void EnsureRegistration() { }
        public static ISetupDescriptor GetSetupDescriptor(Lazy<IServiceLog> service) { return ProtectedGetSetupDescriptor(service, null); }

        public static IServiceLog Get<T>() { return (ServiceLogManager.Lazy ?? EmptyServiceLog).Value.Get<T>(); }
        public static IServiceLog Get(string name) { return (ServiceLogManager.Lazy ?? EmptyServiceLog).Value.Get(name); }
    }
}
