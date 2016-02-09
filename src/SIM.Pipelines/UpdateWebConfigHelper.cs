using System.Xml;

namespace SIM.Pipelines
{
  using System.Collections.Specialized;
  using System.IO;
  using System.Text;
  using SIM.Pipelines.Install;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public static class UpdateWebConfigHelper
  {
    #region Public methods

    public static void Process([NotNull] string rootFolderPath, [NotNull] string webRootPath, [NotNull] string dataFolder, bool serverSideRedirect, bool increaseExecutionTimeout)
    {
      Assert.ArgumentNotNull(rootFolderPath, "rootFolderPath");
      Assert.ArgumentNotNull(webRootPath, "webRootPath");
      Assert.ArgumentNotNull(dataFolder, "dataFolder");

      if (increaseExecutionTimeout)
      {
        var executionTimeout = Settings.CoreInstallHttpRuntimeExecutionTimeout.Value;
        var webConfig = XmlDocumentEx.LoadFile(Path.Combine(webRootPath, "web.config"));
        var httpRuntime = GetHttpRuntime(webConfig, true);

        httpRuntime.SetAttribute("executionTimeout", executionTimeout.ToString());
        webConfig.Save();
      }

      SetupWebsiteHelper.SetDataFolder(rootFolderPath, dataFolder);
      if (serverSideRedirect)
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

      var address = Parameters.Parse(addressString);
      var host = address[0];
      var port = address[1];

      var credentials = Parameters.Parse(credentialsString);
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

    public static XmlElement GetHttpRuntime(XmlDocument configuration, bool createIfMissing = false)
    {
      var systemWeb = configuration.SelectSingleElement("/configuration/system.web");
      if (systemWeb == null)
      {
        if (!createIfMissing)
        {
          return null;
        }

        systemWeb = configuration.CreateElement("system.web");
        configuration.DocumentElement.AppendChild(systemWeb);
      }

      var httpRuntime = systemWeb.SelectSingleElement("httpRuntime");
      if (httpRuntime != null)
      {
        return httpRuntime;
      }

      if (!createIfMissing)
      {
        return null;
      }

      httpRuntime = configuration.CreateElement("httpRuntime");
      systemWeb.AppendChild(httpRuntime);

      return httpRuntime;
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

      var includeFilePath = Path.Combine(rootFolderPath, @"Website\App_Config\Include\zzz\" + includeFileName);
      var sb = new StringBuilder();
      sb.Append(Prefix);
      foreach (string key in settings.Keys)
      {
        sb.AppendFormat(SettingFormat, key, settings[key]);
      }

      sb.Append(Postfix);

      var dir = Path.GetDirectoryName(includeFilePath);
      if (!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }

      FileSystem.FileSystem.Local.File.WriteAllText(includeFilePath, sb.ToString());
    }

    #endregion
  }
}