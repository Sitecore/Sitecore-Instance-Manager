namespace SIM.Adapters.WebServer
{
  #region

  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #endregion

  public class ConnectionStringCollection : List<ConnectionString>
  {
    #region Fields

    private readonly XmlElementEx connectionStringsElement;

    #endregion

    #region Constructors

    public ConnectionStringCollection([NotNull] XmlElementEx connectionStringsElement)
    {
      Assert.ArgumentNotNull(connectionStringsElement, "connectionStringsElement");

      this.connectionStringsElement = connectionStringsElement;
    }

    #endregion

    #region Public Methods

    public void Add([NotNull] string role, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(role, "role");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      XmlElement addElement = this.connectionStringsElement.Element.SelectSingleElement("add[@name='" + role + "']");
      bool exists = addElement != null;

      if (!exists)
      {
        addElement = this.connectionStringsElement.CreateElement("add");
        XmlAttribute attr1 = this.connectionStringsElement.CreateAttribute("name", role);
        addElement.Attributes.Append(attr1);
        XmlAttribute attr2 = this.connectionStringsElement.CreateAttribute("connectionString", connectionString.ConnectionString);
        addElement.Attributes.Append(attr2);
        this.connectionStringsElement.AppendChild(addElement);
      }
      else
      {
        addElement.SetAttribute("connectionString", connectionString.ConnectionString);
      }

      this.Save();
    }

    public void Save()
    {
      this.connectionStringsElement.Save();
    }

    #endregion
  }
}