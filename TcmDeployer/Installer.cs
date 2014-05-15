using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;
using Tridion.ContentDelivery.Services.Utilities;

namespace TcmDeployer
{
	[RunInstaller(true)]
	public class Installer : System.Configuration.Install.Installer
	{
		private static String ServiceName
		{
			get
			{
				return String.IsNullOrEmpty(Installation.instanceID) ? "TcmDeployer" : "TcmDeployer-" + Installation.instanceID;
			}
		}

		public Installer()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller()
			{
				Account = ServiceAccount.LocalSystem,
				Username = null,
				Password = null
			};

			ServiceInstaller serviceInstaller = new ServiceInstaller()
			{
				ServiceName = ServiceName,
				DisplayName = String.IsNullOrEmpty(Installation.instanceID) ? "TcmDeployer" : "TcmDeployer (" + Installation.instanceID + ")",
				Description = "Receives content from the Tridion Content Distributor Transport Service and stores it in the Content Broker.",
				StartType = ServiceStartMode.Automatic				
			};

			serviceInstaller.AfterInstall += serviceInstaller_AfterInstall;

			Installers.AddRange(new System.Configuration.Install.Installer[] { serviceProcessInstaller, serviceInstaller });
        }

		private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
		{
			using (RegistryKey serviceKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\" + ServiceName, true))
			{
				if (serviceKey != null)
				{
					serviceKey.SetValue("DelayedAutostart", 1, RegistryValueKind.DWord);
				
					if (Installation.serviceArguments != null)
						serviceKey.SetValue("ImagePath", serviceKey.GetValue("ImagePath") as String + Installation.serviceArguments);					
				}
			}
		}
	}
}
