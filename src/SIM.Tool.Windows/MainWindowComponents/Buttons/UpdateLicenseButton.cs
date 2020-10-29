using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
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