using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SIM.Base
{
  /// <summary>
  /// Section handler for App.config file that allows to retrive XML representation of section data.
  /// </summary>
  public class AppConfigSectionHandler : System.Configuration.ConfigurationSection
  {
    public XmlDocument XmlRepresentation { get; set; }

    protected override void DeserializeSection(XmlReader reader)
    {
      var document = new XmlDocument();
      document.Load(reader);
      XmlRepresentation = document;
    }
  }
}
