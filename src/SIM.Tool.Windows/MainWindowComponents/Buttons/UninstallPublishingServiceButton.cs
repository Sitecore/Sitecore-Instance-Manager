namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;

  [UsedImplicitly]
  public class UninstallSPSButton : IMainWindowButton
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
        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("uninstallpublishingservice", mainWindow, null, null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new UninstallSPSWizardArgs(instance));
      }
    }

    #endregion
  }
}