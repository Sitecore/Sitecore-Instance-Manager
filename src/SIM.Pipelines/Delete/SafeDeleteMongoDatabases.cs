using JetBrains.Annotations;
using MongoDB.Driver;
using Sitecore.Diagnostics.Logging;

namespace SIM.Pipelines.Delete
{
  /// <summary>
  /// Ignores <see cref="MongoConnectionException"/> so that it is possible to 
  /// uninstall a Sitecore instance even if there is no Mongo service running.
  /// </summary>
  [UsedImplicitly]
  public class SafeDeleteMongoDatabases : DeleteMongoDatabases
  {
    protected override void Process([NotNull] DeleteArgs args)
    {
      try
      {
        base.Process(args);
      }
      catch (MongoConnectionException ex)
      {
        Log.Warn(ex, "Unable to delete Mongo databases.");
      }
    }
  }
}