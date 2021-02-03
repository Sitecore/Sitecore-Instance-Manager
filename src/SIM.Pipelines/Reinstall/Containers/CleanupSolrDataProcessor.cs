using JetBrains.Annotations;

namespace SIM.Pipelines.Reinstall.Containers
{
  [UsedImplicitly]
  public class CleanupSolrDataProcessor : CleanupDataBaseProcessor
  {
    protected override string DataFolder => "solr-data";
  }
}