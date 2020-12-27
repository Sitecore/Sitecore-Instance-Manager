using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class OpenContainerFolderButton : InstanceOnlyButton
  {
    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        if (!FileSystem.FileSystem.Local.Directory.Exists(instance.WebRootPath))
        {
          var answer = WindowHelper.ShowMessage("The folder does not exist. Would you create it?\n\n" + instance.WebRootPath, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
          if (answer != MessageBoxResult.OK)
          {
            return;
          }

          FileSystem.FileSystem.Local.Directory.CreateDirectory(instance.WebRootPath);
        }

        MainWindowHelper.OpenFolder(instance.WebRootPath);
      }
    }
  }
}