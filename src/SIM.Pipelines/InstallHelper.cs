namespace SIM.Pipelines
{
  using System;
  using System.IO;
  using System.Linq;
  using Ionic.Zip;
  using SIM.Pipelines.Install;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  public static class InstallHelper
  {
    public static void ExtractFile([NotNull] string packagePath, [NotNull] string webRootPath, [NotNull] string databasesFolderPath, [NotNull] string dataFolderPath, [CanBeNull] IPipelineController controller = null)
    {
      Assert.ArgumentNotNull(packagePath, "packagePath");
      Assert.ArgumentNotNull(webRootPath, "webRootPath");
      Assert.ArgumentNotNull(databasesFolderPath, "databasesFolderPath");
      Assert.ArgumentNotNull(dataFolderPath, "dataFolderPath");

      Log.Info("Extracting {0}".FormatWith(packagePath), typeof(InstallHelper));
      var ignore = Settings.CoreInstallRadControls.Value ? ":#!" : "sitecore/shell/RadControls";

      var ignore2 = Settings.CoreInstallDictionaries.Value ? ":#!" : "sitecore/shell/Controls/Rich Text Editor/Dictionaries";
      var incrementProgress = controller != null ? new Action<long>(controller.IncrementProgress) : null;

      const long IncrementThreathold = 1 * 1024 * 1024;
      try
      {
        using (var zip = new ZipFile(packagePath))
        {
          long lastEntriesTotalSizes = 0;
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
            if (entry.IsDirectory)
            {
              return;
            }

            var fileName = entry.FileName;
            if (fileName.StartsWith(webRootPrefix, StringComparison.OrdinalIgnoreCase))
            {
              var virtualFilePath = fileName.Substring(websitePrefixLength);
              if (virtualFilePath.StartsWith(ignore, StringComparison.OrdinalIgnoreCase))
              {
                return;
              }

              if (virtualFilePath.StartsWith(ignore2, StringComparison.OrdinalIgnoreCase))
              {
                return;
              }

              var filePath = Path.Combine(webRootPath, fileName.Substring(websitePrefixLength));
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
                return;
              }

              var filePath = Path.Combine(databasesFolderPath, fileName.Substring(databasesPrefixLength));
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
              Log.Warn("Unexpected file is ignored: " + fileName, typeof(InstallHelper));
            }

            lastEntriesTotalSizes += entry.CompressedSize;
            if (lastEntriesTotalSizes <= IncrementThreathold)
            {
              continue;
            }

            incrementProgress(lastEntriesTotalSizes);
            lastEntriesTotalSizes = 0;
          }
        }
      }
      catch (ZipException)
      {
        throw new InvalidOperationException(string.Format("The \"{0}\" package seems to be corrupted.", packagePath));
      }
    }
  }
}
