using SIM.Core;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Windows.Dialogs;
using SIM.Tool.Windows.UserControls.SitecoreAuthentication;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SIM.Tool.Windows.MainWindowComponents.Helpers
{
  public class ButtonAuthenticationHelper
  {
    public Dictionary<string, string> GetIdServerAuthToken(Window mainWindow, Instance instance)
    {
      string sitecoreIdServerUri = "https://" + instance.SitecoreEnvironment.Name + "Id.local";
      string idServerAuthToken = GetAuthToken(sitecoreIdServerUri, CoreAppSettings.AppLoginAsAdminUserName.Value, CoreAppSettings.AppLoginAsAdminNewPassword.Value, mainWindow);
      idServerAuthToken = ValidateAuthToken(idServerAuthToken, sitecoreIdServerUri,
        CoreAppSettings.AppLoginAsAdminUserName.Value, CoreAppSettings.AppLoginAsAdminNewPassword.Value, mainWindow);
      
      if (string.IsNullOrEmpty(idServerAuthToken))
      {
        return null;
      }
      return new Dictionary<string, string> { { "Authorization", idServerAuthToken } }; ;         
    }

    public string GetAuthCookie(Window mainWindow, Instance instance)
    {
      string instanceUri = instance.GetUrl();

      if (!IsInstanceUriValid(instanceUri))
      {
        return null;
      }

      string authCookie = GetAuthClientCookie(instanceUri, CoreAppSettings.AppLoginAsAdminUserName.Value, CoreAppSettings.AppLoginAsAdminNewPassword.Value, mainWindow);
      authCookie = ValidateAuthCookie(authCookie, instanceUri,
      CoreAppSettings.AppLoginAsAdminUserName.Value, CoreAppSettings.AppLoginAsAdminNewPassword.Value, mainWindow);

      return authCookie;
    }

    private string GetAuthToken(string sitecoreIdServerUri, string userName, string password, Window mainWindow)
    {
      string idServerAuthToken = null;

      WindowHelper.LongRunningTask(() =>
      {
        var task = Task.Run(async () => {
          idServerAuthToken = await
            SitecoreIdServerAuth.GetToken(sitecoreIdServerUri, userName, password);
        });
        task?.Wait();
      }, "Get authentication token", mainWindow, "Getting authentication token", "", true);

      return idServerAuthToken;
    }

    private string ValidateAuthToken(string idServerAuthToken, string sitecoreIdServerUri, string userName, string password, Window mainWindow)
    {
      if (SitecoreIdServerAuth.CurrentHttpStatusCode == HttpStatusCode.InternalServerError)
      {
        WindowHelper.HandleError("Unable to get authentication token using the following Sitecore Identity Server URI:\n\n" +
                                 sitecoreIdServerUri, true);
        return null;
      }
      else if (SitecoreIdServerAuth.CurrentHttpStatusCode != HttpStatusCode.OK && SitecoreIdServerAuth.CurrentHttpStatusCode != HttpStatusCode.BadRequest)
      {
        WindowHelper.ShowMessage(
          "Unable to get authentication token using the following Sitecore Identity Server URI:\n\n" +
          sitecoreIdServerUri +
          "\n\nThe '" + SitecoreIdServerAuth.CurrentHttpStatusCode + "' status code has been returned." +
          "\n\nPlease make sure that this Sitecore Identity Server is running.",
          MessageBoxButton.OK,
          MessageBoxImage.Warning);
        return null;
      }

      if (SitecoreIdServerAuth.CurrentHttpStatusCode == HttpStatusCode.BadRequest)
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
            idServerAuthToken = this.GetAuthToken(credentialsContext.Uri, credentialsContext.UserName, credentialsContext.Password, mainWindow);
            this.ValidateAuthToken(idServerAuthToken, credentialsContext.Uri, credentialsContext.UserName, credentialsContext.Password, mainWindow);
          }
        }
      }

      return idServerAuthToken;
    }

    private string GetAuthClientCookie(string instanceUri, string userName, string password, Window mainWindow)
    {
      string authCookie = null;

      WindowHelper.LongRunningTask(() =>
      {
        var task = Task.Run(async () => {
          authCookie = await
            SitecoreServicesClientAuth.GetCookie(instanceUri, userName, password);
        });
        task?.Wait();
      }, "Get authentication cookie", mainWindow, "Getting authentication cookie", "", true);

      return authCookie;
    }

    private string ValidateAuthCookie(string authCookie, string instanceUri, string userName, string password, Window mainWindow)
    {
      if (SitecoreServicesClientAuth.CurrentHttpStatusCode == HttpStatusCode.InternalServerError)
      {
        WindowHelper.HandleError("Unable to get authentication cookie using the following Sitecore instance URI:\n\n" +
                                 instanceUri +
                                 "\n\nPlease make sure that a valid certificate is used for the SSL binding of your Sitecore site.", true);
        return null;
      }
      else if (SitecoreServicesClientAuth.CurrentHttpStatusCode != HttpStatusCode.OK && SitecoreServicesClientAuth.CurrentHttpStatusCode != HttpStatusCode.Forbidden)
      {
        WindowHelper.ShowMessage(
          "Unable to get authentication cookie using the following Sitecore instance URI:\n\n" +
          instanceUri +
          "\n\nThe '" + SitecoreServicesClientAuth.CurrentHttpStatusCode + "' status code has been returned." +
          "\n\nPlease make sure that this Sitecore site is running.",
          MessageBoxButton.OK,
          MessageBoxImage.Warning);
        return null;
      }

      if (SitecoreServicesClientAuth.CurrentHttpStatusCode == HttpStatusCode.Forbidden)
      {
        if (WindowHelper.ShowMessage(
              "Unable to get authentication cookie using the following credentials:\n\n" +
              "User name: " + userName + "\nPassword: " + password + "\n\nWould you like to try to continue installation using other credentials?",
              MessageBoxButton.YesNo,
              MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
          CredentialsContext credentialsContext =
            WindowHelper.ShowDialog<CredentialsDialog>(new CredentialsContext(userName, password), null) as CredentialsContext;
          if (credentialsContext != null)
          {
            authCookie = this.GetAuthClientCookie(instanceUri, credentialsContext.UserName, credentialsContext.Password, mainWindow);
            this.ValidateAuthCookie(authCookie, instanceUri, credentialsContext.UserName, credentialsContext.Password, mainWindow);
          }
        }
      }

      return authCookie;
    }

    private bool IsInstanceUriValid(string instanceUri)
    {
      if (!instanceUri.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
      {
        WindowHelper.ShowMessage(
          "The selected Sitecore instance does not use SSL/TLS connection according to the following URI:\n\n" +
          instanceUri +
          "\n\nThe SSC Auth services that are needed for package installation require an SSL/TLS connection, so you need to set up a valid certificate and SSL binding for your Sitecore site.",
          MessageBoxButton.OK,
          MessageBoxImage.Warning);
        return false;
      }

      return true;
    }
  }
}
