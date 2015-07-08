using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Pipelines
{
  using System.Diagnostics;
  using System.IO;
  using MongoDB.Driver;
  using SIM.Adapters.MongoDb;
  using SIM.Base;
  using SIM.Instances;

  public static class MongoHelper
  {
    /// <summary>
    /// The backup.
    /// </summary>
    /// <param name="database">
    /// The database. 
    /// </param>
    /// <param name="folder">
    /// The folder. 
    /// </param>
    public static void Backup([NotNull] MongoDbDatabase database, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(database, "database");
      Assert.ArgumentNotNull(folder, "folder");

      var arguments = @"--db ""{0}"" --out ""{1}""".FormatWith(database.LogicalName, folder);
      var info = new ProcessStartInfo("mongodump.exe", arguments)
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
      var info = new ProcessStartInfo("mongorestore.exe", arguments)
      {
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden
      };

      Process
        .Start(info)
        .WaitForExit(5 * 60 * 1000);
    }
  }
}
