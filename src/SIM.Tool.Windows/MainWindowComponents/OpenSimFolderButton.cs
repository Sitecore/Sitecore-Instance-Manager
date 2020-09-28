using System.Windows;
using SIM.Instances;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class OpenSimFolderButton : OpenFolderButton
  {
    public OpenSimFolderButton(string folder) : base(folder)
    {
    }

    public override bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public override bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }
  }
}
