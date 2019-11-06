namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Linq;
  using System.Windows;

  using JetBrains.Annotations;

  using SIM.Adapters.SqlServer;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  public abstract class ClearDatabasesButton : IMainWindowButton
  {
    [NotNull]
    private string[] Databases { get; }

    protected ClearDatabasesButton(string databases)
    {
      Databases = databases.Split(",;|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public abstract string DatabaseName { get; }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      if (instance != null && (MainWindowHelper.IsSitecoreMember(instance) || MainWindowHelper.IsSitecore9(instance)))
      {
        return false;
      }

      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      WindowHelper.LongRunningTask(() => DoWork(instance), $"Cleaning up {DatabaseName}", mainWindow);
    }

    private void DoWork(Instance instance)
    {
      foreach (var database in instance.AttachedDatabases)
      {
        if (database == null || database.Name == "reporting")
        {
          continue;
        }

        if (Databases.Length != 0 && Databases.All(x => !x.Equals(database.Name, StringComparison.OrdinalIgnoreCase)))
        {
          continue;
        }

        SqlServerManager.Instance.Execute(database.ConnectionString, $"DELETE FROM [{DatabaseName}]");
      }
    }
  }
}