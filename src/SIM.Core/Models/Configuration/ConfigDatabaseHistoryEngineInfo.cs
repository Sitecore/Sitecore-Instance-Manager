namespace SIM.Core.Models.Configuration
{
  using System.Linq;
  using System.Xml;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ConfigDatabaseHistoryEngineInfo
  {
    public string Type { get; set; }

    public string ConnectionStringName { get; set; }

    [CanBeNull]
    public static ConfigDatabaseHistoryEngineInfo FromXml([CanBeNull] XmlElement element)
    {
      if (element == null)
      {
        return null;
      }
      
      return new ConfigDatabaseHistoryEngineInfo
      {
        Type = element.SelectElements("obj").FirstOrDefault()?.GetAttribute("type"),
        ConnectionStringName = element.SelectElements("obj/param").FirstOrDefault()?.GetAttribute("connectionStringName")
      };
    }
  }
}
