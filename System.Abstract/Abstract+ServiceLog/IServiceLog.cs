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
	/// IServiceLog
	/// </summary>
	public interface IServiceLog : IServiceProvider
	{
		IServiceLog Get(Type type);
		IServiceLog Get(string name);
        //void Trace(string s, params object[] args);
        void Debug(string s, params object[] args);
        void Debug(string s, Exception ex);
		void Warning(string s, params object[] args);
		void Warning(string s, Exception ex);
		void Error(string s, params object[] args);
		void Error(string s, Exception ex);

        //public static void Log(params object[] args) { }
        //public static void Info(params object[] args) { }
        //public static void Warn(params object[] args) { }
        //public static void Error(params object[] args) { }

	}

	/// <summary>
	/// IServiceLogExtensions
	/// </summary>
	public static class IServiceLogExtensions
	{
		public static IServiceLog Get<T>(this IServiceLog service) { return service.Get(typeof(T)); }

		//public static IServiceLog Warning(this IServiceLog service) { return service.Get(typeof(T)); }
		//public static IServiceLog Error(this IServiceLog service) { return service.Get(typeof(T)); }
		//public static IServiceLog Debug(this IServiceLog service, string) { return service.Get(typeof(T)); }

		#region Lazy Setup

		public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, null); return service; }
		public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, string name) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, name); return service; }
		public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, Lazy<IServiceLocator> locator) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
		public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, Lazy<IServiceLocator> locator, string name) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

		#endregion
	}
}
