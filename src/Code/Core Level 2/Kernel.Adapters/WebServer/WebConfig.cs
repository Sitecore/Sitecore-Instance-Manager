#region Usings

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;
using SIM.Adapters.SqlServer;
using SIM.Base;
using SIM.Base.Properties;

#endregion

namespace SIM.Adapters.WebServer
{
  #region
  
  using SIM.Adapters.MongoDb;

  #endregion

  /// <summary>
  ///   The Sitecore web config helper.
  /// </summary>
  public static class WebConfig
  {
    #region Constants

    /// <summary>
    ///   The config source attribute name.
    /// </summary>
    public const string ConfigSourceAttributeName = "configSource";

    /// <summary>
    ///   The configuration x path.
    /// </summary>
    public const string ConfigurationXPath = "/configuration";
    
    /// <summary>
    ///   The package path x path.
    /// </summary>
    private const string SettingXPath = ConfigurationXPath + "/sitecore/settings/setting[@name='{0}']";

    /// <summary>
    ///   The sc variable x path.
    /// </summary>
    public const string ScVariableXPath = ConfigurationXPath + "/sitecore/sc.variable[@name='{0}']";

    #endregion

    #region Public Methods

    /// <summary>
    /// The get databases.
    /// </summary>
    /// <param name="webRootPath">
    /// The web root path. 
    /// </param>
    /// <param name="webConfigDocument"></param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static ICollection<Database> GetDatabases([NotNull] string webRootPath, XmlDocument webConfigDocument=null)
    {
      Assert.ArgumentNotNullOrEmpty(webRootPath, "webRootPath");

      using (new ProfileSection("Get databases from web.config", typeof(WebConfig)))
      {
        ProfileSection.Argument("webRootPath", webRootPath);
        ProfileSection.Argument("webConfigDocument", webConfigDocument);

        List<Database> databases = new List<Database>();
        webConfigDocument = webConfigDocument ?? GetWebConfig(webRootPath);
        XmlElement connectionStringsNode = webConfigDocument.SelectSingleNode("/configuration/connectionStrings") as XmlElement;
        if (connectionStringsNode != null)
        {
          Log.Debug("WebConfig:GetDatabases(...)#connectionStringsNode: " + connectionStringsNode.OuterXml);
          AddDatabases(connectionStringsNode, databases);
          string configSourceValue = connectionStringsNode.GetAttribute("configSource");
          Log.Debug("WebConfig:GetDatabases(...)#configSourceValue: " + configSourceValue);
          if (string.IsNullOrEmpty(configSourceValue))
          {
            return databases;
          }

          string filePath = Path.Combine(webRootPath, configSourceValue);
          Log.Debug("WebConfig:GetDatabases(...)#filePath: " + filePath);
          if (!FileSystem.Local.File.Exists(filePath))
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

      using (new ProfileSection("Get mongo databases from web.config", typeof(WebConfig)))
      {
        ProfileSection.Argument("webRootPath", webRootPath);
        ProfileSection.Argument("webConfigDocument", webConfigDocument);

        var databases = new List<MongoDbDatabase>();
        webConfigDocument = webConfigDocument ?? GetWebConfig(webRootPath);
        XmlElement connectionStringsNode = webConfigDocument.SelectSingleNode("/configuration/connectionStrings") as XmlElement;
        if (connectionStringsNode != null)
        {
          Log.Debug("WebConfig:GetDatabases(...)#connectionStringsNode: " + connectionStringsNode.OuterXml);
          AddMongoDatabases(connectionStringsNode, databases);
          string configSourceValue = connectionStringsNode.GetAttribute("configSource");
          Log.Debug("WebConfig:GetDatabases(...)#configSourceValue: " + configSourceValue);
          if (string.IsNullOrEmpty(configSourceValue))
          {
            return databases;
          }

          string filePath = Path.Combine(webRootPath, configSourceValue);
          Log.Debug("WebConfig:GetDatabases(...)#filePath: " + filePath);
          if (!FileSystem.Local.File.Exists(filePath))
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

    /// <summary>
    /// The get sc variable.
    /// </summary>
    /// <param name="webConfig">
    /// The web config. 
    /// </param>
    /// <param name="variableName">
    /// The folder name. 
    /// </param>
    /// <returns>
    /// The get sc variable. 
    /// </returns>
    [CanBeNull]
    public static string GetScVariable([NotNull] XmlDocument webConfig, [NotNull] string variableName)
    {
      Assert.ArgumentNotNull(webConfig, "webConfig");
      Assert.ArgumentNotNull(variableName, "variableName");

      using (new ProfileSection("Get Sitecore variable", typeof(WebConfig)))
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
          SIM.Base.Log.Warn("Cannot get sc variable {0}".FormatWith(variableName), typeof(WebConfig), ex);
        }
        
        return ProfileSection.Result<string>(null);
      }
    }

    /// <summary>
    /// The get sc variable element.
    /// </summary>
    /// <param name="webConfig">
    /// The web config. 
    /// </param>
    /// <param name="elementName">
    /// The element name. 
    /// </param>
    /// <returns>
    /// </returns>
    [CanBeNull]
    public static XmlElement GetScVariableElement([NotNull] XmlDocument webConfig, [NotNull] string elementName)
    {
      Assert.ArgumentNotNull(webConfig, "webConfig");
      Assert.ArgumentNotNull(elementName, "elementName");

      return webConfig.SelectSingleNode(ScVariableXPath.FormatWith(elementName)) as XmlElement;
    }

    /// <summary>
    /// The get web config.
    /// </summary>
    /// <param name="webRootPath">
    /// The web Root Path. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// XmlFileIsCorrupted
    /// </exception>
    [NotNull]
    public static XmlDocumentEx GetWebConfig([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, "webRootPath");
      string path = GetWebConfigPath(webRootPath);
      FileSystem.Local.File.AssertExists(path, BResources.FileIsMissing.FormatWith(path));

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

    #region Private methods

    /// <summary>
    /// The add databases.
    /// </summary>
    /// <param name="connectionStringsNode">
    /// The connection strings node. 
    /// </param>
    /// <param name="databases">
    /// The databases. 
    /// </param>
    public static void AddDatabases([NotNull] XmlElement connectionStringsNode, [NotNull] List<Database> databases)
    {
      Assert.ArgumentNotNull(connectionStringsNode, "connectionStringsNode");
      Assert.ArgumentNotNull(databases, "databases");

      using (new ProfileSection("Add databases to web.config", typeof(WebConfig)))
      {
        ProfileSection.Argument("connectionStringsNode", connectionStringsNode);
        ProfileSection.Argument("databases", databases);

        XmlNodeList nodes = connectionStringsNode.SelectNodes("add");
        if (nodes != null && nodes.Count > 0)
        {
          var elements = nodes.OfType<XmlElement>().ToArray();
          Log.Debug("WebConfig:AddDatabases(...)#elements.Length: " + elements.Length);
          foreach (XmlElement node in elements)
          {
            string name;

            string value = node.GetAttribute("connectionString");
            Log.Debug("WebConfig:AddDatabases(...)#value: " + value);

            if (!SqlServerManager.Instance.IsSqlConnectionString(value))
            {
              continue;
            }

            name = node.GetAttribute("name");
            Log.Debug("WebConfig:AddDatabases(...)#name: " + name);

            if (!string.IsNullOrEmpty(value))
            {
              SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(value);
              string realName = GetDatabaseName(connectionString);
              Log.Debug("WebConfig:AddDatabases(...)#realName: " + realName);
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

      using (new ProfileSection("Add mongo databases to web.config", typeof(WebConfig)))
      {
        ProfileSection.Argument("connectionStringsNode", connectionStringsNode);
        ProfileSection.Argument("databases", databases);

        XmlNodeList nodes = connectionStringsNode.SelectNodes("add");
        if (nodes != null && nodes.Count > 0)
        {
          var elements = nodes.OfType<XmlElement>().ToArray();
          Log.Debug("WebConfig:AddMongoDatabases(...)#elements.Length: " + elements.Length);
          foreach (XmlElement node in elements)
          {
            string value = node.GetAttribute("connectionString");
            Log.Debug("WebConfig:AddMongoDatabases(...)#value: " + value);

            if (SqlServerManager.Instance.IsSqlConnectionString(value))
            {
              continue;
            }

            var name = node.GetAttribute("name");
            Log.Debug("WebConfig:AddMongoDatabases(...)#name: " + name);

            databases.Add(new MongoDbDatabase(name, value));
          }
        }
      }
    }

    /// <summary>
    /// The get database name.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
    /// <returns>
    /// The get database name. 
    /// </returns>
    [CanBeNull]
    private static string GetDatabaseName([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      return connectionString.InitialCatalog;
    }




    #endregion

    /// <summary>
    /// The get web config path.
    /// </summary>
    /// <param name="webRootPath">
    /// The web root path. 
    /// </param>
    /// <returns>
    /// The get web config path. 
    /// </returns>
    [NotNull]
    public static string GetWebConfigPath([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, "webRootPath");

      return Path.Combine(webRootPath, "web.config");
    }

    #endregion

    public static string GetSitecoreSetting(string name, XmlDocument webConfigResult)
    {
      Assert.ArgumentNotNull(webConfigResult, "webConfigResult");

      XmlElement element = webConfigResult.SelectSingleNode(String.Format(SettingXPath, name)) as XmlElement;
      Assert.IsNotNull(element, String.Format("The \"{0}\" setting is missing in the instance configuration files", name));
      XmlAttribute value = element.Attributes["value"];
      Assert.IsNotNull(value, String.Format("The value attribute of the \"{0}\" setting is missing in the instance configuration files", name));
      var settingValue = value.Value;
      Assert.IsNotNullOrEmpty(settingValue, "settingValue");
      return settingValue;
    }
  }
}