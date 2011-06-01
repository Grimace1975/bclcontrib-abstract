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
namespace System.Abstract
{
	/// <summary>
	/// ServiceBusManager
	/// </summary>
	public class ServiceBusManager : ServiceManagerBase<IServiceBus, Action<IServiceBus>>
	{
		static ServiceBusManager()
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
				ServiceLocatorRegistrar = (locator, name) => (service =>
				{
					var locator2 = locator();
					RegisterInstance(locator2, service, name);
					var publishingServiceBus = (service as IPublishingServiceBus);
					if (publishingServiceBus != null)
						RegisterInstance(locator2, publishingServiceBus, name);
				}),
			};
		}
		public static void EnsureRegistration() { }
		public static ISetupDescriptor GetSetupDescriptor(Lazy<IServiceBus> service) { return ProtectedGetSetupDescriptor(service); }
	}
}
