#region Usings

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;
using SIM.Base;

#endregion

namespace SIM.Adapters.WebServer
{
  #region

  

  #endregion

  /// <summary>
  ///   The connection string collection.
  /// </summary>
  public class ConnectionStringCollection : List<ConnectionString>
  {
    #region Fields

    /// <summary>
    ///   The connection strings element.
    /// </summary>
    private readonly XmlElementEx connectionStringsElement;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringCollection"/> class.
    /// </summary>
    /// <param name="connectionStringsElement">
    /// The connection strings element. 
    /// </param>
    public ConnectionStringCollection([NotNull] XmlElementEx connectionStringsElement)
    {
      Assert.ArgumentNotNull(connectionStringsElement, "connectionStringsElement");

      this.connectionStringsElement = connectionStringsElement;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="role">
    /// The role. 
    /// </param>
    /// <param name="connectionString">
    /// The connection string. 
    /// </param>
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

      Save();
    }

    public void Save()
    {
      this.connectionStringsElement.Save();
    }

    #endregion
  }
}