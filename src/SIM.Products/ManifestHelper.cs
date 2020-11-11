﻿using SIM.Products.ProductParsers;
using Sitecore.Diagnostics.InfoService.Client.Model;

namespace SIM.Products
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Ionic.Zip;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.IO.Real;

  public static class ManifestHelper
  {
    #region Constants

    private const string ManifestExtension = ".manifest.xml";

    #endregion

    #region Fields

    public static readonly LookupFolder[] DefaultManifestsLocations =
    {
      new LookupFolder("Manifests", true), new LookupFolder(ApplicationManager.UserManifestsFolder, true)
    };

    public static LookupFolder[] _CustomManifestsLocations = null;

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
          Log.Error(ex, string.Format("Failed to build a manifest for " + packageFile));

          return Product.EmptyManifest;
        }
      }
    }

    public static List<string> GetFileNamePatterns(string packageFile, string originalName = null)
    {
      Assert.IsNotNullOrEmpty(packageFile, nameof(packageFile));

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

        // Consider fallback product parsers
        IProductParser[] parsers = ProductManager.ProductParsers;

        foreach (var parser in parsers)
        {
          if (parser.TryParseName(packageFile, out originalName))
          {
            if (!string.IsNullOrEmpty(originalName))
            {
              var result = new List<string>()
              {
                originalName
              };

              return result;
            }
          }
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
              Log.Warn($"The {lookupFolder} manifest lookup folder doesn't exist");
              continue;
            }

            using (new ProfileSection("Looking for manifests in folder"))
            {
              ProfileSection.Argument("lookupFolder", lookupFolder);

              foreach (string fileNamePattern in fileNamePatterns)
              {
                var fileName = fileNamePattern + ManifestExtension;
                using (new ProfileSection("Looking for manifests with pattern"))
                {
                  ProfileSection.Argument("fileName", fileName);

                  try
                  {
                    string[] findings = FileSystem.FileSystem.Local.Directory.GetFiles(folderPath, fileName, lookupFolder.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    Log.Debug($"Found {findings.Length} matches");
                    if (findings.Length == 1)
                    {
                      var path = findings.First();
                      Log.Debug($"Found '{path}'");
                      try
                      {
                        if (mainDocument == null)
                        {
                          Log.Debug("Loading file \'as main document\'");
                          mainDocument = XmlDocumentEx.LoadFile(path);

                          ProfileSection.Result("Loaded");
                        }
                        else
                        {
                          Log.Debug("Loading file \'for merge\'");
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
                      var findingsText = string.Join(Environment.NewLine, findings);
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
                    Log.Warn(ex, $"Failed looking for \"{fileNamePattern}\" manifests in \"{folderPath}\"");
                  }
                }
              }
            }
          }
          if (mainDocument == null)
          {
            using (new ProfileSection("Looking for manifests in package"))
            {
              using (var zip = ZipFile.Read(packageFile))
              {
                foreach (var filenamePattern in fileNamePatterns)
                {
                  var entry = zip[filenamePattern + ManifestExtension];
                  if (entry != null)
                  {
                    Log.Debug("Loading file \'as main document\'");

                    using (var stream = new MemoryStream())
                    {
                      entry.Extract(stream);
                      stream.Seek(0, SeekOrigin.Begin);
                      mainDocument = XmlDocumentEx.LoadStream(stream);
                    }

                    ProfileSection.Result("Loaded");
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
              Log.Debug("Merging with \'archiveManifest\'");
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
              Log.Debug("Merging with \'packageManifest\'");
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
          Log.Info($"The '{packageFile}' file is considered as Sitecore Package, (type #1)");
          CacheManager.SetEntry("IsPackage", packageFile, "true");

          return ProfileSection.Result(packageManifest);
        }

        using (var zip = new RealZipFile(new RealFileSystem().ParseFile(packageFile)))
          if (zip.Entries.Contains("metadata/sc_name.txt") &&
              zip.Entries.Contains("installer/version"))
          {
            Log.Info($"The '{packageFile}' file is considered as Sitecore Package, (type #2)");
            CacheManager.SetEntry("IsPackage", packageFile, "true");

            return ProfileSection.Result(packageManifest);
          }

        CacheManager.SetEntry("IsPackage", packageFile, "false");

        return ProfileSection.Result(archiveManifest);
      }
    }

    private static LookupFolder[] GetManifestLookupFolders(string packageFile)
    {
      using (new ProfileSection("Get manifest lookup folders"))
      {
        ProfileSection.Argument("packageFile", packageFile);

        var packageFolder = new FileInfo(packageFile).Directory.IsNotNull("This is impossible").FullName;
        Assert.IsNotNull(packageFolder.EmptyToNull(), "folder");
        var manifestLookupFolders = (_CustomManifestsLocations ?? DefaultManifestsLocations).Add(new LookupFolder(packageFolder, false));

        return ProfileSection.Result(manifestLookupFolders.ToArray());
      }
    }

    private static void HandleError(Exception ex, string path, IEnumerable<string> list)
    {
      var str = list.Join(", ", "'", "'");
      Log.Warn(ex, $"Failed merging '{(object)path}' with successfully merged {(object)str}. {(object)ex.Message}");
    }

    #endregion

    #region Nested type: LookupFolder

    public class LookupFolder
    {
      #region Fields

      [NotNull]
      public string Path { get; }

      public bool Recursive { get; }

      #endregion

      #region Constructors

      public LookupFolder([NotNull] string path, bool recursive)
      {
        Assert.ArgumentNotNull(path, nameof(path));

        Path = path;
        Recursive = recursive;
      }

      #endregion

      #region Public methods

      public override string ToString()
      {
        return "Path: {0}, Recursive: {1}".FormatWith(Path, Recursive);
      }

      #endregion
    }

    #endregion
  }
}