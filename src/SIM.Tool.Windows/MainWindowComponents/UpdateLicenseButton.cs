namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class UpdateLicenseButton : WindowOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      LicenseUpdater.Update(mainWindow, instance);
    }

    #endregion

    #region Protected methods

    protected override void OnClick(Window mainWindow)
    {
    }

    #endregion
  }
}