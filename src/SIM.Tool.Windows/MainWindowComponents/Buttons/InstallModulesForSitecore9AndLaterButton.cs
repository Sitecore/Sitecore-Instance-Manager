using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using SIM.Core;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.Helpers;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class InstallModulesForSitecore9AndLaterButton : InstanceOnlyButton
  {
    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        Dictionary<string, string> headers = null;
        string authCookie = null;

        int.TryParse(instance.Product.ShortVersion, out int sitecoreVersion);
        if (sitecoreVersion >= 91)
        {
          string idServerAuthToken = null;
          string sitecoreIdServerUri = "https://" + instance.SitecoreEnvironment.Name + "Id.local";
          var task = Task.Run(async () => {
            idServerAuthToken = await
              SitecoreIdServerAuth.GetToken(sitecoreIdServerUri, CoreAppSettings.AppLoginAsAdminUserName.Value, CoreAppSettings.AppLoginAsAdminNewPassword.Value);
          });
          task?.Wait();

          if (string.IsNullOrEmpty(idServerAuthToken))
          {
            WindowHelper.ShowMessage("Unable to get authentication token from the following Sitecore Identity Server: " + sitecoreIdServerUri, MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
          }

          headers = new Dictionary<string, string> { { "Authorization", idServerAuthToken } };
        }
        else if (sitecoreVersion == 90)
        {
          string instanceUri = instance.GetUrl();
          var task = Task.Run(async () => {
            authCookie = await
              SitecoreServicesClientAuth.GetCookie(instanceUri, CoreAppSettings.AppLoginAsAdminUserName.Value, CoreAppSettings.AppLoginAsAdminNewPassword.Value);
          });
          task?.Wait();

          if (string.IsNullOrEmpty(authCookie))
          {
            WindowHelper.ShowMessage("Unable to get authentication cookie from the following Sitecore instance: " + instanceUri, MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
          }
        }

        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("installmodules", mainWindow, null, null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new InstallModulesWizardArgs(instance, authCookie, headers));
      }
    }
  }
}