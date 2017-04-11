namespace SIM.Adapters.WebServer
{
  #region

  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.Linq;
  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;

  #endregion

  public class ConnectionStringCollection : List<ConnectionString>
  {
    #region Fields

    private XmlElementEx ConnectionStringsElement { get; }

    #endregion

    #region Constructors

    public ConnectionStringCollection([NotNull] XmlElementEx connectionStringsElement)
    {
      Assert.ArgumentNotNull(connectionStringsElement, nameof(connectionStringsElement));

      ConnectionStringsElement = connectionStringsElement;
    }

    #endregion

    #region Public Methods

    public void Add([NotNull] string role, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(role, nameof(role));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      XmlElement addElement = ConnectionStringsElement.Element.SelectSingleElement("add[@name='" + role + "']");
      bool exists = addElement != null;

      if (!exists)
      {
        addElement = ConnectionStringsElement.CreateElement("add");
        XmlAttribute attr1 = ConnectionStringsElement.CreateAttribute("name", role);
        addElement.Attributes.Append(attr1);
        XmlAttribute attr2 = ConnectionStringsElement.CreateAttribute("connectionString", connectionString.ConnectionString);
        addElement.Attributes.Append(attr2);
        ConnectionStringsElement.AppendChild(addElement);
      }
      else
      {
        addElement.SetAttribute("connectionString", connectionString.ConnectionString);
      }

      Save();
    }

    public void Save()
    {
      ConnectionStringsElement.Save();
    }

    [CanBeNull]
    public ConnectionString this[string name]
    {
      get
      {
        return this.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
      }
    }

    #endregion
  }
}