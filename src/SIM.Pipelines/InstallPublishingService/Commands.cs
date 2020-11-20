using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public static class Commands
  {
    private const string SET_CONNECTION_STRING = ".\\Sitecore.Framework.Publishing.Host.exe configuration setconnectionstring {0} \"{1}\"";
    private const string SCHEMA_UPGRADE = ".\\Sitecore.Framework.Publishing.Host.exe schema upgrade --force";
    private const string SCHEMA_RESET = ".\\Sitecore.Framework.Publishing.Host.exe schema reset --force";
    private const string IIS_INSTALL = ".\\Sitecore.Framework.Publishing.Host.exe iis install --sitename \"{0}\" --apppool \"{1}\" --hosts --force";

    public static void SetConnectionString(string connectionStringName, string connectrionStringValue)
    {
      ExecuteCommand(SET_CONNECTION_STRING, connectionStringName, connectrionStringValue);
    }

    public static void SchemaUpgrade()
    {
      ExecuteCommand(SCHEMA_UPGRADE);
    }

    public static void SchemaReset()
    {
      ExecuteCommand(SCHEMA_RESET);
    }

    public static void IISInstall(string siteName, string appPoolName)
    {
      ExecuteCommand(IIS_INSTALL, siteName, appPoolName);
    }

    private static void ExecuteCommand(string commandFormat, params object[] args)
    {
      using (PowerShell PS = PowerShell.Create())
      {
        PS.AddScript(string.Format(commandFormat, args)).Invoke();
      }
    }
  }
}
