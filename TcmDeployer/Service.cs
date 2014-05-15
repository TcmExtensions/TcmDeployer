using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TcmDeployer.Misc;
using Tridion.ContentDelivery.Web.Jvm;
using Tridion.ContentDelivery.Web.Utilities;

namespace TcmDeployer
{
	public class Service : ServiceBase
	{
		private static Thread mDeployerThread;

		/// <summary>
		/// Start the Content Deployer in a seperate thread
		/// </summary>
		public static void StartDeployer()
		{
			// Start the Tridion deployer in a seperate thread
			mDeployerThread = new Thread(JavaDeployer);
			mDeployerThread.Start();
		}

		public static void StopDeployer()
		{
			// There seems to be no documented way to stop the deployer once started
			// Existing cd_deployer service just waits for Windows to kill the process
			//Environment.Exit(0);
		}

		/// <summary>
		/// Starts the TcmDeployer service
		/// </summary>
		public static void StartService()
		{
			ServiceBase.Run(new ServiceBase[] 
			{ 
				new Service() 
			});
		}

		/// <summary>
		/// Execute the "com.tridion.deployer.Deployer Java class
		/// </summary>
		/// <remarks>
		/// Tridion will look for the TRIDION_HOME directory in the following order:
		/// 1) Current HttpContext (if HttpContext is present) ~/bin directory
		/// 2) Current Directory
		/// 3) TRIDION_HOME value from registry key Software\Tridion\Content Delivery\General or Software\Wow6432Node\Tridion\Content Delivery\General
		/// 4) Environment variable TRIDION_HOME
		/// 5) "C:\Program Files\Tridion"
		/// 
		/// A valid TRIDION_HOME directory is determined by the presence of:
		/// 
		/// .\lib\cd_core.jar
		/// .\lib\cd_model.jar
		/// .\config\cd_broker_conf.xml OR .\config\cd_storage.conf.xml
		/// </remarks>
		private static void JavaDeployer()
		{
			// Ensure the current directory is set correctly for Tridion to evaluate the cd_deployer executable directory
			// as a valid TRIDION_HOME
			Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

			// Start the Java Tridion deployer
			ServiceHandler.StartService("com.tridion.deployer.Deployer");
		}

		/// <summary>
		/// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
		/// </summary>
		/// <param name="args">Data passed by the start command.</param>
		protected override void OnStart(string[] args)
		{
			try
			{
				EventLogger.GetEventLogger().WriteEntry("Starting TcmDeployer: " + AppDomain.CurrentDomain.BaseDirectory);
				StartDeployer();
			}
			catch (Exception ex)
			{
				EventLogger.GetEventLogger().WriteEntry("Unable to start TcmDeployer:\n" + Logging.TraceException(ex), EventLogEntryType.Error);
				throw;
			}
		}

		/// <summary>
		/// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
		/// </summary>
		protected override void OnStop()
		{
			StopDeployer();
		}
	}
}
