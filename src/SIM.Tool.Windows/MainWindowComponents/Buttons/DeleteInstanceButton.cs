using System.IO;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Pipelines.Delete;
using SIM.SitecoreEnvironments;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class DeleteInstanceButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
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