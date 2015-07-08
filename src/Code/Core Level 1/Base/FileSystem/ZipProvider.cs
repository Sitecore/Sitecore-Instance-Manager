using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Ionic.Zlib;

namespace SIM.Base
{
  public class ZipProvider
  {
    private readonly FileSystem fileSystem;

    public ZipProvider(FileSystem fileSystem)
    {
      this.fileSystem = fileSystem;
    }

    public virtual bool ZipContainsSingleFile(string packagePath, string innerFileName)
    {
      var fileInfo = new FileInfo(packagePath);
      Assert.IsTrue(fileInfo.Exists, "The {0} file does not exist".FormatWith(packagePath));

      if (fileInfo.Length <= 22)
      {
        return false;
      }

      using (ZipFile zip = new ZipFile(packagePath))
      {
        if (zip.Entries.Count == 1 && zip.Entries.First().FileName == innerFileName)
        {
          return true;
        }
      }

      return false;
    }

    public virtual bool ZipContainsFile(string packagePath, string innerFileName)
    {
      var fileInfo = new FileInfo(packagePath);
      Assert.IsTrue(fileInfo.Exists, "The {0} file does not exist".FormatWith(packagePath));

      if (fileInfo.Length <= 22)
      {
        return false;
      }

      using (ZipFile zip = new ZipFile(packagePath))
      {
        if (zip.Entries.Any(e => e.FileName.EqualsIgnoreCase(innerFileName)))
        {
          return true;
        }
      }

      return false;
    }

    public virtual string ZipUnpackFile(string pathToZip, string pathToUnpack, string fileName)
    {
      string zipToUnpack = pathToZip;
      string unpackDirectory = pathToUnpack;
      using (ZipFile zip = ZipFile.Read(zipToUnpack))
      {
        foreach (ZipEntry entry in zip)
        {
          string[] splittedFileName = entry.FileName.Split('/');
          if (splittedFileName[splittedFileName.Length - 1] == fileName)
          {
            entry.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
            return Path.Combine(pathToUnpack, fileName);
          }
        }
      }
      return "";
    }

    public virtual string ZipUnpackFolder(string pathToZip, string pathToUnpack, string folderName)
    {
      using (ZipFile zip1 = ZipFile.Read(pathToZip))
      {
        var selection = (from e in zip1.Entries
          where (e.FileName).StartsWith(folderName + "/")
          select e);


        Directory.CreateDirectory(pathToUnpack);

        foreach (var e in selection)
        {
          e.Extract(pathToUnpack, ExtractExistingFileAction.OverwriteSilently);
        }
      }
      return pathToUnpack.PathCombine(folderName);
    }

