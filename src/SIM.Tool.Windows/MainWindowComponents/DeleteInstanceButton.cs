namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.IO;
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines.Delete;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base;
  using System.Linq;

  [UsedImplicitly]
  public class DeleteInstanceButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var connectionString = ProfileManager.GetConnectionString();
        var args = new DeleteArgs(instance, connectionString);
        args.OnCompleted += () => mainWindow.Dispatcher.Invoke(() => OnPipelineCompleted(args.RootPath));
        var index = MainWindowHelper.GetListItemID(instance.ID);
        if (int.Parse(instance.Product.ShortVersion) < 90)
        {
          WizardPipelineManager.Start("delete", mainWindow, args, null, (ignore) => OnWizardCompleted(index), () => null);
        }
        else
        {
          string uninstallPath = string.Empty;
          foreach(string installName in Directory.GetDirectories(ApplicationManager.UnInstallParamsFolder).OrderByDescending(s=>s.Length))
          {
            if (instance.Name.StartsWith(Path.GetFileName(installName)))
            {
              uninstallPath = installName;
              break;
            }
          }
          if (string.IsNullOrEmpty(uninstallPath))
          {
            WindowHelper.ShowMessage("UnInstall files not found.");
            return;
          }
          
          WizardPipelineManager.Start("delete9", mainWindow, null, null, (ignore) => OnWizardCompleted(index),
          () => new Delete9WizardArgs(instance,connectionString,uninstallPath)
         );
        }
      }
    }

    #endregion

    #region Private methods

    private static void OnWizardCompleted(int index)
    {
      MainWindowHelper.SoftlyRefreshInstances();
      MainWindowHelper.MakeInstanceSelected(index);
    }

    private void OnPipelineCompleted(string rootPath)
    {
      var root = new DirectoryInfo(rootPath);
      if (root.Exists && root.GetFiles("*", SearchOption.AllDirectories).Length > 0)
      {
        FileSystem.FileSystem.Local.Directory.TryDelete(rootPath);
      }
    }

    #endregion
  }
}