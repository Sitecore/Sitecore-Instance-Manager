#region Usings

using System.Data.SqlClient;
using System.Xml;
using SIM.Adapters.SqlServer;
using SIM.Base;

#endregion

namespace SIM.Adapters.WebServer
{
  #region

  

  #endregion

  /// <summary>
  ///   The connection string.
  /// </summary>
  public class ConnectionString
  {
    #region Fields

    /// <summary>
    ///   The element.
    /// </summary>
    [NotNull]
    private readonly XmlElementEx element;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionString"/> class.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="document">
    /// The document. 
    /// </param>
    public ConnectionString([NotNull] XmlElement element, [NotNull] XmlDocumentEx document) : this(new XmlElementEx(element, document))
    {
      Assert.ArgumentNotNull(element, "element");
      Assert.ArgumentNotNull(document, "document");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionString"/> class.
    /// </summary>
    /// <param name="xmlElement">
    /// The xml element. 
    /// </param>
    private ConnectionString([NotNull] XmlElementEx xmlElement)
    {
      Assert.ArgumentNotNull(xmlElement, "xmlElement");

      this.element = xmlElement;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets DefaultFileName.
    /// </summary>
    [NotNull]
    public string DefaultFileName
    {
      get
      {
        return "Sitecore." + this.Name + ".mdf";
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsSqlConnectionString.
    /// </summary>
    public bool IsSqlConnectionString
    {
      get
      {
        if (SqlServerManager.Instance.IsSqlConnectionString(this.Value))
        {
          return true;
        }

        return false;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsMongoConnectionString.
    /// </summary>
    public bool IsMongoConnectionString
    {
      get
      {
        if (SqlServerManager.Instance.IsMongoConnectionString(this.Value))
        {
          return true;
        }

        return false;
      }
    }

    /// <summary>
    ///   Gets Name.
    /// </summary>
    [NotNull]
    public string Name
    {
      get
      {
        XmlAttribute attribute = this.element.Attributes["name"];
        if (attribute != null)
        {
          return attribute.Value ?? this.element.Name;
        }

        return this.element.Name;
      }
    }

    /// <summary>
    ///   Gets or sets RealName.
    /// </summary>
    [NotNull]
    public string RealName
    {
      get
      {
        return new SqlConnectionStringBuilder(this.Value).InitialCatalog;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this.Value) {InitialCatalog = value};
        this.Value = builder.ToString();
      }
    }

    /// <summary>
    ///   Gets or sets Value.
    /// </summary>
    [CanBeNull]
    public string Value
    {
      get
      {
        XmlAttribute attribute = this.element.Attributes["connectionString"];
        return attribute == null ? null : attribute.Value;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        XmlAttribute attribute = this.element.Attributes["connectionString"] ?? this.element.CreateAttribute("connectionString");
        attribute.Value = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the name is used to attach the database to SQL Server with.
    /// </summary>
    /// <param name="instanceName">
    /// The instance Name. 
    /// </param>
    /// <returns>
    /// The name is used to attach the database to SQL Server with. 
    /// </returns>
    [NotNull]
    public string GenerateDatabaseName([NotNull] string instanceName)
    {
      Assert.ArgumentNotNull(instanceName, "instanceName");

      return SqlServerManager.Instance.GenerateDatabaseRealName(instanceName, this.Name, this.GetProductName(instanceName));
    }

    /// <summary>
    ///   The save changes.
    /// </summary>
    public void SaveChanges()
    {
      this.element.Save();
    }

    public void Delete()
    {
      var xmlElement = this.element.Element;
      xmlElement.ParentNode.RemoveChild(xmlElement);
      SaveChanges();  
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get product name.
    /// </summary>
    /// <param name="instanceName">
    /// The instance name. 
    /// </param>
    /// <returns>
    /// The get product name. 
    /// </returns>
    [NotNull]
    protected string GetProductName([NotNull] string instanceName)
    {
      Assert.ArgumentNotNull(instanceName, "instanceName");

      string value = new SqlConnectionStringBuilder(this.Value).InitialCatalog;
      string[] arr = value.Split('_');
      return arr.Length == 2 ? arr[0].TrimStart(instanceName) : string.Empty;
    }

    #endregion
  }
}