using System.Collections.Specialized;
using System.IO;
using System.Text;
using SIM.Pipelines.Install;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.Annotations;

namespace SIM.Pipelines
{
  public static class UpdateWebConfigHelper
  {
    #region Public methods

    public static void Process([NotNull] string rootFolderPath, [NotNull] string webRootPath, [NotNull] string dataFolder)
    {
      Assert.ArgumentNotNull(rootFolderPath, "rootFolderPath");
      Assert.ArgumentNotNull(webRootPath, "webRootPath");
      Assert.ArgumentNotNull(dataFolder, "dataFolder");

      SetupWebsiteHelper.SetDataFolder(rootFolderPath, dataFolder);
      if (Settings.CoreInstallNotFoundTransfer.Value)
      {
        CreateIncludeFile(rootFolderPath, "UseServerSideRedirect.config", new NameValueCollection
        {
          {
            "RequestErrors.UseServerSideRedirect", "true"
          }
        });
      }

      var addressString = Settings.CoreInstallMailServerAddress.Value;
      if (string.IsNullOrEmpty(addressString))
      {
        return;
      }

      var credentialsString = Settings.CoreInstallMailServerCredentials.Value;

      var address = (addressString + ":").Split(':');
      var host = address[0];
      var port = address[1];

      var credentials = (credentialsString + ":").Split(':');
      var username = credentials[0];
      var password = credentials[1];

      var settings = new NameValueCollection
      {
        {
          "MailServer", host
        }, 
        {
          "MailServerPort", port
        }, 
        {
          "MailServerUserName", username
        }, 
        {
          "MailServerPassword", password
        }
      };

      CreateIncludeFile(rootFolderPath, "MailServer.config", settings);
    }

    #endregion

    #region Private methods

    private static void CreateIncludeFile([NotNull] string rootFolderPath, [NotNull] string includeFileName, [NotNull] NameValueCollection settings)
    {
      Assert.ArgumentNotNull(rootFolderPath, "rootFolderPath");
      Assert.ArgumentNotNull(includeFileName, "includeFileName");
      Assert.ArgumentNotNull(settings, "settings");

      const string Prefix = @"<configuration xmlns:patch=""http://www.sitecore.net/xmlconfig/"">
  <sitecore>
    <settings>";

      const string SettingFormat = @"
      <setting name=""{0}"">
        <patch:attribute name=""value"">{1}</patch:attribute>
      </setting>";

      const string Postfix = @"
    </settings>
  </sitecore>
</configuration>";

      var includeFilePath = Path.Combine(rootFolderPath, @"Website\App_Config\Include\" + includeFileName);
      var sb = new StringBuilder();
      sb.Append(Prefix);
      foreach (string key in settings.Keys)
      {
        sb.AppendFormat(SettingFormat, key, settings[key]);
      }

      sb.Append(Postfix);
      FileSystem.FileSystem.Local.File.WriteAllText(includeFilePath, sb.ToString());
    }

    #endregion
  }
}