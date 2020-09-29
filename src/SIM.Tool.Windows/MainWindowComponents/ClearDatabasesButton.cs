namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Linq;
  using System.Windows;
  using JetBrains.Annotations;
  using SIM.Adapters.SqlServer;
  using SIM.Instances;
  using SIM.Tool.Base;

  public abstract class ClearDatabasesButton : InstanceOnlyButton
  {
    [NotNull]
    private string[] Databases { get; }

    protected ClearDatabasesButton(string databases)
    {
      Databases = databases.Split(",;|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public abstract string DatabaseName { get; }

    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
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

    #endregion
  }
}