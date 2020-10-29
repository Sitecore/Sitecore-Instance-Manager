using System.Windows;
using SIM.Instances;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class OpenSimFolderButton : OpenFolderButton
  {
    public OpenSimFolderButton(string folder) : base(folder)
    {
    }

    #region Public methods

    public override bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public override bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    #endregion
  }
}
