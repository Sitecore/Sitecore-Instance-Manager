#region usings

using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using Microsoft.Web.Administration;
using SIM.Adapters.WebServer;

#endregion

namespace SIM.Pipelines.Export
{
	class ExportSettings : ExportProcessor
	{
		protected override void Process(ExportArgs args)
		{
			var websiteName = args.Instance.Name;
			var appPoolName = new Website(args.Instance.ID).GetPool(new WebServerManager.WebServerContext(string.Empty)).Name;

			var websiteSettingsCommand = string.Format(@"%windir%\system32\inetsrv\appcmd list site {0}{1}{0} /config /xml > {2}", '"', websiteName, Path.Combine(args.Folder, "WebsiteSettings.xml"));
			var appPoolSettingsCommand = string.Format(@"%windir%\system32\inetsrv\appcmd list apppool {0}{1}{0} /config /xml > {2}", '"', appPoolName, Path.Combine(args.Folder, "AppPoolSettings.xml"));

			EcexuteCommand(websiteSettingsCommand);
			EcexuteCommand(appPoolSettingsCommand);
		}

		private static void EcexuteCommand(string command)
		{
			var procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
			{
				UseShellExecute = false,
				CreateNoWindow = true
			};

			var proc = new Process { StartInfo = procStartInfo };
			proc.Start();
			proc.WaitForExit();
		}
	}
}
