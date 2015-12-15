namespace SIM.Tool.Base
{
  using System;
  using System.Data;
  using System.Data.SqlClient;
  using System.DirectoryServices.ActiveDirectory;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.ServiceProcess;
  using System.Threading;
  using SIM.Adapters.SqlServer;
  using SIM.FileSystem;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

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

    [NotNull]
    private static readonly AdvancedProperty<string> GetCurrentDomainTimeout = AdvancedSettings.Create("App/GetCurrentDomain/Timeout", "00:00:00.1");

    private static bool? isSitecoreMachine;

    #endregion

    #region Public properties

    public static bool IsSitecoreMachine
    {
      get
      {
        return isSitecoreMachine ?? (bool)(isSitecoreMachine = GetIsSitecoreMachine());
      }
    }

    #endregion

    #region Public methods

    public static bool DoNotTrack()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "donottrack.txt");

      return FileSystem.Local.File.Exists(path);
    }

    public static string GetId()
    {
      try
      {
        if (IsSitecoreMachine)
        {
          return "internal-" + Environment.MachineName + "/" + Environment.UserName;
        }
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "Failed to compute internal identifier");
      }

      string cookie = GetCookie();

      return String.Format("public-{0}", cookie);
    }

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
        Log.Warn(ex, "Failed to check SQL Server state");
        return ProfileSection.Result(true);
      }
    }

    #endregion

    #region Private methods

    [NotNull]
    private static string GetCookie()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "cookie.txt");
      if (FileSystem.Local.File.Exists(path))
      {
        var cookie = FileSystem.Local.File.ReadAllText(path);
        if (!string.IsNullOrEmpty(cookie))
        {
          return cookie;
        }
        
        try
        {
          FileSystem.Local.File.Delete(path);
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Cannot delete cookie file");
        }
      }

      var newCookie = Guid.NewGuid().ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("-", string.Empty);
      try
      {
        FileSystem.Local.File.WriteAllText(path, newCookie);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Cannot write cookie");
      }

      return newCookie;
    }

    private static bool CheckSqlServer([CanBeNull] ServiceController server)
    {
      if (server != null && server.Status == ServiceControllerStatus.Running)
      {
        return true;
      }

      WindowHelper.HandleError("The {0} instance of SQL Server is not running, it is in {1} state".FormatWith(server.ServiceName, server.Status), false);

      return false;
    }

    private static bool GetIsSitecoreMachine()
    {
      using (new ProfileSection("IsSitecoreMachine"))
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
            Log.Warn(ex, "An error occurred during retrieving current domain name");

            return false;
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Error getting error getting current domain");
          return false;
        }
      }
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

    #endregion
  }
}