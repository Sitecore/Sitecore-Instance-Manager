namespace SIM.Adapters.WebServer
{
  #region

  using System.Data.SqlClient;
  using System.Xml;
  using SIM.Adapters.SqlServer;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;

  #endregion

  public class ConnectionString
  {
    #region Fields

    [NotNull]
    private XmlElementEx Element { get; }

    #endregion

    #region Constructors

    public ConnectionString([NotNull] XmlElement element, [NotNull] XmlDocumentEx document) : this(new XmlElementEx(element, document))
    {
      Assert.ArgumentNotNull(element, nameof(element));
      Assert.ArgumentNotNull(document, nameof(document));
    }

    private ConnectionString([NotNull] XmlElementEx xmlElement)
    {
      Assert.ArgumentNotNull(xmlElement, nameof(xmlElement));

      this.Element = xmlElement;
    }

    #endregion

    #region Properties

    [NotNull]
    public string DefaultFileName
    {
      get
      {
        return "Sitecore." + this.Name + ".mdf";
      }
    }

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

    [NotNull]
    public string Name
    {
      get
      {
        XmlAttribute attribute = this.Element.Attributes["name"];
        if (attribute != null)
        {
          return attribute.Value ?? this.Element.Name;
        }

        return this.Element.Name;
      }
    }

    [NotNull]
    public string RealName
    {
      get
      {
        return new SqlConnectionStringBuilder(this.Value).InitialCatalog;
      }

      set
      {
        Assert.ArgumentNotNull(value, nameof(value));

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this.Value)
        {
          InitialCatalog = value
        };
        this.Value = builder.ToString();
      }
    }

    [CanBeNull]
    public string Value
    {
      get
      {
        XmlAttribute attribute = this.Element.Attributes["connectionString"];
        return attribute == null ? null : attribute.Value;
      }

      set
      {
        Assert.ArgumentNotNull(value, nameof(value));

        XmlAttribute attribute = this.Element.Attributes["connectionString"] ?? this.Element.CreateAttribute("connectionString");
        attribute.Value = value;
      }
    }

    #endregion

    #region Public Methods

    public void Delete()
    {
      var xmlElement = this.Element.Element;
      xmlElement.ParentNode.RemoveChild(xmlElement);
      this.SaveChanges();
    }

    [NotNull]
    public string GenerateDatabaseName([NotNull] string instanceName, [NotNull] string sqlPrefix)
    {
      Assert.ArgumentNotNull(instanceName, nameof(instanceName));
      Assert.ArgumentNotNull(sqlPrefix, nameof(sqlPrefix));

      return SqlServerManager.Instance.GenerateDatabaseRealName(instanceName, sqlPrefix, this.Name, this.GetProductName(instanceName));
    }

    public void SaveChanges()
    {
      this.Element.Save();
    }

    #endregion

    #region Methods

    [NotNull]
    protected string GetProductName([NotNull] string instanceName)
    {
      Assert.ArgumentNotNull(instanceName, nameof(instanceName));

      var value = new SqlConnectionStringBuilder(this.Value).InitialCatalog;
      string[] arr = value.Split('_');
      return arr.Length == 2 ? arr[0].TrimStart(instanceName) : string.Empty;
    }

    #endregion
  }
}