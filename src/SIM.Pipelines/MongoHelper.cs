namespace SIM.Pipelines
{
  using System.Diagnostics;
  using System.IO;
  using SIM.Adapters.MongoDb;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;

  public static class MongoHelper
  {
    #region Public methods

    public static void Backup([NotNull] MongoDbDatabase database, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(database, nameof(database));
      Assert.ArgumentNotNull(folder, nameof(folder));

      var arguments = @"--db ""{0}"" --out ""{1}""".FormatWith(database.LogicalName, folder);
      var info = new ProcessStartInfo(ApplicationManager.GetEmbeddedFile("mongo.tools.zip", "SIM.Pipelines", "mongodump.exe"), arguments)
      {
        CreateNoWindow = true, 
        WindowStyle = ProcessWindowStyle.Hidden
      };

      Process
        .Start(info)
        .WaitForExit(5 * 60 * 1000);
    }

    public static void Restore([NotNull] string directoryPath)
    {
      Assert.ArgumentNotNull(directoryPath, nameof(directoryPath));

      var logicalName = Path.GetFileName(directoryPath);
      Restore(directoryPath, logicalName);
    }

    public static void Restore([NotNull] string directoryPath, [NotNull] string newDatabaseName)
    {
      Assert.ArgumentNotNull(directoryPath, nameof(directoryPath));
      Assert.ArgumentNotNull(newDatabaseName, nameof(newDatabaseName));

      var arguments = @"--db ""{0}"" ""{1}""".FormatWith(newDatabaseName, directoryPath);
      var info = new ProcessStartInfo(ApplicationManager.GetEmbeddedFile("mongo.tools.zip", "SIM.Pipelines", "mongorestore.exe"), arguments)
      {
        CreateNoWindow = true, 
        WindowStyle = ProcessWindowStyle.Hidden
      };

      Process
        .Start(info)
        .WaitForExit(5 * 60 * 1000);
    }

    #endregion
  }
}