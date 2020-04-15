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
  using SIM.SitecoreEnvironments;

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
        args.OnCompleted += () => mainWindow.Dispatcher.Invoke(() => OnPipelineCompleted(args));
        var index = MainWindowHelper.GetListItemID(instance.ID);        
        int version;
        if (int.TryParse(instance.Product.ShortVersion,out version)&& version < 90)
        {
          WizardPipelineManager.Start("delete", mainWindow, args, null, (ignore) => OnWizardCompleted(index, args.HasInstallationBeenCompleted), () => null);
        }
        else
        {
          string uninstallPath = string.Empty;
          SitecoreEnvironment env = SitecoreEnvironmentHelper.GetExistingSitecoreEnvironment(instance.Name);
          if (!string.IsNullOrEmpty(env?.UnInstallDataPath))
          {
            uninstallPath = env.UnInstallDataPath;
          }
          else
          {
            foreach (string installName in Directory.GetDirectories(ApplicationManager.UnInstallParamsFolder).OrderByDescending(s => s.Length))
            {
              if (instance.Name.StartsWith(Path.GetFileName(installName)))
              {
                uninstallPath = installName;
                break;
              }
            }
          }
          if (string.IsNullOrEmpty(uninstallPath))
          {
            WindowHelper.ShowMessage("UnInstall files not found.");
            return;
          }

          Delete9WizardArgs delete9WizardArgsargs = new Delete9WizardArgs(instance, connectionString, uninstallPath);
          WizardPipelineManager.Start("delete9", mainWindow, null, null, (ignore) => OnWizardCompleted(index, delete9WizardArgsargs.HasInstallationBeenCompleted),() => delete9WizardArgsargs);
        }
      }
    }

    #endregion

    #region Private methods

    private static void OnWizardCompleted(int index, bool hasInstallationBeenCompleted)
    {                           
      if (hasInstallationBeenCompleted)
      {
        MainWindowHelper.SoftlyRefreshInstances();
      }

      MainWindowHelper.MakeInstanceSelected(index);
    }

    private void OnPipelineCompleted(DeleteArgs args)
    {
      var root = new DirectoryInfo(args.RootPath);
      if (root.Exists && root.GetFiles("*", SearchOption.AllDirectories).Length > 0)
      {
        FileSystem.FileSystem.Local.Directory.TryDelete(args.RootPath);
      }

      args.HasInstallationBeenCompleted = true;

    }

    #endregion
  }
}