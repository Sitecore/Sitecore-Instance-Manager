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
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #endregion

  public static class WebConfig
  {
    #region Constants

    public const string ConfigSourceAttributeName = "configSource";

    public const string ConfigurationXPath = "/configuration";

    public const string ScVariableXPath = ConfigurationXPath + "/sitecore/sc.variable[@name='{0}']";

    private const string SettingXPath = ConfigurationXPath + "/sitecore/settings/setting[@name='{0}']";

    #endregion

    #region Public Methods

    [NotNull]
    public static ICollection<Database> GetDatabases([NotNull] string webRootPath, XmlDocument webConfigDocument = null)
    {
      Assert.ArgumentNotNullOrEmpty(webRootPath, "webRootPath");

      using (new ProfileSection("Get databases from web.config"))
      {
        ProfileSection.Argument("webRootPath", webRootPath);
        ProfileSection.Argument("webConfigDocument", webConfigDocument);

        List<Database> databases = new List<Database>();
        webConfigDocument = webConfigDocument ?? GetWebConfig(webRootPath);
        XmlElement connectionStringsNode = webConfigDocument.SelectSingleNode("/configuration/connectionStrings") as XmlElement;
        if (connectionStringsNode != null)
        {
          Log.Debug("WebConfig:GetDatabases(...)#connectionStringsNode: {0}",  connectionStringsNode.OuterXml);
          AddDatabases(connectionStringsNode, databases);
          string configSourceValue = connectionStringsNode.GetAttribute("configSource");
          Log.Debug("WebConfig:GetDatabases(...)#configSourceValue: {0}",  configSourceValue);
          if (string.IsNullOrEmpty(configSourceValue))
          {
            return databases;
          }

          string filePath = Path.Combine(webRootPath, configSourceValue);
          Log.Debug("WebConfig:GetDatabases(...)#filePath: {0}",  filePath);
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
      Assert.ArgumentNotNullOrEmpty(webRootPath, "webRootPath");

      using (new ProfileSection("Get mongo databases from web.config"))
      {
        ProfileSection.Argument("webRootPath", webRootPath);
        ProfileSection.Argument("webConfigDocument", webConfigDocument);

        var databases = new List<MongoDbDatabase>();
        webConfigDocument = webConfigDocument ?? GetWebConfig(webRootPath);
        XmlElement connectionStringsNode = webConfigDocument.SelectSingleNode("/configuration/connectionStrings") as XmlElement;
        if (connectionStringsNode != null)
        {
          Log.Debug("WebConfig:GetDatabases(...)#connectionStringsNode: {0}",  connectionStringsNode.OuterXml);
          AddMongoDatabases(connectionStringsNode, databases);
          string configSourceValue = connectionStringsNode.GetAttribute("configSource");
          Log.Debug("WebConfig:GetDatabases(...)#configSourceValue: {0}",  configSourceValue);
          if (string.IsNullOrEmpty(configSourceValue))
          {
            return databases;
          }

          string filePath = Path.Combine(webRootPath, configSourceValue);
          Log.Debug("WebConfig:GetDatabases(...)#filePath: {0}",  filePath);
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
      Assert.ArgumentNotNull(webConfig, "webConfig");
      Assert.ArgumentNotNull(variableName, "variableName");

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
          Log.Warn(ex, "Cannot get sc variable {0}", variableName);
        }

        return ProfileSection.Result<string>(null);
      }
    }

    [CanBeNull]
    public static XmlElement GetScVariableElement([NotNull] XmlDocument webConfig, [NotNull] string elementName)
    {
      Assert.ArgumentNotNull(webConfig, "webConfig");
      Assert.ArgumentNotNull(elementName, "elementName");

      return webConfig.SelectSingleNode(ScVariableXPath.FormatWith(elementName)) as XmlElement;
    }

    [NotNull]
    public static XmlDocumentEx GetWebConfig([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, "webRootPath");
      string path = GetWebConfigPath(webRootPath);
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
      Assert.ArgumentNotNull(connectionStringsNode, "connectionStringsNode");
      Assert.ArgumentNotNull(databases, "databases");

      using (new ProfileSection("Add databases to web.config"))
      {
        ProfileSection.Argument("connectionStringsNode", connectionStringsNode);
        ProfileSection.Argument("databases", databases);

        XmlNodeList nodes = connectionStringsNode.SelectNodes("add");
        if (nodes != null && nodes.Count > 0)
        {
          var elements = nodes.OfType<XmlElement>().ToArray();
          Log.Debug("WebConfig:AddDatabases(...)#elements.Length: {0}",  elements.Length);
          foreach (XmlElement node in elements)
          {
            string name;

            string value = node.GetAttribute("connectionString");
            Log.Debug("WebConfig:AddDatabases(...)#value: {0}",  value);

            if (!SqlServerManager.Instance.IsSqlConnectionString(value))
            {
              continue;
            }

            name = node.GetAttribute("name");
            Log.Debug("WebConfig:AddDatabases(...)#name: {0}",  name);

            if (!string.IsNullOrEmpty(value))
            {
              SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(value);
              string realName = GetDatabaseName(connectionString);
              Log.Debug("WebConfig:AddDatabases(...)#realName: {0}",  realName);
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
      Assert.ArgumentNotNull(connectionStringsNode, "connectionStringsNode");
      Assert.ArgumentNotNull(databases, "databases");

      using (new ProfileSection("Add mongo databases to web.config"))
      {
        ProfileSection.Argument("connectionStringsNode", connectionStringsNode);
        ProfileSection.Argument("databases", databases);

        XmlNodeList nodes = connectionStringsNode.SelectNodes("add");
        if (nodes != null && nodes.Count > 0)
        {
          var elements = nodes.OfType<XmlElement>().ToArray();
          Log.Debug("WebConfig:AddMongoDatabases(...)#elements.Length: {0}",  elements.Length);
          foreach (XmlElement node in elements)
          {
            string value = node.GetAttribute("connectionString");
            Log.Debug("WebConfig:AddMongoDatabases(...)#value: {0}",  value);

            if (SqlServerManager.Instance.IsSqlConnectionString(value))
            {
              continue;
            }

            var name = node.GetAttribute("name");
            Log.Debug("WebConfig:AddMongoDatabases(...)#name: {0}",  name);

            databases.Add(new MongoDbDatabase(name, value));
          }
        }
      }
    }

    [NotNull]
    public static string GetWebConfigPath([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, "webRootPath");

      return Path.Combine(webRootPath, "web.config");
    }

    #endregion

    #region Private methods

    [CanBeNull]
    private static string GetDatabaseName([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      return connectionString.InitialCatalog;
    }

    #endregion

    #endregion

    #region Public methods

    public static string GetSitecoreSetting(string name, XmlDocument webConfigResult)
    {
      Assert.ArgumentNotNull(webConfigResult, "webConfigResult");

      XmlElement element = webConfigResult.SelectSingleNode(string.Format(SettingXPath, name)) as XmlElement;
      Assert.IsNotNull(element, string.Format("The \"{0}\" setting is missing in the instance configuration files", name));
      XmlAttribute value = element.Attributes["value"];
      Assert.IsNotNull(value, string.Format("The value attribute of the \"{0}\" setting is missing in the instance configuration files", name));
      var settingValue = value.Value;
      Assert.IsNotNullOrEmpty(settingValue, "settingValue");
      return settingValue;
    }

    #endregion
  }
}