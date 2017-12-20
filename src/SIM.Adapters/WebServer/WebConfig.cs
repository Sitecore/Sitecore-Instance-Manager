namespace SIM.Adapters.WebServer
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using SIM.Adapters.MongoDb;
  using SIM.Adapters.SqlServer;
  using SIM.Properties;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #endregion

  public static class WebConfig
  {
    #region Constants

    public const string ConfigSourceAttributeName = "configSource";

    public const string ConfigurationXPath = "/configuration";

    private const string SettingXPath = ConfigurationXPath + "/sitecore/settings/setting[@name='{0}']";

    #endregion

    #region Public Methods

    [NotNull]
    public static ICollection<Database> GetDatabases([NotNull] string webRootPath, XmlDocument webConfigDocument = null)
    {
      Assert.ArgumentNotNullOrEmpty(webRootPath, nameof(webRootPath));

      using (new ProfileSection("Get databases from web.config"))
      {
        ProfileSection.Argument("webRootPath", webRootPath);
        ProfileSection.Argument("webConfigDocument", webConfigDocument);

        List<Database> databases = new List<Database>();
        webConfigDocument = webConfigDocument ?? GetWebConfig(webRootPath);
        XmlElement connectionStringsNode = webConfigDocument.SelectSingleNode("/configuration/connectionStrings") as XmlElement;
        if (connectionStringsNode != null)
        {
          Log.Debug($"WebConfig:GetDatabases(...)#connectionStringsNode: {connectionStringsNode.OuterXml}");
          AddDatabases(connectionStringsNode, databases);
          var configSourceValue = connectionStringsNode.GetAttribute("configSource");
          Log.Debug($"WebConfig:GetDatabases(...)#configSourceValue: {configSourceValue}");
          if (string.IsNullOrEmpty(configSourceValue))
          {
            return databases;
          }

          var filePath = Path.Combine(webRootPath, configSourceValue);
          Log.Debug($"WebConfig:GetDatabases(...)#filePath: {filePath}");
          if (!FileSystem.FileSystem.Local.File.Exists(filePath))
          {
            return databases;
          }

          XmlDocumentEx document = XmlDocumentEx.LoadFile(filePath);
          XmlElement root = document.DocumentElement;
          if (root != null)
          {
            AddDatabases(root, databases);
          }
        }

        return ProfileSection.Result(databases);
      }
    }

    [NotNull]
    public static ICollection<MongoDbDatabase> GetMongoDatabases([NotNull] string webRootPath, XmlDocument webConfigDocument = null)
    {
      Assert.ArgumentNotNullOrEmpty(webRootPath, nameof(webRootPath));

      using (new ProfileSection("Get mongo databases from web.config"))
      {
        ProfileSection.Argument("webRootPath", webRootPath);
        ProfileSection.Argument("webConfigDocument", webConfigDocument);

        var databases = new List<MongoDbDatabase>();
        webConfigDocument = webConfigDocument ?? GetWebConfig(webRootPath);
        XmlElement connectionStringsNode = webConfigDocument.SelectSingleNode("/configuration/connectionStrings") as XmlElement;
        if (connectionStringsNode != null)
        {
          Log.Debug($"WebConfig:GetDatabases(...)#connectionStringsNode: {connectionStringsNode.OuterXml}");
          AddMongoDatabases(connectionStringsNode, databases);
          var configSourceValue = connectionStringsNode.GetAttribute("configSource");
          Log.Debug($"WebConfig:GetDatabases(...)#configSourceValue: {configSourceValue}");
          if (string.IsNullOrEmpty(configSourceValue))
          {
            return databases;
          }

          var filePath = Path.Combine(webRootPath, configSourceValue);
          Log.Debug($"WebConfig:GetDatabases(...)#filePath: {filePath}");
          if (!FileSystem.FileSystem.Local.File.Exists(filePath))
          {
            return databases;
          }

          XmlDocumentEx document = XmlDocumentEx.LoadFile(filePath);
          XmlElement root = document.DocumentElement;
          if (root != null)
          {
            AddMongoDatabases(root, databases);
          }
        }

        return ProfileSection.Result(databases);
      }
    }

    [CanBeNull]
    public static string GetScVariable([NotNull] XmlDocument webConfig, [NotNull] string variableName)
    {
      Assert.ArgumentNotNull(webConfig, nameof(webConfig));
      Assert.ArgumentNotNull(variableName, nameof(variableName));

      using (new ProfileSection("Get Sitecore variable"))
      {
        ProfileSection.Argument("webConfig", webConfig);
        ProfileSection.Argument("variableName", variableName);

        try
        {
          XmlElement dataFolderNode = GetScVariableElement(webConfig, variableName);
          if (dataFolderNode != null)
          {
            XmlAttribute valueAttr = dataFolderNode.Attributes["value"];
            if (valueAttr != null)
            {
              var result = valueAttr.Value;

              return ProfileSection.Result(result);
            }
          }
        }
        catch (Exception ex)
        {
          Log.Warn(ex, $"Cannot get sc variable {variableName}");
        }

        return ProfileSection.Result<string>(null);
      }
    }

    [CanBeNull]
    public static XmlElement GetScVariableElement([NotNull] XmlDocument webConfig, [NotNull] string elementName)
    {
      Assert.ArgumentNotNull(webConfig, nameof(webConfig));
      Assert.ArgumentNotNull(elementName, nameof(elementName));

      return webConfig.SelectSingleNode(ConfigurationXPath + $"/sitecore/sc.variable[@name='{elementName}']") as XmlElement;
    }

    [NotNull]
    public static XmlDocumentEx GetWebConfig([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, nameof(webRootPath));
      var path = GetWebConfigPath(webRootPath);
      FileSystem.FileSystem.Local.File.AssertExists(path, BResources.FileIsMissing.FormatWith(path));

      try
      {
        return XmlDocumentEx.LoadFile(path);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(BResources.XmlFileIsCorrupted.FormatWith(path), ex);
      }
    }

    #endregion

    #region Methods

    #region Public methods

    public static void AddDatabases([NotNull] XmlElement connectionStringsNode, [NotNull] List<Database> databases)
    {
      Assert.ArgumentNotNull(connectionStringsNode, nameof(connectionStringsNode));
      Assert.ArgumentNotNull(databases, nameof(databases));

      using (new ProfileSection("Add databases to web.config"))
      {
        ProfileSection.Argument("connectionStringsNode", connectionStringsNode);
        ProfileSection.Argument("databases", databases);

        XmlNodeList nodes = connectionStringsNode.SelectNodes("add");
        if (nodes != null && nodes.Count > 0)
        {
          var elements = nodes.OfType<XmlElement>().ToArray();
          Log.Debug($"WebConfig:AddDatabases(...)#elements.Length: {elements.Length}");
          foreach (XmlElement node in elements)
          {
            var value = node.GetAttribute("connectionString");
            Log.Debug($"WebConfig:AddDatabases(...)#value: {value}");

            if (!SqlServerManager.Instance.IsSqlConnectionString(value))
            {
              continue;
            }

            var name = node.GetAttribute("name");
            Log.Debug($"WebConfig:AddDatabases(...)#name: {name}");

            if (!string.IsNullOrEmpty(value))
            {
              SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(value);
              var realName = GetDatabaseName(connectionString);
              Log.Debug($"WebConfig:AddDatabases(...)#realName: {realName}");
              if (!string.IsNullOrEmpty(realName))
              {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(realName))
                {
                  Database database = new Database
                  {
                    Name = name, 
                    RealName = realName, 
                    ConnectionString = connectionString
                  };

                  databases.Add(database);
                }
              }
            }
          }
        }
      }
    }

    public static void AddMongoDatabases([NotNull] XmlElement connectionStringsNode, [NotNull] List<MongoDbDatabase> databases)
    {
      Assert.ArgumentNotNull(connectionStringsNode, nameof(connectionStringsNode));
      Assert.ArgumentNotNull(databases, nameof(databases));

      using (new ProfileSection("Add mongo databases to web.config"))
      {
        ProfileSection.Argument("connectionStringsNode", connectionStringsNode);
        ProfileSection.Argument("databases", databases);

        XmlNodeList nodes = connectionStringsNode.SelectNodes("add");
        if (nodes != null && nodes.Count > 0)
        {
          var elements = nodes.OfType<XmlElement>().ToArray();
          Log.Debug($"WebConfig:AddMongoDatabases(...)#elements.Length: {elements.Length}");
          foreach (XmlElement node in elements)
          {
            var value = node.GetAttribute("connectionString");
            Log.Debug($"WebConfig:AddMongoDatabases(...)#value: {value}");

            if (SqlServerManager.Instance.IsSqlConnectionString(value))
            {
              continue;
            }

            var name = node.GetAttribute("name");
            Log.Debug($"WebConfig:AddMongoDatabases(...)#name: {name}");

            databases.Add(new MongoDbDatabase(name, value));
          }
        }
      }
    }

    [NotNull]
    public static string GetWebConfigPath([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, nameof(webRootPath));

      return Path.Combine(webRootPath, "web.config");
    }

    #endregion

    #region Private methods

    [CanBeNull]
    private static string GetDatabaseName([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      return connectionString.InitialCatalog;
    }

    #endregion

    #endregion

    #region Public methods

    public static string GetSitecoreSetting(string name, XmlDocument webConfigResult)
    {
      Assert.ArgumentNotNull(webConfigResult, nameof(webConfigResult));

      XmlElement element = webConfigResult.SelectSingleNode(string.Format(SettingXPath, name)) as XmlElement;
      Assert.IsNotNull(element, $"The \"{name}\" setting is missing in the instance configuration files");
      XmlAttribute value = element.Attributes["value"];
      Assert.IsNotNull(value, $"The value attribute of the \"{name}\" setting is missing in the instance configuration files");
      var settingValue = value.Value;
      Assert.IsNotNullOrEmpty(settingValue, nameof(settingValue));
      return settingValue;
    }

    #endregion
  }
}