    public virtual void CreateZip(string path, string zipFileName, string ignore = null)
    {
      var zip = new ZipFile
      {
        CompressionLevel = CompressionLevel.None,
        UseZip64WhenSaving = Zip64Option.AsNecessary
      };
      if (ignore == null)
      {
        zip.AddDirectory(path); // , relativePath);
      }
      else
      {
        Assert.IsTrue(!ignore.Contains('\\') && !ignore.Contains('/'), "Multi-level ignore is not supported for archiving");
        foreach (var directory in Directory.GetDirectories(path))
        {
          string directoryName = new DirectoryInfo(directory).Name;
          if (!directoryName.EqualsIgnoreCase(ignore))
          {
            zip.AddDirectory(directory, directoryName);
          }
        }
        foreach (var file in Directory.GetFiles(path))
        {
          zip.AddFile(file, "/");
        }
      }

      // zip.SaveProgress += this.OnSaveProgress;
      zip.Save(zipFileName);
    }
    public virtual void UnpackZipWithActualWebRootName([NotNull] string packagePath, [NotNull] string path, string webRootName, [CanBeNull] string entriesPattern = null, int stepsCount = 1, [CanBeNull] Action incrementProgress = null)
    {
      Assert.ArgumentNotNull(packagePath, "packagePath");
      Assert.ArgumentNotNull(path, "path");

      // TODO: comment this line when the progress bar is adjusted
      incrementProgress = null;

      var hasResponse = incrementProgress != null;
      if (!System.IO.File.Exists(packagePath)) return;

      try
      {
        Log.Info("Unzipping {2}the '{0}' archive to the '{1}' folder".FormatWith(packagePath, path, entriesPattern != null ? "\"" + entriesPattern + "\"" + " files of " : string.Empty), typeof(FileSystem));

        using (var zip = new ZipFile(packagePath))
        {
          var q = Math.Max(zip.Entries.Count / stepsCount, 1);

          this.fileSystem.Directory.Ensure(path);
          var i = 0;
          var entries = entriesPattern != null ? zip.SelectEntries(entriesPattern) : zip.Entries;

          for (var j = 0; j < entries.Count(); j++)
          {
            try
            {
              if (entries.ElementAt(j).FileName.StartsWith("Website", true, CultureInfo.InvariantCulture))
              {
                entries.ElementAt(j).FileName = webRootName + entries.ElementAt(j).FileName.Substring(7);
              }

              entries.ElementAt(j).Extract(path, ExtractExistingFileAction.OverwriteSilently);
            }
            catch (IOException)
            {
              var b = false;
              foreach (var postFix in new[] { ".tmp", ".PendingOverwrite" })
              {
                var errorPath = Path.Combine(path, entries.ElementAt(j).FileName) + postFix;
                if (!System.IO.File.Exists(errorPath)) continue;
                System.IO.File.Delete(errorPath);
                b = true;
              }

              if (!b)
              {
                throw;
              }

              entries.ElementAt(j).Extract(path, ExtractExistingFileAction.OverwriteSilently);
            }

            if (!hasResponse) continue;
            if (++i != q) continue;
            incrementProgress();
            i = 0;
          }
        }
      }
      catch (ZipException)
      {
        throw new InvalidOperationException(string.Format("The \"{0}\" package seems to be corrupted.", packagePath));
      }
    }

    private const long KB = 1024;
    private const long MB = 1024 * KB;

