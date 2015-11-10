namespace SIM.Products
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

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

    public static XmlDocumentEx Compute(string packageFile, string originalName = null)
    {
      using (new ProfileSection("Compute manifest"))
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
          Log.Error(ex, "Failed to build a manifest for " + packageFile);

          return Product.EmptyManifest;
        }
      }
    }

    public static List<string> GetFileNamePatterns(string packageFile, string originalName = null)
    {
      Assert.IsNotNullOrEmpty(packageFile, "packageFile");

      using (new ProfileSection("Get file name patterns"))
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

    #endregion

    #region Private methods

    private static XmlDocumentEx Compute(string packageFile, LookupFolder[] manifestLookupFolders, List<string> fileNamePatterns)
    {
      using (new ProfileSection("Compute manifest"))
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
              Log.Warn("The {0} manifest lookup folder doesn't exist", lookupFolder);
              continue;
            }

            using (new ProfileSection("Looking for manifests in folder"))
            {
              ProfileSection.Argument("lookupFolder", lookupFolder);

              foreach (string fileNamePattern in fileNamePatterns)
              {
                string fileName = fileNamePattern + ManifestExtension;
                using (new ProfileSection("Looking for manifests with pattern"))
                {
                  ProfileSection.Argument("fileName", fileName);

                  try
                  {
                    string[] findings = FileSystem.FileSystem.Local.Directory.GetFiles(folderPath, fileName, lookupFolder.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    Log.Debug("Found {0} matches", findings.Length);
                    if (findings.Length == 1)
                    {
                      string path = findings.First();
                      Log.Debug("Found '{0}'", path);
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
                        HandleError(ex, path, list);
                      }
                    }
                    else if (findings.Length > 1)
                    {
                      var findingsText = findings.Join(Environment.NewLine);
                      var message = "There are several {0} files in the {1} folder: {2}".FormatWith(fileName, lookupFolder, findingsText);
                      Log.Warn(message);

                      ProfileSection.Result("Skipped (Too many files found)");
                    }
                    else
                    {
                      ProfileSection.Result("Skipped (No files found)");
                    }
                  }
                  catch (Exception ex)
                  {
                    Log.Warn(ex, "Failed looking for \"{0}\" manifests in \"{1}\"", fileNamePattern, folderPath);
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Failed to find and merge manifests on file system");
        }

        XmlDocumentEx archiveManifest = Product.ArchiveManifest;
        XmlDocumentEx packageManifest = Product.PackageManifest;
        if (mainDocument != null)
        {
          if (mainDocument.SelectSingleElement("/manifest/archive") != null)
          {
            CacheManager.SetEntry("IsPackage", packageFile, "false");
            try
            {
              Log.Debug("Merging with 'archiveManifest'");
              mainDocument.Merge(archiveManifest);
            }
            catch (Exception ex)
            {
              HandleError(ex, archiveManifest.FilePath, list);
            }
          }
          else if (mainDocument.SelectSingleElement("/manifest/package") != null)
          {
            CacheManager.SetEntry("IsPackage", packageFile, "true");
            try
            {
              Log.Debug("Merging with 'packageManifest'");
              mainDocument.Merge(packageManifest);
            }
            catch (Exception ex)
            {
              HandleError(ex, packageManifest.FilePath, list);
            }
          }

          return ProfileSection.Result(mainDocument);
        }

        if (FileSystem.FileSystem.Local.Zip.ZipContainsSingleFile(packageFile, "package.zip"))
        {
          Log.Info("The '{0}' file is considered as Sitecore Package, (type #1)", packageFile);
          CacheManager.SetEntry("IsPackage", packageFile, "true");

          return ProfileSection.Result(packageManifest);
        }

        if (FileSystem.FileSystem.Local.Zip.ZipContainsFile(packageFile, "metadata/sc_name.txt") &&
            FileSystem.FileSystem.Local.Zip.ZipContainsFile(packageFile, "installer/version"))
        {
          Log.Info("The '{0}' file is considered as Sitecore Package, (type #2)", packageFile);
          CacheManager.SetEntry("IsPackage", packageFile, "true");

          return ProfileSection.Result(packageManifest);
        }

        CacheManager.SetEntry("IsPackage", packageFile, "false");

        return ProfileSection.Result(archiveManifest);
      }
    }

    private static List<string> GetFileNamePatterns(IEnumerable<string> fileNamesRaw)
    {
      using (new ProfileSection("Get manifest lookup folders"))
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
      using (new ProfileSection("Get manifest lookup folders"))
      {
        ProfileSection.Argument("packageFile", packageFile);

        var packageFolder = new FileInfo(packageFile).Directory.IsNotNull("This is impossible").FullName;
        Assert.IsNotNull(packageFolder.EmptyToNull(), "folder");
        var manifestLookupFolders = (CustomManifestsLocations ?? DefaultManifestsLocations).Add(new LookupFolder(packageFolder, false));

        return ProfileSection.Result(manifestLookupFolders.ToArray());
      }
    }

    private static void HandleError(Exception ex, string path, IEnumerable<string> list)
    {
      string str = list.Join(", ", "'", "'");
      Log.Warn(ex, "Failed merging '{0}' with successfully merged {1}. {2}", (object)path, (object)str, (object)ex.Message);
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