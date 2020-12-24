using System.IO;
using JetBrains.Annotations;

namespace SIM.Pipelines.Reinstall.Containers
{
  [UsedImplicitly]
  public class CleanupSqlDataProcessor : CleanupDataBaseProcessor
  {
    protected override string DataFolder => "mssql-data";
  }
}