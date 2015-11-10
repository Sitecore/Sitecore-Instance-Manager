namespace SIM.Pipelines.Export
{
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ExportFiles : ExportProcessor
  {
    #region Methods

    #region Protected methods

    protected override long EvaluateStepsCount(ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 1 + 10;
    }

    protected override void Process([NotNull] ExportArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var instance = args.Instance;
      var webRootPath = instance.WebRootPath;
      var webRootName = instance.WebRootPath.Split('\\')[instance.WebRootPath.Split('\\').Length - 1];
      var websiteFolder = Path.Combine(args.Folder, webRootName);
      var dataFolder = Path.Combine(args.Folder, "data");


      this.BackupFolder(args, webRootPath, webRootName);

      // 0
      this.IncrementProgress();

      this.BackupFolder(args, instance.DataFolderPath, "Data");

      // 1
      this.IncrementProgress();

      if (args.WipeSqlServerCredentials)
      {
        WipeSqlServerCredentials(websiteFolder);
      }

      // 2
      this.IncrementProgress();

      if (!args.IncludeTempFolderContents)
      {
        this.WipeTempFolderContents(websiteFolder);
      }

      // 3
      this.IncrementProgress();

      if (!args.IncludeMediaCacheFolderContents)
      {
        this.WipeMediaCacheFolderContents(websiteFolder);
      }

      // 4
      this.IncrementProgress();

      if (args.ExcludeUploadFolderContents)
      {
        this.WipeUploadFolderContents(websiteFolder);
      }

      // 5
      this.IncrementProgress();

      if (true)
      {
        this.WipeViewStateFolderContents(dataFolder);
      }

      // 6
      this.IncrementProgress();

      if (args.ExcludeDiagnosticsFolderContents)
      {
        this.WipDiagnosticsFolderContents(dataFolder);
      }

      // 7
      this.IncrementProgress();

      if (args.ExcludeLogsFolderContents)
      {
        this.WipeLogsFolderContents(dataFolder);
      }

      // 8
      this.IncrementProgress();

      if (args.ExcludePackagesFolderContents)
      {
        this.WipePackagesFolderContents(dataFolder);
      }

      // 9
      this.IncrementProgress();

      if (args.ExcludeLicenseFile)
      {
        this.WipeLicenseFile(dataFolder);
      }
    }

    #endregion

    #region Private methods

    private static void DirectoryCopy(string sourceDirectory, string targetDirectory)
    {
      FileSystem.FileSystem.Local.Directory.Copy(sourceDirectory, targetDirectory, true);
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
        if (!connectionString.IsSqlConnectionString)
        {
          continue;
        }

        var builder = new SqlConnectionStringBuilder(connectionString.Value)
        {
          DataSource = string.Empty, 
          UserID = string.Empty, 
          Password = string.Empty
        };
        connectionString.Value = builder.ToString();
      }

      connectionStrings.Save();
    }

    private void BackupFolder([NotNull] ExportArgs args, [NotNull] string path, [NotNull] string fileName)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(path, "path");
      Assert.ArgumentNotNull(fileName, "fileName");

      if (!FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        return;
      }

      var backupFolder = Path.Combine(args.Folder, fileName);
      DirectoryCopy(path, backupFolder);
    }

    private void WipDiagnosticsFolderContents(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "diagnostics");
      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeLicenseFile(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "license.xml");
      FileSystem.FileSystem.Local.File.DeleteIfExists(path);
    }

    private void WipeLogsFolderContents(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "logs");
      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeMediaCacheFolderContents(string websiteFolder)
    {
      var path = Path.Combine(websiteFolder, "App_Data\\MediaCache");
      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipePackagesFolderContents(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "packages");
      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeTempFolderContents(string websiteFolder)
    {
      var path = Path.Combine(websiteFolder, "temp");
      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeUploadFolderContents(string websiteFolder)
    {
      var path = Path.Combine(websiteFolder, "upload");
      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    private void WipeViewStateFolderContents(string dataFolder)
    {
      var path = Path.Combine(dataFolder, "viewstate");
      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        FileSystem.FileSystem.Local.Directory.DeleteContents(path);
      }
    }

    #endregion

    #endregion
  }
}