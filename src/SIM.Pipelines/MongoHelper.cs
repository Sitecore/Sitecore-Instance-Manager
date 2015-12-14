namespace SIM.Pipelines
{
  using System.Diagnostics;
  using System.IO;
  using SIM.Adapters.MongoDb;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public static class MongoHelper
  {
    #region Public methods

    public static void Backup([NotNull] MongoDbDatabase database, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(database, "database");
      Assert.ArgumentNotNull(folder, "folder");

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
      Assert.ArgumentNotNull(directoryPath, "filePath");

      var logicalName = Path.GetFileName(directoryPath);
      var arguments = @"--db ""{0}"" ""{1}""".FormatWith(logicalName, directoryPath);
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