namespace SIM.Tool.Base
{
  using System;
  using System.Data;
  using System.Data.SqlClient;
  using System.Linq;
  using System.ServiceProcess;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Adapters.SqlServer;
  using SIM.Tool.Base.Profiles;

  public class EnvironmentHelper
  {
    #region Fields

    public static readonly string[] ConfigurationPackageFolders = new[]
    {
      ApplicationManager.ConfigurationPackagesFolder, ApplicationManager.DefaultConfigurations
    };

    public static readonly string[] FilePackageFolders = new[]
    {
      ApplicationManager.FilePackagesFolder, ApplicationManager.PluginsFolder, ApplicationManager.DefaultPackages, ApplicationManager.StockPlugins
    };

    [NotNull]
    private static readonly AdvancedProperty<bool> CheckSqlServerState = AdvancedSettings.Create("App/SqlServer/CheckSqlState", false);

    #endregion

    #region Public properties

    #endregion

    #region Public methods

    public static bool CheckSqlServer()
    {
      if (!CheckSqlServerState.Value)
      {
        return true;
      }

      try
      {
        using (new ProfileSection("Check SQL Server"))
        {
          var profile = ProfileManager.Profile;
          Assert.IsNotNull(profile, "Profile is unavailable");

          var ds = new SqlConnectionStringBuilder(profile.ConnectionString).DataSource;
          var arr = ds.Split('\\');
          var name = arr.Length == 2 ? arr[1] : String.Empty;

          var serviceName = GetSqlServerServiceName(profile.ConnectionString);
          if (String.IsNullOrEmpty(serviceName))
          {
            WindowHelper.HandleError("The {0} instance of SQL Server cannot be reached".FormatWith(ds), false);
            return ProfileSection.Result(false);
          }

          ServiceController[] serviceControllers = ServiceController.GetServices();
          ServiceController server = (!String.IsNullOrEmpty(name) ? serviceControllers.FirstOrDefault(s => s.ServiceName.EqualsIgnoreCase(name)) : null) ??
                                     serviceControllers.FirstOrDefault(
                                       s => s.ServiceName.EqualsIgnoreCase(serviceName)) ??
                                     serviceControllers.FirstOrDefault(s => s.ServiceName.EqualsIgnoreCase(serviceName));
          Assert.IsNotNull(server, "Cannot find the " + (name.EmptyToNull() ?? "default") + " sql server instance");
          var result = CheckSqlServer(server);

          return ProfileSection.Result(result);
        }
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "Failed to check SQL Server state");
        return ProfileSection.Result(true);
      }
    }

    #endregion

    #region Private methods

    private static bool CheckSqlServer([CanBeNull] ServiceController server)
    {
      if (server != null && server.Status == ServiceControllerStatus.Running)
      {
        return true;
      }

      WindowHelper.HandleError("The {0} instance of SQL Server is not running, it is in {1} state".FormatWith(server.ServiceName, server.Status), false);

      return false;
    }

    private static string GetSqlServerServiceName(string connectionString)
    {
      try
      {
        using (var connection = SqlServerManager.Instance.OpenConnection(new SqlConnectionStringBuilder(connectionString)))
        {
          var command = new SqlCommand
          {
            Connection = connection, 
            CommandText = @"SELECT @@SERVICENAME"
          };

          var adapter = new SqlDataAdapter
          {
            SelectCommand = command
          };

          var dataTable = new DataTable();
          adapter.Fill(dataTable);

          return dataTable.Rows[0][0] as string;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "GetSqlServerServiceName");
        return null;
      }
    }

    #endregion
  }
}