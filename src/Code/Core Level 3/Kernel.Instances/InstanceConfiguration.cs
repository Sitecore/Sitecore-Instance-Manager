#region Usings

using System.IO;
using System.Linq;
using System.Xml;
using SIM.Adapters.WebServer;
using SIM.Base;

#endregion

namespace SIM.Instances
{
  #region

  

  #endregion

  /// <summary>
  ///   The instance configuration.
  /// </summary>
  public sealed class InstanceConfiguration
  {
    #region Fields

    /// <summary>
    ///   The instance.
    /// </summary>
    [NotNull]
    private readonly Instance instance;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceConfiguration"/> class.
    /// </summary>
    /// <param name="instance">
    /// The instance. 
    /// </param>
    public InstanceConfiguration([NotNull] Instance instance)
    {
      Assert.ArgumentNotNull(instance, "instance");

      this.instance = instance;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets ConnectionStrings.
    /// </summary>
    [NotNull]
    public ConnectionStringCollection ConnectionStrings
    {
      get
      {
        XmlElementEx connectionStringsNode = this.GetConnectionStringsElement();
        return GetConnectionStringCollection(connectionStringsNode);
      }
    }

    private static ConnectionStringCollection GetConnectionStringCollection(XmlElementEx connectionStringsNode)
    {
      ConnectionStringCollection connectionStrings = new ConnectionStringCollection(connectionStringsNode);
      XmlNodeList addNodes = connectionStringsNode.Element.ChildNodes;
      connectionStrings.AddRange(
        addNodes.OfType<XmlElement>().Select(element => new ConnectionString(element, connectionStringsNode.Document)));

      return connectionStrings;
    }

    #endregion

    #region Methods

    /// <summary>
    ///   The get connection strings element.
    /// </summary>
    /// <returns> </returns>
    [NotNull]
    private XmlElementEx GetConnectionStringsElement()
    {
      XmlDocumentEx webConfig = this.instance.GetWebConfig();
      Assert.IsNotNull(webConfig, "webConfig");

      return GetConnectionStringsElement(webConfig);
    }

    private static XmlElementEx GetConnectionStringsElement(XmlDocumentEx webConfig)
    {
      var webRootPath = Path.GetDirectoryName(webConfig.FilePath);
      XmlElement configurationNode = webConfig.SelectSingleNode(WebConfig.ConfigurationXPath) as XmlElement;
      Assert.IsNotNull(configurationNode,
                       "The {0} element is missing in the {1} file".FormatWith("/configuration", webConfig.FilePath));
      XmlElement webConfigConnectionStrings = configurationNode.SelectSingleNode("connectionStrings") as XmlElement;
      Assert.IsNotNull(webConfigConnectionStrings,
                       "The web.config file doesn't contain the /configuration/connectionStrings node");
      XmlAttribute configSourceAttribute = webConfigConnectionStrings.Attributes[WebConfig.ConfigSourceAttributeName];
      if (configSourceAttribute != null)
      {
        string configSourceValue = configSourceAttribute.Value;
        if (!string.IsNullOrEmpty(configSourceValue) && !string.IsNullOrEmpty(webRootPath))
        {
          string filePath = Path.Combine(webRootPath, configSourceValue);
          if (FileSystem.Local.File.Exists(filePath))
          {
            XmlDocumentEx connectionStringsConfig = XmlDocumentEx.LoadFile(filePath);
            XmlElement connectionStrings = connectionStringsConfig.SelectSingleNode("/connectionStrings") as XmlElement;
            if (connectionStrings != null)
            {
              return new XmlElementEx(connectionStrings, connectionStringsConfig);
            }
          }
        }
      }

      return new XmlElementEx(webConfigConnectionStrings, webConfig);
    }

    #endregion

    public static ConnectionStringCollection GetConnectionStrings(string webRootPath)
    {
      XmlElementEx connectionStringsNode = GetConnectionStringsElement(XmlDocumentEx.LoadFile(Path.Combine(webRootPath, "web.config")));
      return GetConnectionStringCollection(connectionStringsNode);
    }
  }
}