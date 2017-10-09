namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Linq;
  using System.Windows;
  using Adapters.SqlServer;
  using Base;
  using Base.Plugins;
  using SIM.Instances;
  using JetBrains.Annotations;

  public class ClearEventQueueButton : IMainWindowButton
  {
    [NotNull]
    private readonly string[] _Databases;

    public ClearEventQueueButton()
    {
      _Databases = new string[0];
    }

    public ClearEventQueueButton(string databases)
    {
      _Databases = databases.Split(",;|".ToCharArray());
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

        if (_Databases.Length != 0 && _Databases.All(x => !x.Equals(database.Name, StringComparison.OrdinalIgnoreCase)))
        {
          continue;
        }

        SqlServerManager.Instance.Execute(database.ConnectionString, "DELETE FROM [EventQueue]");
      }
    }
  }
}
