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
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Abstract;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.ServiceProcess;
namespace Contoso.Practices.DurableBus.Utilities
{
	/// <summary>
	/// MsmqInstallation
	/// </summary>
	public static class MsmqInstallation
	{
        private static readonly IServiceLog ServiceLog = ServiceLogManager.Get("DurableBus.Utilities");
		private const string OcSetup = "OCSETUP";
		private static readonly List<string> RequiredMsmqComponentsXp = new List<string>(new string[] { "msmq_Core", "msmq_LocalStorage" });
		private const string Server2008OcSetupParams = "MSMQ-Server /passive";
		private static readonly List<string> UndesirableMsmqComponentsV4 = new List<string>(new string[] { "msmq_DCOMProxy", "msmq_MQDSServiceInstalled", "msmq_MulticastInstalled", "msmq_RoutingInstalled", "msmq_TriggersInstalled" });
		private static readonly List<string> UndesirableMsmqComponentsXp = new List<string>(new string[] { "msmq_ADIntegrated", "msmq_TriggersService", "msmq_HTTPSupport", "msmq_RoutingSupport", "msmq_MQDSService" });
		private const string Uninstall = " /uninstall";
		private const byte VER_NT_SERVER = 3;
		private const byte VER_NT_WORKSTATION = 1;
		private const string VistaOcSetupParams = "MSMQ-Container;MSMQ-Server /passive";

		#region OS

		private enum OperatingSystem
		{
			DontCare,
			XpOrServer2003,
			Vista,
			Server2008
		}
		private static OperatingSystem GetOperatingSystem()
		{
			var osVersionEx = EnvironmentEx.OSVersionEx;
			switch (osVersionEx.Version.Major)
			{
				case 5:
					return OperatingSystem.XpOrServer2003;
				case 6:
					return (osVersionEx.PlatformProductID != PlatformProductID.Workstation ? OperatingSystem.Server2008 : OperatingSystem.Vista);
			}
			return OperatingSystem.DontCare;
		}

		#endregion

		private static bool HasOnlyNeededComponents(IEnumerable<string> installedComponents)
		{
			var needed = new List<string>(RequiredMsmqComponentsXp);
			foreach (string installedComponent in installedComponents)
			{
				if (UndesirableMsmqComponentsXp.Contains(installedComponent))
				{
					ServiceLog.Warning("Undesirable MSMQ component installed: " + installedComponent);
					return false;
				}
				if (UndesirableMsmqComponentsV4.Contains(installedComponent))
				{
					ServiceLog.Warning("Undesirable MSMQ component installed: " + installedComponent);
					return false;
				}
				needed.Remove(installedComponent);
			}
			return (needed.Count == 0);
		}

		private static void InstallMsmqIfNecessary()
		{
			ServiceLog.Debug("Checking if MSMQ is installed.");
			if (IsMsmqInstalled())
			{
				ServiceLog.Debug("MSMQ is installed.");
				ServiceLog.Debug("Checking that only needed components are active.");
				if (IsInstallationGood())
					ServiceLog.Debug("Installation is good.");
				else
				{
					ServiceLog.Debug("Installation isn't good.");
					ServiceLog.Debug("Going to re-install MSMQ. A reboot may be required.");
					PerformFunctionDependingOnOS(() => Process.Start("OCSETUP", "MSMQ-Container;MSMQ-Server /passive /uninstall"), () => Process.Start("OCSETUP", "MSMQ-Server /passive /uninstall"), new Func<Process>(MsmqInstallation.InstallMsmqOnXpOrServer2003));
					ServiceLog.Debug("Installation of MSMQ successful.");
				}
			}
			else
			{
				ServiceLog.Debug("MSMQ is not installed. Going to install.");
				PerformFunctionDependingOnOS(() => Process.Start("OCSETUP", "MSMQ-Container;MSMQ-Server /passive"), () => Process.Start("OCSETUP", "MSMQ-Server /passive"), new Func<Process>(MsmqInstallation.InstallMsmqOnXpOrServer2003));
				ServiceLog.Debug("Installation of MSMQ successful.");
			}
		}

		private static Process InstallMsmqOnXpOrServer2003()
		{
			var path = Path.GetTempFileName();
			ServiceLog.Debug("Creating installation instruction file.");
			using (var sw = File.CreateText(path))
			{
				sw.WriteLine("[Version]");
				sw.WriteLine("Signature = \"$Windows NT$\"");
				sw.WriteLine();
				sw.WriteLine("[Global]");
				sw.WriteLine("FreshMode = Custom");
				sw.WriteLine("MaintenanceMode = RemoveAll");
				sw.WriteLine("UpgradeMode = UpgradeOnly");
				sw.WriteLine();
				sw.WriteLine("[Components]");
				foreach (string s in RequiredMsmqComponentsXp)
					sw.WriteLine(s + " = ON");
				foreach (string s in UndesirableMsmqComponentsXp)
					sw.WriteLine(s + " = OFF");
				sw.Flush();
			}
			ServiceLog.Debug("Installation instruction file created.");
			ServiceLog.Debug("Invoking MSMQ installation.");
			return Process.Start("sysocmgr", @"/i:sysoc.inf /x /q /w /u:%temp%\" + Path.GetFileName(path));
		}

        /// <summary>
        /// Determines whether [is installation good].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is installation good]; otherwise, <c>false</c>.
        /// </returns>
		public static bool IsInstallationGood()
		{
			var msmqSetup = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSMQ\Setup");
			if (msmqSetup == null)
				return false;
			var installedComponents = new List<string>(msmqSetup.GetValueNames());
			msmqSetup.Close();
			return HasOnlyNeededComponents(installedComponents);
		}

		private static bool IsMsmqInstalled()
		{
			return (LoadLibraryW("Mqrt.dll") != IntPtr.Zero);
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibraryW([In, MarshalAs(UnmanagedType.LPWStr)] string lpLibFileName);

		private static void PerformFunctionDependingOnOS(Func<Process> vistaFunc, Func<Process> server2008Func, Func<Process> xpAndServer2003Func)
		{
			Process process = null;
			switch (GetOperatingSystem())
			{
				case OperatingSystem.XpOrServer2003:
					process = xpAndServer2003Func();
					break;
				case OperatingSystem.Vista:
					process = vistaFunc();
					break;
				case OperatingSystem.Server2008:
					process = server2008Func();
					break;
				default:
					ServiceLog.Warning("OS not supported.");
					break;
			}
			if (process != null)
			{
				ServiceLog.Debug("Waiting for process to complete.");
				process.WaitForExit();
			}
		}

        /// <summary>
        /// Starts the MSMQ if necessary.
        /// </summary>
		public static void StartMsmqIfNecessary()
		{
			InstallMsmqIfNecessary();
			var controller = new ServiceController { ServiceName = "MSMQ", MachineName = "." };
			ProcessUtil.ChangeServiceStatus(controller, ServiceControllerStatus.Running, new Action(controller.Start));
		}
	}
}