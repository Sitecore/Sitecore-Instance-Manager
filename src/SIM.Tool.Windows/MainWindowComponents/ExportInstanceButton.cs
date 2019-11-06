namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines.Export;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.Export;

  [UsedImplicitly]
  public class ExportInstanceButton : IMainWindowButton
  {
    #region Public methods

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
      Analytics.TrackEvent("Export");

      if (instance != null)
      {
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("export", mainWindow, new ExportArgs(instance, false, true, true, true, false, false, false, false, false), null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new ExportWizardArgs(instance, string.Empty));
      }
    }

    #endregion
  }
}