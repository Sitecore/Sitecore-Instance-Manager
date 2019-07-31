﻿using System.Xml;

namespace SIM.Pipelines
{
  using System.Collections.Specialized;
  using System.IO;
  using System.Text;
  using SIM.Pipelines.Install;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.Products;

  public static class UpdateWebConfigHelper
  {
    #region Public methods

    public static void Process([NotNull] string rootFolderPath, [NotNull] string webRootPath, [NotNull] string dataFolder, bool serverSideRedirect, bool increaseExecutionTimeout, Product product)
    {
      Assert.ArgumentNotNull(rootFolderPath, nameof(rootFolderPath));
      Assert.ArgumentNotNull(webRootPath, nameof(webRootPath));
      Assert.ArgumentNotNull(dataFolder, nameof(dataFolder));

      if (increaseExecutionTimeout)
      {
        var executionTimeout = Settings.CoreInstallHttpRuntimeExecutionTimeout.Value;
        if (!string.IsNullOrEmpty(executionTimeout))
        {
          var webConfig = XmlDocumentEx.LoadFile(Path.Combine(webRootPath, "web.config"));
          var httpRuntime = GetHttpRuntime(webConfig, true);
          if (httpRuntime == null)
          {
            Log.Error("Cannot extend executionTimeout as httpRuntime element is missing");
          }
          else
          {
            httpRuntime.SetAttribute("executionTimeout", executionTimeout);
            webConfig.Save();
          }
        }
      }

      SetupWebsiteHelper.SetDataFolder(rootFolderPath, dataFolder);
      if (serverSideRedirect)
      {
        CreateSettingsIncludeFile(rootFolderPath, "UseServerSideRedirect.config", new NameValueCollection
        {
          {
            "RequestErrors.UseServerSideRedirect", "true"
          }
        });
      }

      var addressString = Settings.CoreInstallMailServerAddress.Value;
      if (!string.IsNullOrEmpty(addressString))
      {
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

        CreateSettingsIncludeFile(rootFolderPath, "MailServer.config", settings);
      }

      if (product.Name == "Sitecore CMS" && product.TwoVersion.StartsWith("9.0"))
      {
        CreateSettingsIncludeFile(rootFolderPath, "DisableXdb.config", new NameValueCollection
        {
          {
            "Xdb.Enabled", "false"
          }
        });
      }
    }

    [CanBeNull]
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

    private static void CreateSettingsIncludeFile([NotNull] string rootFolderPath, [NotNull] string includeFileName, [NotNull] NameValueCollection settings)
    {
      Assert.ArgumentNotNull(rootFolderPath, nameof(rootFolderPath));
      Assert.ArgumentNotNull(includeFileName, nameof(includeFileName));
      Assert.ArgumentNotNull(settings, nameof(settings));

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

      var includeFilePath = Path.Combine(rootFolderPath, $@"Website\App_Config\Include\zzz\{includeFileName}");
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