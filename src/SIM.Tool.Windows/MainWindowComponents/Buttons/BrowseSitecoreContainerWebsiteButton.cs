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
        SitecoreRole sitecoreRole = GetInstanceRole(instance);
        if (sitecoreRole != SitecoreRole.Unknown)
        {
          string filePath = Path.Combine(instance.WebRootPath, ".env");
          EnvModel model = EnvModel.LoadFromFile(filePath);
          switch (sitecoreRole)
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
    }

    public override bool IsVisible(Window mainWindow, Instance instance)
    {
      if (base.IsVisible(mainWindow, instance))
      {
        if (this.GetInstanceRole(instance) != SitecoreRole.Unknown)
        {
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

    private SitecoreRole GetInstanceRole(Instance instance)
    {
      if (instance.Name.EndsWith("-cm"))
      {
        return SitecoreRole.Cm;
      }
      
      if (instance.Name.EndsWith("-cd"))
      {
        return SitecoreRole.Cd;
      }

      if (instance.Name.EndsWith("-id"))
      {
        return SitecoreRole.Id;
      }
      
      return SitecoreRole.Unknown;
    }
  }
}