using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class OpenBinFolderButton : OpenFolderButton
  {
    public OpenBinFolderButton(string folder) : base(folder)
    {
    }

    public override void OnClick(Window mainWindow, Instance instance)
    {
      var path = ExpandPath(instance).Replace("/", "\\");

      if (!FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        if (FileSystem.FileSystem.Local.Directory.Exists(instance.WebRootPath))
        {
          WindowHelper.ShowMessage($"The following folder does not exist:\n\n'{path}'\n\nThe web root folder will be opened instead.", MessageBoxButton.OK, MessageBoxImage.Warning);
          MainWindowHelper.OpenFolder(instance.WebRootPath);
        }
        else
        {
          WindowHelper.ShowMessage($"The following folders do not exist:\n\n{path}\n\n{instance.WebRootPath}", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
      }
      else
      {
        MainWindowHelper.OpenFolder(path);
      }
    }
  }
}