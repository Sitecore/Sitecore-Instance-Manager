namespace SIM.Core.Models.Configuration
{
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using SIM.Extensions;

  public class ConfigDatabaseNotificationProviderInfo
  {
    public string Type { get; set; }

    public string ConnectionStringName { get; set; }

    public string DatabaseName { get; set; }

    [CanBeNull]
    public static ConfigDatabaseNotificationProviderInfo FromXml([CanBeNull] XmlElement element)
    {
      if (element == null)
      {
        return null;
      }

      return new ConfigDatabaseNotificationProviderInfo
      {
        Type = element.GetAttribute("type"),
        ConnectionStringName = element.SelectElements("param").FirstOrDefault()?.GetAttribute("connectionStringName"),
        DatabaseName = element.SelectElements("param").Skip(1).FirstOrDefault()?.GetAttribute("databaseName")
      };
    }
  }
}