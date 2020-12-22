using System.IO;
using System.Windows;
using JetBrains.Annotations;
using SIM.ContainerInstaller;
using SIM.Core;
using SIM.Instances;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class BrowseSitecoreContainerWebsiteButton : InstanceOnlyButton
  {
    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        string filePath = Path.Combine(instance.WebRootPath, ".env");
        EnvModel model = EnvModel.LoadFromFile(filePath);
        if (!string.IsNullOrEmpty(model.CmHost))
        {
          CoreApp.OpenInBrowser($"https://{model.CmHost}", true, null, new string[0]);
        }
      }
    }
  }
}