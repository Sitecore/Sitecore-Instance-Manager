using System;
using System.IO;
using System.Windows;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Delete;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Wizards;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class DeleteInstanceButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        var connectionString = ProfileManager.GetConnectionString();
        var args = new DeleteArgs(instance, connectionString);
        args.OnCompleted += () => mainWindow.Dispatcher.Invoke(new Action(() => OnPipelineCompleted(args.RootPath)));
        var index = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("delete", mainWindow, args, null, () => OnWizardCompleted(index));
      }
    }

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
        FileSystem.Local.Directory.TryDelete(rootPath);
      }      
    }
  }
}
