#region Usings

using System;
using System.IO;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Export
{
  using System.Data.SqlClient;
  using System.Linq;
  using System.Xml;
  using Adapters.WebServer;

  /// <summary>
  ///   The backup files.
  /// </summary>
  [UsedImplicitly]
  public class ExportFiles : ExportProcessor
  {
    #region Methods

    #region Protected methods

    /// <summary>
    /// The evaluate steps count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The evaluate steps count. 
    /// </returns>
    protected override long EvaluateStepsCount(ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 1 + 10;
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var instance = args.Instance;
      var rootFolder = instance.RootPath;
      var webRootPath = instance.WebRootPath;
      var webRootName = instance.WebRootPath.Split('\\')[instance.WebRootPath.Split('\\').Length - 1];
      var websiteFolder = Path.Combine(args.Folder, webRootName);
      var dataFolder = Path.Combine(args.Folder, "data");


      BackupFolder(args, webRootPath, webRootName);

      // 0
      IncrementProgress();

      BackupFolder(args, instance.DataFolderPath, "Data");

      // 1
      IncrementProgress();

      if (args.WipeSqlServerCredentials) WipeSqlServerCredentials(websiteFolder);

      // 2
      IncrementProgress();

      if (!args.IncludeTempFolderContents) WipeTempFolderContents(websiteFolder);

      // 3
      IncrementProgress();

      if (!args.IncludeMediaCacheFolderContents) WipeMediaCacheFolderContents(websiteFolder);

      // 4
      IncrementProgress();

      if (args.ExcludeUploadFolderContents) WipeUploadFolderContents(websiteFolder);

      // 5
      IncrementProgress();

      if (true) WipeViewStateFolderContents(dataFolder);

      // 6
      IncrementProgress();

      if (args.ExcludeDiagnosticsFolderContents) WipDiagnosticsFolderContents(dataFolder);

      // 7
      IncrementProgress();

      if (args.ExcludeLogsFolderContents) WipeLogsFolderContents(dataFolder);

      // 8
      IncrementProgress();

      if (args.ExcludePackagesFolderContents) WipePackagesFolderContents(dataFolder);

      // 9
      IncrementProgress();

      if (args.ExcludeLicenseFile) WipeLicenseFile(dataFolder);
    }

    private void WipeLicenseFile(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "license.xml");
      FileSystem.Local.File.DeleteIfExists(path);
    }

    private void WipePackagesFolderContents(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "packages");
      if (FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeLogsFolderContents(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "logs");
      if (FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipDiagnosticsFolderContents(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "diagnostics");
      if (FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeViewStateFolderContents(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "viewstate");
      if (FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeUploadFolderContents(string websiteFolder)
    {
      var path = Path.Combine(websiteFolder, "upload");
      if (FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeMediaCacheFolderContents(string websiteFolder)
    {
      var path = Path.Combine(websiteFolder, "App_Data\\MediaCache");
      if (FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeTempFolderContents(string websiteFolder)
    {
      var path = Path.Combine(websiteFolder, "temp");
      if (FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// The backup folder.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="fileName">
    /// The file name. 
    /// </param>
    private void BackupFolder([NotNull] ExportArgs args, [NotNull] string path, [NotNull] string fileName)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(path, "path");
      Assert.ArgumentNotNull(fileName, "fileName");

      if (!FileSystem.Local.Directory.Exists(path)) return;

      var backupFolder = Path.Combine(args.Folder, fileName);
      DirectoryCopy(path, backupFolder);
    }

    private static void DirectoryCopy(string sourceDirectory, string targetDirectory)
    {
      FileSystem.Local.Directory.Copy(sourceDirectory, targetDirectory, true);
    }

    private static void WipeSqlServerCredentials(string webRootPath)
    {
      var pathToConnectionStringsConfig = webRootPath.PathCombine("App_Config").PathCombine("ConnectionStrings.config");

      var connectionStringsDocument = new XmlDocumentEx();
      connectionStringsDocument.Load(pathToConnectionStringsConfig);

      var connectionsStringsElement = new XmlElementEx(connectionStringsDocument.DocumentElement, connectionStringsDocument);

      var connectionStrings = new ConnectionStringCollection(connectionsStringsElement);
      connectionStrings.AddRange(connectionsStringsElement.Element.ChildNodes.OfType<XmlElement>().Select(element => new ConnectionString(element, connectionsStringsElement.Document)));

      foreach (var connectionString in connectionStrings)
      {
        if (!connectionString.IsSqlConnectionString) continue;

        var builder = new SqlConnectionStringBuilder(connectionString.Value) { DataSource = string.Empty, UserID = string.Empty, Password = string.Empty };
        connectionString.Value = builder.ToString();
      }

      connectionStrings.Save();
    }

    #endregion

    #endregion
  }
}
