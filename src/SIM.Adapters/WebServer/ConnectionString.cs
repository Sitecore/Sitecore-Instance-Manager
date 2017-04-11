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

      Element = xmlElement;
    }

    #endregion

    #region Properties

    [NotNull]
    public string DefaultFileName
    {
      get
      {
        return "Sitecore." + Name + ".mdf";
      }
    }

    public bool IsMongoConnectionString
    {
      get
      {
        if (SqlServerManager.Instance.IsMongoConnectionString(Value))
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
        if (SqlServerManager.Instance.IsSqlConnectionString(Value))
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
        XmlAttribute attribute = Element.Attributes["name"];
        if (attribute != null)
        {
          return attribute.Value ?? Element.Name;
        }

        return Element.Name;
      }
    }

    [NotNull]
    public string RealName
    {
      get
      {
        return new SqlConnectionStringBuilder(Value).InitialCatalog;
      }

      set
      {
        Assert.ArgumentNotNull(value, nameof(value));

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Value)
        {
          InitialCatalog = value
        };
        Value = builder.ToString();
      }
    }

    [CanBeNull]
    public string Value
    {
      get
      {
        XmlAttribute attribute = Element.Attributes["connectionString"];
        return attribute == null ? null : attribute.Value;
      }

      set
      {
        Assert.ArgumentNotNull(value, nameof(value));

        XmlAttribute attribute = Element.Attributes["connectionString"] ?? Element.CreateAttribute("connectionString");
        attribute.Value = value;
      }
    }

    #endregion

    #region Public Methods

    public void Delete()
    {
      var xmlElement = Element.Element;
      xmlElement.ParentNode.RemoveChild(xmlElement);
      SaveChanges();
    }

    [NotNull]
    public string GenerateDatabaseName([NotNull] string instanceName, [NotNull] string sqlPrefix)
    {
      Assert.ArgumentNotNull(instanceName, nameof(instanceName));
      Assert.ArgumentNotNull(sqlPrefix, nameof(sqlPrefix));

      return SqlServerManager.Instance.GenerateDatabaseRealName(instanceName, sqlPrefix, Name, GetProductName(instanceName));
    }

    public void SaveChanges()
    {
      Element.Save();
    }

    #endregion

    #region Methods

    [NotNull]
    protected string GetProductName([NotNull] string instanceName)
    {
      Assert.ArgumentNotNull(instanceName, nameof(instanceName));

      var value = new SqlConnectionStringBuilder(Value).InitialCatalog;
      string[] arr = value.Split('_');
      return arr.Length == 2 ? arr[0].TrimStart(instanceName) : string.Empty;
    }

    #endregion
  }
}