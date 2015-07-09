namespace SIM.Products
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  public static class ManifestHelper
  {
    #region Constants

    private const string ManifestExtension = ".manifest.xml";

    #endregion

    #region Fields

    public static readonly LookupFolder[] DefaultManifestsLocations =
    {
      new LookupFolder("Manifests", true), new LookupFolder("Plugins", true), new LookupFolder(ApplicationManager.UserManifestsFolder, true), new LookupFolder(ApplicationManager.PluginsFolder, true)
    };

    public static LookupFolder[] CustomManifestsLocations = null;

    #endregion

    #region Public properties

    public static bool UpdateNeeded { get; private set; }

    #endregion

    #region Public methods

    public static void CheckUpdateNeeded()
    {
      using (new ProfileSection("Checking if manifests update needed", typeof(ManifestHelper)))
      {
        try
        {
          if (!ProductHelper.Settings.CoreManifestsUpdateEnabled.Value)
          {
            UpdateNeeded = false;
            return;
          }

          foreach (var name in new[]
          {
            "newmanifests.zip", "newmanifests.zip.tmp"
          })
          {
            string filePath = Path.Combine(Environment.CurrentDirectory, name);
            if (FileSystem.FileSystem.Local.File.Exists(filePath))
            {
              Log.Warn("The " + name + " exists however it must not to be", typeof(ManifestHelper));
              try
              {
                FileSystem.FileSystem.Local.File.Delete(filePath);
                Log.Info("The " + name + " has been deleted", typeof(ManifestHelper));
              }
              catch (Exception ex)
              {
                Log.Warn("Cannot delete " + name + string.Empty, typeof(ManifestHelper), ex);
                ProfileSection.Result(false);
                return;
              }
            }
          }

          string manifestUrl = ProductHelper.Settings.CoreManifestsUpdateDatabaseUrl.Value;
          string localManifestsPath = Path.Combine(Environment.CurrentDirectory, "manifests.zip");
          if (!FileSystem.FileSystem.Local.File.Exists(localManifestsPath))
          {
            UpdateNeeded = true;
            ProfileSection.Result(UpdateNeeded);
            return;
          }

          var localFileSize = FileSystem.FileSystem.Local.File.GetFileLength(localManifestsPath);
          var remoteFileSize = WebRequestHelper.GetFileSize(new Uri(manifestUrl));

          UpdateNeeded = localFileSize != remoteFileSize;
          Log.Debug("Local manifests.zip file size is " + localFileSize);
          Log.Debug("Remote manifests.zip file size is " + remoteFileSize);
          ProfileSection.Result(UpdateNeeded);
        }
        catch (Exception ex)
        {
          Log.Error("Error while updating manifests", typeof(ManifestHelper), ex);
          ProfileSection.Result(false);
        }
      }
    }

    public static XmlDocumentEx Compute(string packageFile, string originalName = null)
    {
      using (new ProfileSection("Compute manifest", typeof(ManifestHelper)))
      {
        ProfileSection.Argument("packageFile", packageFile);
        ProfileSection.Argument("originalName", originalName);

        try
        {
          var manifestLookupFolders = GetManifestLookupFolders(packageFile);
          var fileNamePatterns = GetFileNamePatterns(packageFile, originalName);
          var result = Compute(packageFile, manifestLookupFolders, fileNamePatterns);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          Log.Error("Failed to build a manifest for " + packageFile, typeof(ManifestHelper), ex);

          return Product.EmptyManifest;
        }
      }
    }

    public static List<string> GetFileNamePatterns(string packageFile, string originalName = null)
    {
      Assert.IsNotNullOrEmpty(packageFile, "packageFile");

      using (new ProfileSection("Get file name patterns", typeof(ManifestHelper)))
      {
        ProfileSection.Argument("packageFile", packageFile);
        ProfileSection.Argument("originalName", originalName);

        var match = Product.ProductRegex.Match(packageFile);
        if (match.Success)
        {
          var groups = match.Groups;

          var name = groups[1].Value;
          var version = groups[2].Value;
          var revision = groups[5].Value;

          foreach (var word in new[]
          {
            "upgrade", "oracle", "(exe)"
          })
          {
            if (revision.ContainsIgnoreCase(word))
            {
              return ProfileSection.Result(new List<string>());
            }
          }

          var result = new[]
          {
            name, 
            name + " " + version.SubstringEx(0, 1), 
            name + " " + version.SubstringEx(0, 3), 
            name + " " + version.SubstringEx(0, 5), 
            name + " " + version + " rev. " + revision
          }
            .Distinct()
            .ToList();

          return ProfileSection.Result(result);
        }

        return new List<string>(new[]
        {
          Path.GetFileNameWithoutExtension(packageFile)
        });
      }
    }

    public static void UpdateManifestsSync()
    {
      using (new ProfileSection("Updating manifests", typeof(ManifestHelper)))
      {
        if (!ManifestHelper.UpdateNeeded)
        {
          ProfileSection.Result("Not needed");
          return;
        }

        var manifestUrl = ProductHelper.Settings.CoreManifestsUpdateDatabaseUrl.Value;
        var newLocalManifests = Path.Combine(Environment.CurrentDirectory, "newmanifests.zip");

        try
        {
          try
          {
            var newLocalManifestsTmp = Path.Combine(Environment.CurrentDirectory, "newmanifests.zip.tmp");
            WebRequestHelper.DownloadFile(new Uri(manifestUrl), newLocalManifestsTmp);

            try
            {
              FileSystem.FileSystem.Local.File.Move(newLocalManifestsTmp, newLocalManifests, true);
            }
            catch (Exception ex)
            {
              Log.Error("Cannot rename newmanifests.zip.tmp into newmanifests.zip", typeof(ManifestHelper), ex);
              ProfileSection.Result("Terminated");
              return;
            }
          }
          catch (Exception e)
          {
            Log.Error("Could not download manifests.zip", typeof(ManifestHelper), e);
            ProfileSection.Result("Terminated");
            return;
          }

          FileSystem.FileSystem.Local.Directory.DeleteIfExists("Manifests");
          FileSystem.FileSystem.Local.Zip.UnpackZip(newLocalManifests, Environment.CurrentDirectory, null, 1, null, true);

          var localManifestsPath = Path.Combine(Environment.CurrentDirectory, "manifests.zip");
          FileSystem.FileSystem.Local.File.Move(newLocalManifests, localManifestsPath, true);

          CacheManager.ClearAll();
          ProfileSection.Result("Done");
        }
        catch (Exception ex)
        {
          Log.Error("Error while unpacking newmanifests.zip", typeof(ManifestHelper), ex);
          ProfileSection.Result("Terminated");
        }
      }
    }

    #endregion

    #region Private methods

    private static XmlDocumentEx Compute(string packageFile, LookupFolder[] manifestLookupFolders, List<string> fileNamePatterns)
    {
      using (new ProfileSection("Compute manifest", typeof(ManifestHelper)))
      {
        ProfileSection.Argument("packageFile", packageFile);
        ProfileSection.Argument("manifestLookupFolders", manifestLookupFolders);
        ProfileSection.Argument("fileNamePatterns", fileNamePatterns);

        var list = new List<string>();
        XmlDocumentEx mainDocument = null;

        try
        {
          foreach (var lookupFolder in manifestLookupFolders)
          {
            var folderPath = lookupFolder.Path;
            if (!FileSystem.FileSystem.Local.Directory.Exists(folderPath))
            {
              Log.Warn("The {0} manifest lookup folder doesn't exist".FormatWith(lookupFolder), typeof(ManifestHelper));
              continue;
            }

            using (new ProfileSection("Looking for manifests in folder", typeof(ManifestHelper)))
            {
              ProfileSection.Argument("lookupFolder", lookupFolder);

              foreach (string fileNamePattern in fileNamePatterns)
              {
                string fileName = fileNamePattern + ManifestExtension;
                using (new ProfileSection("Looking for manifests with pattern", typeof(ManifestHelper)))
                {
                  ProfileSection.Argument("fileName", fileName);

                  try
                  {
                    string[] findings = FileSystem.FileSystem.Local.Directory.GetFiles(folderPath, fileName, lookupFolder.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    Log.Debug("Found " + findings.Length + " matches");
                    if (findings.Length == 1)
                    {
                      string path = findings.First();
                      Log.Debug("Found '" + path + "'");
                      try
                      {
                        if (mainDocument == null)
                        {
                          Log.Debug("Loading file 'as main document'");
                          mainDocument = XmlDocumentEx.LoadFile(path);

                          ProfileSection.Result("Loaded");
                        }
                        else
                        {
                          Log.Debug("Loading file 'for merge'");
                          mainDocument.Merge(XmlDocumentEx.LoadFile(path));

                          ProfileSection.Result("Merged");
                        }

                        list.Add(path);
                      }
                      catch (Exception ex)
                      {
                        HandleError(path, list, ex);
                      }
                    }
                    else if (findings.Length > 1)
                    {
                      var findingsText = findings.Join(Environment.NewLine);
                      var message = "There are several {0} files in the {1} folder: {2}".FormatWith(fileName, lookupFolder, findingsText);
                      Log.Warn(message, typeof(ManifestHelper));

                      ProfileSection.Result("Skipped (Too many files found)");
                    }
                    else
                    {
                      ProfileSection.Result("Skipped (No files found)");
                    }
                  }
                  catch (Exception ex)
                  {
                    Log.Warn("Failed looking for \"{0}\" manifests in \"{1}\"".FormatWith(fileNamePattern, folderPath), typeof(ManifestHelper), ex);
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Log.Error("Failed to find and merge manifests on file system", typeof(ManifestHelper), ex);
        }

        XmlDocumentEx archiveManifest = Product.ArchiveManifest;
        XmlDocumentEx packageManifest = Product.PackageManifest;
        if (mainDocument != null)
        {
          if (mainDocument.SelectSingleElement("/manifest[@version='1.3']/archive") != null)
          {
            CacheManager.SetEntry("IsPackage", packageFile, "false");
            try
            {
              Log.Debug("Merging with 'archiveManifest'");
              mainDocument.Merge(archiveManifest);
            }
            catch (Exception ex)
            {
              HandleError(archiveManifest.FilePath, list, ex);
            }
          }
          else if (mainDocument.SelectSingleElement("/manifest[@version='1.3']/package") != null)
          {
            CacheManager.SetEntry("IsPackage", packageFile, "true");
            try
            {
              Log.Debug("Merging with 'packageManifest'");
              mainDocument.Merge(packageManifest);
            }
            catch (Exception ex)
            {
              HandleError(packageManifest.FilePath, list, ex);
            }
          }

          return ProfileSection.Result(mainDocument);
        }

        if (FileSystem.FileSystem.Local.Zip.ZipContainsSingleFile(packageFile, "package.zip"))
        {
          Log.Info("The '{0}' file is considered as Sitecore Package, (type #1)".FormatWith(packageFile), typeof(Product));
          CacheManager.SetEntry("IsPackage", packageFile, "true");

          return ProfileSection.Result(packageManifest);
        }

        if (FileSystem.FileSystem.Local.Zip.ZipContainsFile(packageFile, "metadata/sc_name.txt") &&
            FileSystem.FileSystem.Local.Zip.ZipContainsFile(packageFile, "installer/version"))
        {
          Log.Info("The '{0}' file is considered as Sitecore Package, (type #2)".FormatWith(packageFile), typeof(Product));
          CacheManager.SetEntry("IsPackage", packageFile, "true");

          return ProfileSection.Result(packageManifest);
        }

        CacheManager.SetEntry("IsPackage", packageFile, "false");

        return ProfileSection.Result(archiveManifest);
      }
    }

    private static List<string> GetFileNamePatterns(IEnumerable<string> fileNamesRaw)
    {
      using (new ProfileSection("Get manifest lookup folders", typeof(ManifestHelper)))
      {
        ProfileSection.Argument("fileNamesRaw", fileNamesRaw);

        var fileNamePatterns = new List<string>();
        foreach (string filename in fileNamesRaw)
        {
          if (string.IsNullOrEmpty(filename) || fileNamePatterns.Contains(filename))
          {
            continue;
          }

          fileNamePatterns.Add(filename);

          // if it is CMS
          if (filename.StartsWith("sitecore 6") || filename.StartsWith("sitecore 7"))
          {
            continue;
          }

          string cut = filename.TrimStart("sitecore ").Trim();
          if (!string.IsNullOrEmpty(cut) && !fileNamePatterns.Contains(cut))
          {
            fileNamePatterns.Add(cut);
          }
        }

        return fileNamePatterns;
      }
    }

    private static LookupFolder[] GetManifestLookupFolders(string packageFile)
    {
      using (new ProfileSection("Get manifest lookup folders", typeof(ManifestHelper)))
      {
        ProfileSection.Argument("packageFile", packageFile);

        var packageFolder = new FileInfo(packageFile).Directory.IsNotNull("This is impossible").FullName;
        Assert.IsNotNull(packageFolder.EmptyToNull(), "folder");
        var manifestLookupFolders = (CustomManifestsLocations ?? DefaultManifestsLocations).Add(new LookupFolder(packageFolder, false));

        return ProfileSection.Result(manifestLookupFolders.ToArray());
      }
    }

    private static void HandleError(string path, IEnumerable<string> list, Exception ex)
    {
      string str = list.Join(", ", "'", "'");
      Log.Warn("Failed merging '{0}' with successfully merged {1}. {2}".FormatWith((object)path, (object)str, (object)ex.Message), typeof(ManifestHelper), ex);
    }

    private static string TrimRevision(string fileName)
    {
      int revIndex = fileName.IndexOf(" rev.", StringComparison.Ordinal);
      if (revIndex >= 0)
      {
        return fileName.Substring(0, revIndex);
      }

      return fileName;
    }

    #endregion

    #region Nested type: LookupFolder

    public class LookupFolder
    {
      #region Fields

      [NotNull]
      public readonly string Path;

      public readonly bool Recursive;

      #endregion

      #region Constructors

      public LookupFolder([NotNull] string path, bool recursive)
      {
        Assert.ArgumentNotNull(path, "path");

        this.Path = path;
        this.Recursive = recursive;
      }

      #endregion

      #region Public methods

      public override string ToString()
      {
        return "Path: {0}, Recursive: {1}".FormatWith(this.Path, this.Recursive);
      }

      #endregion
    }

    #endregion
  }
}