using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using SIM.Core;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.Dialogs;
using SIM.Tool.Windows.UserControls.SitecoreAuthentication;

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
          string sitecoreIdServerUri = "https://" + instance.SitecoreEnvironment.Name + "Id.local";
          string idServerAuthToken = this.GetAuthToken(sitecoreIdServerUri, 
            CoreAppSettings.AppLoginAsAdminUserName.Value, CoreAppSettings.AppLoginAsAdminNewPassword.Value);

          if (string.IsNullOrEmpty(idServerAuthToken))
          {
            return;
          }

          headers = new Dictionary<string, string> { { "Authorization", idServerAuthToken } };
        }
        else if (sitecoreVersion == 90)
        {
          authCookie = this.GetAuthCookie(instance.GetUrl(), 
            CoreAppSettings.AppLoginAsAdminUserName.Value, CoreAppSettings.AppLoginAsAdminNewPassword.Value);

          if (string.IsNullOrEmpty(authCookie))
          {
            return;
          }
        }

        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("installmodules", mainWindow, null, null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new InstallModulesWizardArgs(instance, authCookie, headers));
      }
    }

    private string GetAuthToken(string sitecoreIdServerUri, string userName, string password)
    {
      string idServerAuthToken = null;
      var task = Task.Run(async () => {
        idServerAuthToken = await
          SitecoreIdServerAuth.GetToken(sitecoreIdServerUri, userName, password);
      });
      task?.Wait();

      if (string.IsNullOrEmpty(idServerAuthToken))
      {
        if (WindowHelper.ShowMessage(
              "Unable to get authentication token using the following data:\n\nIdentity Server URI: " + sitecoreIdServerUri +
              "\nUser name: " + userName + "\nPassword: " + password + "\n\nWould you like to try to continue installation using other URI and credentials?", 
              MessageBoxButton.YesNo,
              MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
          CredentialsContext credentialsContext = 
            WindowHelper.ShowDialog<CredentialsDialog>(new CredentialsContext(userName, password, sitecoreIdServerUri), null) as CredentialsContext;
          if (credentialsContext != null)
          {
            this.GetAuthToken(credentialsContext.Uri, credentialsContext.UserName, credentialsContext.Password);
          }
        }
      }

      return idServerAuthToken;
    }

    private string GetAuthCookie(string instanceUri, string userName, string password)
    {
      string authCookie = null;
      var task = Task.Run(async () => {
        authCookie = await
          SitecoreServicesClientAuth.GetCookie(instanceUri, userName, CoreAppSettings.AppLoginAsAdminNewPassword.Value);
      });
      task?.Wait();

      if (string.IsNullOrEmpty(authCookie))
      {
        if (WindowHelper.ShowMessage(
              "Unable to get authentication cookie using the following data:\n\n" +
              "User name: " + userName + "\nPassword: " + password + "\n\nWould you like to try to continue installation using other credentials?", 
              MessageBoxButton.YesNo,
              MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
          CredentialsContext credentialsContext = 
            WindowHelper.ShowDialog<CredentialsDialog>(new CredentialsContext(userName, password), null) as CredentialsContext;
          if (credentialsContext != null)
          {
            this.GetAuthCookie(instanceUri, credentialsContext.UserName, credentialsContext.Password);
          }
        }
      }

      return authCookie;
    }
  }
}