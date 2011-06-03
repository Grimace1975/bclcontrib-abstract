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
using System.Abstract;
using System;
using System.ServiceProcess;
namespace Contoso.Practices.DurableBus.Utilities
{
	/// <summary>
	/// ProcessUtil
	/// </summary>
	public static class ProcessUtil
	{
		private static readonly IServiceLog ServiceLog = ServiceLogManager.Get("NServiceBus.Utils");

		public static void ChangeServiceStatus(ServiceController controller, ServiceControllerStatus status, Action changeStatus)
		{
			if (controller.Status == status)
				ServiceLog.Debug(controller.ServiceName + " status is good: " + Enum.GetName(typeof(ServiceControllerStatus), status));
			else
			{
				ServiceLog.Debug(controller.ServiceName + " status is NOT " + Enum.GetName(typeof(ServiceControllerStatus), status) + ". Changing status...");
				changeStatus();
				var timeout = TimeSpan.FromSeconds(3.0);
				controller.WaitForStatus(status, timeout);
				if (controller.Status != status)
					throw new InvalidOperationException("Unable to change " + controller.ServiceName + " status to " + Enum.GetName(typeof(ServiceControllerStatus), status));
				ServiceLog.Debug(controller.ServiceName + " status changed successfully.");
			}
		}
	}
}