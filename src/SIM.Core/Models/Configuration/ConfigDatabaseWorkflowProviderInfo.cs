namespace SIM.Core.Models.Configuration
{
  using System.Linq;
  using System.Xml;
  using SIM.Extensions;

  public class ConfigDatabaseWorkflowProviderInfo
  {
    public string Database { get; set; }

    public string Type { get; set; }

    public static ConfigDatabaseWorkflowProviderInfo FromXml(XmlElement element)
    {
      if (element == null)
      {
        return null;
      }

      return new ConfigDatabaseWorkflowProviderInfo
      {
        Type = element.GetAttribute("type"),
        Database = element.SelectElements("param").FirstOrDefault()?.InnerText
      };
    }
  }
}