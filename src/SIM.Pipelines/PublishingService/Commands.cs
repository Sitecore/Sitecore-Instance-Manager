using System.Management.Automation;

namespace SIM.Pipelines.PublishingService
{
  public static class Commands
  {
    private const string SET_CONNECTION_STRING = "configuration setconnectionstring {0} \"{1}\"";
    private const string SCHEMA_UPGRADE = "schema upgrade --force";
    private const string SCHEMA_RESET = "schema reset --force";
    private const string IIS_INSTALL = "iis install --sitename \"{0}\" --apppool \"{1}\" --port \"{2}\" --hosts --force";

    public static string CommandRoot { get; set; }

    public static void SetConnectionString(string connectionStringName, string connectionStringValue)
    {
      ExecuteCommand(SET_CONNECTION_STRING, connectionStringName, connectionStringValue);
    }

    public static void SchemaUpgrade()
    {
      ExecuteCommand(SCHEMA_UPGRADE);
    }

    public static void SchemaReset()
    {
      ExecuteCommand(SCHEMA_RESET);
    }

    public static void IISInstall(string siteName, string appPoolName, int port)
    {
      ExecuteCommand(IIS_INSTALL, siteName, appPoolName, port);
    }

    private static void ExecuteCommand(string commandArgs, params object[] args)
    {
      using (PowerShell PS = PowerShell.Create())
      {
        PS.AddScript($"Set-Location {CommandRoot}").Invoke();
        PS.AddScript(string.Format(".\\Sitecore.Framework.Publishing.Host.exe " + commandArgs, args)).Invoke();
      }
    }
  }
}
