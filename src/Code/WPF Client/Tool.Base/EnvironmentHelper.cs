using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceProcess;
using SIM.Adapters.SqlServer;
using SIM.Base;
using SIM.Tool.Base.Profiles;

namespace SIM.Tool.Base
{
  using System.DirectoryServices.ActiveDirectory;
  using System.Reflection;
  using System.Threading;

  public class EnvironmentHelper
  {
    public static readonly string[] FilePackageFolders = new[] { ApplicationManager.FilePackagesFolder, ApplicationManager.PluginsFolder, ApplicationManager.DefaultPackages, ApplicationManager.StockPlugins };

    public static readonly string[] ConfigurationPackageFolders = new[] { ApplicationManager.ConfigurationPackagesFolder, ApplicationManager.DefaultConfigurations };

    [NotNull]
    private static readonly AdvancedProperty<bool> CheckSqlServerState = AdvancedSettings.Create("App/SqlServer/CheckSqlState", false);

    [NotNull]
    private static readonly AdvancedProperty<string> GetCurrentDomainTimeout = AdvancedSettings.Create("App/GetCurrentDomain/Timeout", "00:00:00.1");

    private static bool? isSitecoreMachine;

    public static bool IsSitecoreMachine
    {
      get
      {
        return isSitecoreMachine ?? (bool)(isSitecoreMachine = GetIsSitecoreMachine());
      }
    }

    private static bool GetIsSitecoreMachine()
    {
      using (new ProfileSection("IsSitecoreMachine", typeof(EnvironmentHelper)))
      {
        try
        {
          TimeSpan timeout;
          if (!TimeSpan.TryParse(GetCurrentDomainTimeout.Value, out timeout))
          {
            timeout = TimeSpan.Parse(GetCurrentDomainTimeout.DefaultValue);
          }

          try
          {
            var name = Run(timeout, () => Domain.GetCurrentDomain().Name);

            return "dk.sitecore.net".EqualsIgnoreCase(name);
          }
          catch (Exception ex)
          {
            Log.Warn("An error occurred during retrieving current domain name", typeof(EnvironmentHelper), ex);

            return false;
          }
        }
        catch (Exception ex)
        {
          Log.Error("Error getting error getting current domain", typeof(EnvironmentHelper), ex);
          return false;
        }
      }
    }

    private static T Run<T>(TimeSpan timeout, [NotNull] Func<T> operation) where T : class
    {
      Assert.ArgumentNotNull(operation, "operation");
      Exception error = null;
      T result = null;

      var mre = new ManualResetEvent(false);
      ThreadPool.QueueUserWorkItem(
        delegate
        {
          try
          {
            result = operation();
          }
          catch (Exception e)
          {
            error = e;
          }
          finally
          {
            mre.Set();
          }
        });

      if (!mre.WaitOne(timeout, true))
      {
        return null;
      }

      if (error != null)
      {
        throw new TargetInvocationException(error);
      }

      return result;
    }

    public static bool CheckSqlServer()
    {
      if (!EnvironmentHelper.CheckSqlServerState.Value)
      {
        return true;
      }

      try
      {
        using (new ProfileSection("Check SQL Server", typeof(EnvironmentHelper)))
        {
          Profile profile = ProfileManager.Profile;
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
        Log.Warn("Failed to check SQL Server state", typeof(EnvironmentHelper), ex);
        return ProfileSection.Result(true);
      }
    }
    private static bool CheckSqlServer(ServiceController server)
    {
      if (server.Status != ServiceControllerStatus.Running)
      {
        WindowHelper.HandleError("The {0} instance of SQL Server is not running, it is in {1} state".FormatWith(server.ServiceName, server.Status), false);
        return false;
      }
      return true;
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
        Log.Error("GetSqlServerServiceName", typeof(EnvironmentHelper), ex);
        return null;
      }

    }
  }
}
