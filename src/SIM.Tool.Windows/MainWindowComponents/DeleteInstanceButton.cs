namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.IO;
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines.Delete;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Tool.Base.Wizards;

  [UsedImplicitly]
  public class DeleteInstanceButton : IMainWindowButton
  {
    #region Public methods

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
        args.OnCompleted += () => mainWindow.Dispatcher.Invoke(new Action(() => this.OnPipelineCompleted(args.RootPath)));
        var index = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("delete", mainWindow, args, null, () => OnWizardCompleted(index));
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