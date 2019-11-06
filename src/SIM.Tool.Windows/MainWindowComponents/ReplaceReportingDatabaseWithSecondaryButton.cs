namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Data.SqlClient;
  using System.Linq;
  using System.Windows;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Adapters.SqlServer;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  public class ReplaceReportingDatabaseWithSecondaryButton : IMainWindowButton
  {
    [UsedImplicitly]
    public ReplaceReportingDatabaseWithSecondaryButton()
    {
    }

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
      WindowHelper.LongRunningTask(() => Process(instance), "Replacing reporting database", mainWindow);
    }

    private static void Process(Instance instance)
    {
      var connectionStrings = instance.Configuration.ConnectionStrings;

      var primary = connectionStrings.FirstOrDefault(x => x.Name == "reporting");
      Assert.IsNotNull(primary, nameof(primary));

      var secondary = connectionStrings.FirstOrDefault(x => x.Name == "reporting.secondary");
      Assert.IsNotNull(secondary, nameof(secondary));

      var value = secondary.Value;

      secondary.Delete();

      var cstr = new SqlConnectionStringBuilder(primary.Value);
      SqlServerManager.Instance.DeleteDatabase(cstr.InitialCatalog, cstr);

      primary.Value = value;

      primary.SaveChanges();
    }
  }
}