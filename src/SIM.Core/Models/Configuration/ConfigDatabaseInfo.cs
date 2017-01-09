namespace SIM.Core.Models.Configuration
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using SIM.Extensions;

  public class ConfigDatabaseInfo
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public XmlElement ConnectionStringName { get; set; }

    public ConfigDatabaseWorkflowProviderInfo WorkflowProvider { get; set; }

    public IReadOnlyCollection<string> Archives { get; set; }

    public Dictionary<string, string> CacheSizes { get; set; }

    public ConfigDatabaseHistoryEngineInfo HistoryEngine { get; set; }

    public ConfigDatabaseNotificationProviderInfo NotificationProvider { get; set; }

    [CanBeNull]
    public static ConfigDatabaseInfo FromXml([CanBeNull] XmlElement element)
    {
      if (element == null)
      {
        return null;
      }

      return new ConfigDatabaseInfo
      {
        Id = element.GetAttribute("id"),
        Name = element.SelectElements("param").FirstOrDefault().InnerText,
        ConnectionStringName = element.SelectElements("connectionStringName").FirstOrDefault(),
        WorkflowProvider = ConfigDatabaseWorkflowProviderInfo.FromXml(element.SelectElements("workflowProvider").FirstOrDefault()),
        Archives = element.SelectElements("archives/archive").Select(x => x.GetAttribute("name")).ToArray(),
        CacheSizes = element.SelectElements("cacheSizes").FirstOrDefault().SelectElements("*").ToDictionary(x => x.Name, x => x.InnerText),
        HistoryEngine = ConfigDatabaseHistoryEngineInfo.FromXml(element.SelectElements("HistoryEngine").FirstOrDefault()),
        NotificationProvider = ConfigDatabaseNotificationProviderInfo.FromXml(element.SelectElements("NotificationProvider").FirstOrDefault())
      };
    }
  }
}