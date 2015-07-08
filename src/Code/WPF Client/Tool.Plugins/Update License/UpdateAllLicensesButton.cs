using System.Windows;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Plugins.UpdateLicense
{
  public class UpdateAllLicensesButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      LicenseUpdater.Update(mainWindow, instance);
    }
  }
}
