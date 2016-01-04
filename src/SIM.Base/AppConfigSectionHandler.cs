namespace SIM
{
  using System.Xml;

  /// <summary>
  ///   Section handler for App.config file that allows to retrive XML representation of section data.
  /// </summary>
  public class AppConfigSectionHandler : System.Configuration.ConfigurationSection
  {
    #region Public properties

    public XmlDocument XmlRepresentation { get; set; }

    #endregion

    #region Protected methods

    protected override void DeserializeSection(XmlReader reader)
    {
      var document = new XmlDocument();
      document.Load(reader);
      this.XmlRepresentation = document;
    }

    #endregion
  }
}