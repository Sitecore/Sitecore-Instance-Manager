namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Linq;
  using System.Windows;
  using Adapters.SqlServer;
  using Base;
  using Base.Plugins;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ClearEventQueueButton : IMainWindowButton
  {
    [NotNull]
    private readonly string[] Databases;

    public ClearEventQueueButton()
    {
      this.Databases = new string[0];
    }

    public ClearEventQueueButton(string databases)
    {
      this.Databases = databases.Split(",;|".ToCharArray());
    }

    public bool IsEnabled([NotNull] Window mainWindow, [CanBeNull] Instance instance)
    {
      return instance != null;
    }

    public void OnClick([NotNull] Window mainWindow, [CanBeNull] Instance instance)
    {
      WindowHelper.LongRunningTask(() => DoWork(instance), "Cleaning up EventQueue", mainWindow);
    }

    private void DoWork(Instance instance)
    {
      foreach (var database in instance.AttachedDatabases)
      {
        if (database == null || database.Name == "reporting")
        {
          continue;
        }

        if (this.Databases.Length != 0 && this.Databases.All(x => !x.Equals(database.Name, StringComparison.OrdinalIgnoreCase)))
        {
          continue;
        }

        SqlServerManager.Instance.Execute(database.ConnectionString, "DELETE FROM [EventQueue]");
      }
    }
  }
}
