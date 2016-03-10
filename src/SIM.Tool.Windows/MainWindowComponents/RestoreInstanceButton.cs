namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines.Restore;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Tool.Base.Wizards;

  [UsedImplicitly]
  public class RestoreInstanceButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("Restore");

      if (instance != null)
      {
        var args = new RestoreArgs(instance);
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("restore", mainWindow, args, null, () => MainWindowHelper.MakeInstanceSelected(id), instance);
      }
    }

    #endregion
  }
}