using System;
using System.Collections.Generic;
using System.Text;
using Tridion.ContentDelivery.Services.Utilities;
using Tridion.ContentDelivery.Web.Jvm;

namespace TcmDeployer
{
	public class ServiceConsole
	{
		private const String CONFIG_PARAMETER = "configLocation";

		private static bool IsSpecified(String argument, String parameter)
		{
			if (String.IsNullOrEmpty(argument))
				return false;

			return argument.StartsWith("-" + parameter, StringComparison.OrdinalIgnoreCase) ||
				argument.StartsWith("--" + parameter, StringComparison.OrdinalIgnoreCase);
		}

		static void Main(String[] args)
		{
			// No arguments present or "-start"
			if (args.Length == 0 || IsSpecified(args[0], "start"))
			{
				// Instance config directory was specified
				if (args.Length == 3)
					ConfigurationHook.configFolder = args[2];

				if (Environment.UserInteractive)
				{
					if (args.Length == 3)
						Console.WriteLine("[i] Starting TcmDeployer Service, configuration \"{0}\".", args[2]);
					else
						Console.WriteLine("[i] Starting TcmDeployer Service.");

					Service.StartDeployer();
					Console.WriteLine("[!] Press any key to stop...");
					Console.ReadKey();
					Console.WriteLine("[i] Stopping...");
					Service.StopDeployer();
				}
				else
					Service.StartService();

				return;
			}

			if (IsSpecified(args[0], "install"))
			{
				if (args.Length == 1)
				{
					Console.WriteLine("[i] Installing TcmDeployer Service");
					Installation.InstallService("TcmDeployer.exe");
					return;
				}
				else if (args.Length == 3)
				{
					Console.WriteLine("[i] Installing TcmDeployer Service instance \"{0}\", configuration \"{1}\".", args[1], args[2]);
					Installation.instanceID = args[1];
					Installation.serviceArguments = " -" + CONFIG_PARAMETER + "=" + args[2];
					Installation.InstallService("TcmDeployer.exe");
					return;
				}
			}

			if (IsSpecified(args[0], "remove"))
			{
				if (args.Length == 2)
				{
					Console.WriteLine("[i] Uninstalling TcmDeployer Service instance \"{0}\".", args[1]);
					Installation.instanceID = args[1];
				}
				else
					Console.WriteLine("[i] Uninstalling TcmDeployer Service");

				Installation.UninstallService("TcmDeployer.exe");
				return;
			}

			// Handle initialization from TcmDeployer as a service with a specified configuration directory
			if (IsSpecified(args[0], CONFIG_PARAMETER))
			{
				String[] values = args[0].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

				if (values.Length == 2)
				{
					ConfigurationHook.configFolder = values[1];
					Service.StartService();
					return;
				}
			}

			Console.WriteLine("Usage: TcmDeployer (-start|-install [instance-id instance-config-directory]|-remove [instance-id])");			
		}
	}
}
