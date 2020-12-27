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
    private SitecoreRole sitecoreRole = SitecoreRole.Unknown;

    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null && this.sitecoreRole != SitecoreRole.Unknown)
      {
        string filePath = Path.Combine(instance.WebRootPath, ".env");
        EnvModel model = EnvModel.LoadFromFile(filePath);
        switch (this.sitecoreRole)
        {
          case SitecoreRole.Cm:
          {
            if (!string.IsNullOrEmpty(model.CmHost))
            {
              CoreApp.OpenInBrowser($"https://{model.CmHost}", true, null, new string[0]);
            }
            break;
          }
          case SitecoreRole.Cd:
          {
            if (!string.IsNullOrEmpty(model.CdHost))
            {
              CoreApp.OpenInBrowser($"https://{model.CdHost}", true, null, new string[0]);
            }
            break;
          }
          case SitecoreRole.Id:
          {
            if (!string.IsNullOrEmpty(model.IdHost))
            {
              CoreApp.OpenInBrowser($"https://{model.IdHost}", true, null, new string[0]);
            }
            break;
          }
        }
      }
    }

    public override bool IsVisible(Window mainWindow, Instance instance)
    {
      if (base.IsVisible(mainWindow, instance))
      {
        if (instance.Name.EndsWith("-cm"))
        {
          this.sitecoreRole = SitecoreRole.Cm;
          return true;
        }
        if (instance.Name.EndsWith("-cd"))
        {
          this.sitecoreRole = SitecoreRole.Cd;
          return true;
        }
        if (instance.Name.EndsWith("-id"))
        {
          this.sitecoreRole = SitecoreRole.Id;
          return true;
        }
      }

      return false;
    }

    private enum SitecoreRole
    {
      Cm,
      Cd,
      Id,
      Unknown
    }
  }
}