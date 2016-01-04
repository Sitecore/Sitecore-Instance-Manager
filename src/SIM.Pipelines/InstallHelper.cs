namespace SIM.Pipelines
{
  using System;
  using System.IO;
  using System.Linq;
  using Ionic.Zip;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public static class InstallHelper
  {
    public const string RadControls = "sitecore/shell/RadControls";
    public const string Dictionaries = "sitecore/shell/Controls/Rich Text Editor/Dictionaries";
      
    public static void ExtractFile([NotNull] string packagePath, [NotNull] string webRootPath, [NotNull] string databasesFolderPath, [NotNull] string dataFolderPath, bool installRadControls, bool installDictionaries, [CanBeNull] IPipelineController controller = null)
    {
      Assert.ArgumentNotNull(packagePath, "packagePath");
      Assert.ArgumentNotNull(webRootPath, "webRootPath");
      Assert.ArgumentNotNull(databasesFolderPath, "databasesFolderPath");
      Assert.ArgumentNotNull(dataFolderPath, "dataFolderPath");

      Log.Info("Extracting {0}", packagePath);
      var ignore = installRadControls ? ":#!" : RadControls;

      var ignore2 = installDictionaries ? ":#!" : Dictionaries;
      var incrementProgress = controller != null ? new Action<long>(controller.IncrementProgress) : null;

      try
      {
        using (var zip = new ZipFile(packagePath))
        {
          var prefix = zip.Entries.First().FileName;
          prefix = prefix.Substring(0, prefix.IndexOf('/', 1));

          var webRootPrefix = prefix + "/Website/";
          var websitePrefixLength = webRootPrefix.Length;

          var databasesPrefix = prefix + "/Databases/";
          var databasesPrefixLength = databasesPrefix.Length;

          var dataPrefix = prefix + "/Data/";
          var dataPrefixLength = dataPrefix.Length;

          foreach (var entry in zip.Entries)
          {
            if (incrementProgress != null)
            {
              incrementProgress(1);
            }

            var fileName = entry.FileName;
            if (fileName.StartsWith(webRootPrefix, StringComparison.OrdinalIgnoreCase))
            {
              var virtualFilePath = fileName.Substring(websitePrefixLength);
              if (virtualFilePath.StartsWith(ignore, StringComparison.OrdinalIgnoreCase))
              {
                continue;
              }

              if (virtualFilePath.StartsWith(ignore2, StringComparison.OrdinalIgnoreCase))
              {
                continue;
              }

              var filePath = Path.Combine(webRootPath, fileName.Substring(websitePrefixLength));
              if (entry.IsDirectory)
              {
                Directory.CreateDirectory(filePath);
                continue;
              }

              var folder = Path.GetDirectoryName(filePath);
              if (!Directory.Exists(folder))
              {
                Directory.CreateDirectory(folder);
              }

              using (var write = new StreamWriter(filePath))
              {
                entry.Extract(write.BaseStream);
              }
            }
            else if (fileName.StartsWith(databasesPrefix, StringComparison.OrdinalIgnoreCase))
            {
              if (fileName.EndsWith(".ldf"))
              {
                continue;
              }

              var filePath = Path.Combine(databasesFolderPath, fileName.Substring(databasesPrefixLength));
              if (entry.IsDirectory)
              {
                Directory.CreateDirectory(filePath);
                continue;
              }

              var folder = Path.GetDirectoryName(filePath);
              if (!Directory.Exists(folder))
              {
                Directory.CreateDirectory(folder);
              }

              using (var write = new StreamWriter(filePath))
              {
                entry.Extract(write.BaseStream);
              }
            }
            else if (fileName.StartsWith(dataPrefix, StringComparison.OrdinalIgnoreCase))
            {
              var filePath = Path.Combine(dataFolderPath, fileName.Substring(dataPrefixLength));
              if (entry.IsDirectory)
              {
                Directory.CreateDirectory(filePath);
                continue;
              }

              var folder = Path.GetDirectoryName(filePath);
              if (!Directory.Exists(folder))
              {
                Directory.CreateDirectory(folder);
              }

              using (var write = new StreamWriter(filePath))
              {
                entry.Extract(write.BaseStream);
              }
            }
            else
            {
              Log.Warn("Unexpected file or directory is ignored: {0}",  fileName);
            }
          }
        }
      }
      catch (ZipException)
      {
        throw new InvalidOperationException(string.Format("The \"{0}\" package seems to be corrupted.", packagePath));
      }
    }

    public static long GetStepsCount(string packagePath)
    {
      using (var zip = new ZipFile(packagePath))
      {
        return zip.Entries.Count;
      }
    }
  }
}