    public virtual void UnpackZip([NotNull] string packagePath, [NotNull] string path, [CanBeNull] Action<long> incrementProgress, string ignore1 = null, string ignore2 = null)
    {
      Assert.ArgumentNotNull(packagePath, "packagePath");
      Assert.ArgumentNotNull(path, "path");

      if (System.IO.File.Exists(packagePath))
      {
        try
        {
          Log.Info("Unzipping the '{0}' archive to the '{1}' folder".FormatWith(packagePath, path), typeof(FileSystem));

          using (ZipFile zip = new ZipFile(packagePath))
          {
            fileSystem.Directory.Ensure(path);
            const long minAmountRequiredBytesToUpdateProgress = 1 * MB;
            long lastEntriesTotalSizes = 0;
            foreach (ZipEntry entry in zip.Entries)
            {
              try
              {
                if (ignore1 != null && entry.FileName.ContainsIgnoreCase(ignore1))
                {
                  continue;
                }

                if (ignore2 != null && entry.FileName.ContainsIgnoreCase(ignore2))
                {
                  continue;
                }

                entry.Extract(path, ExtractExistingFileAction.OverwriteSilently);
                lastEntriesTotalSizes += entry.CompressedSize;
                if (lastEntriesTotalSizes > minAmountRequiredBytesToUpdateProgress)
                {
                  incrementProgress(lastEntriesTotalSizes);
                  lastEntriesTotalSizes = 0;
                }
              }
              catch (IOException)
              {
                bool b = false;
                foreach (string postFix in new[] { ".tmp", ".PendingOverwrite" })
                {
                  string errorPath = Path.Combine(path, entry.FileName) + postFix;
                  if (System.IO.File.Exists(errorPath))
                  {
                    System.IO.File.Delete(errorPath);
                    b = true;
                  }
                }

                if (!b)
                {
                  throw;
                }

                entry.Extract(path, ExtractExistingFileAction.OverwriteSilently);
              }
            }
          }
        }
        catch (ZipException)
        {
          throw new InvalidOperationException(string.Format("The \"{0}\" package seems to be corrupted.", packagePath));
        }
      }
    }
    /// <summary>
    /// The unpack zip.
    /// </summary>
    /// <param name="packagePath">
    /// The package Path. 
    /// </param>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="entriesPattern">
    /// The entries Pattern. 
    /// </param>
    /// <param name="stepsCount">
    /// The steps Count. 
    /// </param>
    /// <param name="incrementProgress">
    /// The increment Progress. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The \"{0}\" installation package seems to be corrupted.
    /// </exception>
    public virtual void UnpackZip([NotNull] string packagePath, [NotNull] string path,
      [CanBeNull] string entriesPattern = null, int stepsCount = 1,
      [CanBeNull] Action incrementProgress = null, bool skipErrors = false)
    {
      Assert.ArgumentNotNull(packagePath, "packagePath");
      Assert.ArgumentNotNull(path, "path");

      // TODO: comment this line when the progress bar is adjusted
      incrementProgress = null;

      bool hasResponse = incrementProgress != null;
      if (System.IO.File.Exists(packagePath))
      {
        try
        {
          Log.Info(
            "Unzipping {2}the '{0}' archive to the '{1}' folder".FormatWith(packagePath, path,
              entriesPattern != null
                ? "\"" + entriesPattern + "\"" +
                  " files of "
                : string.Empty), typeof(FileSystem));

          using (ZipFile zip = new ZipFile(packagePath))
          {
            int q = Math.Max(zip.Entries.Count / stepsCount, 1);

            fileSystem.Directory.Ensure(path);
            int i = 0;
            ICollection<ZipEntry> entries = entriesPattern != null ? zip.SelectEntries(entriesPattern) : zip.Entries;
            foreach (ZipEntry entry in entries)
            {
              try
              {
                entry.Extract(path, ExtractExistingFileAction.OverwriteSilently);
              }
              catch (IOException ex)
              {
                if (skipErrors)
                {
                  Log.Error("Unpacking caused exception", ex);
                  continue;
                }

                bool b = false;
                foreach (string postFix in new[] { ".tmp", ".PendingOverwrite" })
                {
                  string errorPath = Path.Combine(path, entry.FileName) + postFix;
                  if (System.IO.File.Exists(errorPath))
                  {
                    System.IO.File.Delete(errorPath);
                    b = true;
                  }
                }

                if (!b)
                {
                  throw;
                }

                entry.Extract(path, ExtractExistingFileAction.OverwriteSilently);
              }
              catch (Exception ex)
              {
                if (skipErrors)
                {
                  Log.Error("Unpacking caused exception", ex);
                  continue;
                }
              }

              if (hasResponse)
              {
                if (++i == q)
                {
                  incrementProgress();
                  i = 0;
                }
              }
            }
          }
        }
        catch (ZipException)
        {
          throw new InvalidOperationException(string.Format("The \"{0}\" package seems to be corrupted.", packagePath));
        }
      }
    }/// <summary>
    /// The check zip.
    /// </summary>
    /// <param name="packagePath">
    /// The package Path. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The \"{0}\" archive isn't a Sitecore installation package.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The \"{0}\" installation package seems to be corrupted.
    /// </exception>
    public virtual void CheckZip([NotNull] string packagePath)
    {
      Assert.ArgumentNotNullOrEmpty(packagePath, "packagePath");

      if (System.IO.File.Exists(packagePath))
      {
        try
        {
          using (ZipFile zip = new ZipFile(packagePath))
          {
            bool data = zip.Entries.FirstOrDefault(e => e.IsDirectory && e.FileName.Contains("/Data/")) != null;
            bool databases = zip.Entries.FirstOrDefault(e => e.IsDirectory && e.FileName.Contains("/Databases/")) !=
                             null;
            bool website = zip.Entries.FirstOrDefault(e => e.IsDirectory && e.FileName.Contains("/Website/")) != null;
            if (!(data && databases && website))
            {
              throw new InvalidOperationException(
                string.Format("The \"{0}\" archive isn't a Sitecore installation package.", packagePath));
            }
          }
        }
        catch (ZipException)
        {
          throw new InvalidOperationException(string.Format("The \"{0}\" installation package seems to be corrupted.",
            packagePath));
        }
      }
    }

    public virtual string GetFirstRootFolder(string packagePath)
    {
      if (System.IO.File.Exists(packagePath))
      {
        try
        {
          using (ZipFile zip = new ZipFile(packagePath))
          {
            var first = zip.EntryFileNames.First();
            return first.Substring(0, first.IndexOf('/'));
          }
        }
        catch (ZipException)
        {
          throw new InvalidOperationException(string.Format("The \"{0}\" installation package seems to be corrupted.",
            packagePath));
        }
      }

      return null;
    }
  }
}