using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Sitecore.Diagnostics.Base;
using JetBrains.Annotations;

namespace SIM.FileSystem
{
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  public class DirectoryProvider
  {
    #region Fields

    private FileSystem FileSystem { get; }

    #endregion

    #region Constructors

    public DirectoryProvider(FileSystem fileSystem)
    {
      FileSystem = fileSystem;
    }

    #endregion

    #region Public methods

    public virtual void AssertExists([NotNull] string path, [CanBeNull] string message = null)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      Assert.IsTrue(Directory.Exists(path), message.EmptyToNull() ?? $"The {Environment.ExpandEnvironmentVariables(path)} folder does not exist, but is expected to be");
    }

    public virtual string Combine(DirectoryInfo one, string two)
    {
      return Path.Combine(one.FullName, two);
    }

    public virtual void Copy(string path, string newPath, bool recursive)
    {
      // Get the subdirectories for the specified directory.
      DirectoryInfo dir = new DirectoryInfo(path);
      DirectoryInfo[] dirs = dir.GetDirectories();

      if (!dir.Exists)
      {
        throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + path);
      }

      // If the destination directory doesn't exist, create it. 
      if (!Exists(newPath))
      {
        CreateDirectory(newPath);
      }

      // Get the files in the directory and copy them to the new location.
      foreach (var file in FileSystem.Directory.GetFiles(path))
      {
        var temppath = Path.Combine(newPath, Path.GetFileName(file));
        FileSystem.File.Copy(file, temppath);
      }

      // If copying subdirectories, copy them and their contents to new location. 
      if (recursive)
      {
        foreach (DirectoryInfo subdir in dirs)
        {
          var temppath = Path.Combine(newPath, subdir.Name);
          Copy(subdir.FullName, temppath, recursive);
        }
      }
    }

    public virtual DirectoryInfo CreateDirectory(string path)
    {
      return Directory.CreateDirectory(path);
    }

    public virtual void Delete([NotNull] string path)
    {
      // TODO: Refactor this to edit attributes only on problem files and folders
      Assert.ArgumentNotNull(path, nameof(path));
      Log.Info($"Deleting file or folder: {path}");

      if (!string.IsNullOrEmpty(path))
      {
        if (Directory.Exists(path))
        {
          var directoryInfo = new DirectoryInfo(path)
          {
            Attributes = FileAttributes.Normal
          };
          try
          {
            directoryInfo.Delete(true);

            // Hook to make async delete operation synchronus
            for (int i = 0; Directory.Exists(path) && i < 10; ++i)
            {
              Thread.Sleep(100);
            }
          }
          catch (Exception ex)
          {
            Log.Warn(ex, $"Failed to delete {path} folder, altering attributes and trying again");
            Thread.Sleep(100);
            foreach (var fileSystemInfo in directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
              fileSystemInfo.Attributes = FileAttributes.Normal;
            }

            directoryInfo.Delete(true);
          }
        }
      }
    }

    public virtual void DeleteContents(string path)
    {
      foreach (var file in GetFiles(path))
      {
        FileSystem.File.Delete(file);
      }

      foreach (var directory in GetDirectories(path))
      {
        Delete(directory);
      }
    }

    public virtual void DeleteIfExists([CanBeNull] string path, string ignore = null)
    {
      if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
      {
        if (ignore == null)
        {
          Delete(path);
        }
        else
        {
          Assert.IsTrue(!ignore.Contains('\\') && !ignore.Contains('/'), "Multi-level ignore is not supported for deleting");
          foreach (var directory in Directory.GetDirectories(path))
          {
            var directoryName = new DirectoryInfo(directory).Name;
            if (!directoryName.EqualsIgnoreCase(ignore))
            {
              Delete(directory);
            }
          }

          foreach (var file in Directory.GetFiles(path))
          {
            Delete(file);
          }
        }
      }
    }

    public void DeleteTempFolders()
    {
      using (new ProfileSection("Delete temp folders", this))
      {
        var tempFoldersCacheFilePath = Path.Combine(ApplicationManager.TempFolder, "tempFolders.txt");
        if (!System.IO.File.Exists(tempFoldersCacheFilePath))
        {
          ProfileSection.Result("Skipped");
          return;
        }

        var paths = System.IO.File.ReadAllLines(tempFoldersCacheFilePath);
        foreach (var path in paths)
        {
          DeleteIfExists(path);
        }

        System.IO.File.Delete(tempFoldersCacheFilePath);
        ProfileSection.Result("Done");
      }
    }

    [NotNull]
    public virtual string Ensure([NotNull] string folder)
    {
      Assert.ArgumentNotNullOrEmpty(folder, nameof(folder));

      if (!Directory.Exists(folder))
      {
        Log.Info($"Creating folder {folder}");
        Directory.CreateDirectory(folder);
      }

      return folder;
    }

    public virtual bool Exists(string path)
    {
      return Directory.Exists(path);
    }

    [NotNull]
    public virtual string FindCommonParent([NotNull] string path1, [NotNull] string path2)
    {
      Assert.ArgumentNotNull(path1, nameof(path1));
      Assert.ArgumentNotNull(path2, nameof(path2));
      var path = string.Empty;
      using (new ProfileSection("Find common parent", this))
      {
        ProfileSection.Argument("path1", path1);
        ProfileSection.Argument("path2", path2);

        string[] arr1 = path1.Replace('/', '\\').Split('\\');
        string[] arr2 = path2.Replace('/', '\\').Split('\\');
        var minLen = Math.Min(arr1.Length, arr2.Length);
        for (int i = 0; i < minLen; ++i)
        {
          var word1 = arr1[i];
          var word2 = arr2[i];
          if (!word1.EqualsIgnoreCase(word2))
          {
            break;
          }

          path += word1 + '\\';
        }

        path = path.TrimEnd('\\');
        ProfileSection.Result(path);
      }

      return path;
    }

    [NotNull]
    public virtual string GenerateTempFolderPath([NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));

      return Path.Combine(folder, Guid.NewGuid().ToString());
    }

    [CanBeNull]
    public virtual IEnumerable<string> GetAncestors([NotNull] string path)
    {
      Assert.ArgumentNotNull(path, nameof(path));

      DirectoryInfo dir = new DirectoryInfo(path);
      while (dir != null && dir.Exists)
      {
        path = dir.FullName;
        yield return path;
        dir = dir.Parent;
      }
    }

    public virtual string[] GetDirectories(string path)
    {
      return Directory.GetDirectories(path);
    }

    public virtual string[] GetDirectories(string path, string pattern)
    {
      return Directory.GetDirectories(path, pattern);
    }

    public virtual string GetDirectoryRoot(string path)
    {
      return Directory.GetDirectoryRoot(path);
    }

    public virtual int GetDistance(string directory1, string directory2)
    {
      Assert.ArgumentNotNullOrEmpty(directory1, nameof(directory1));
      Assert.ArgumentNotNullOrEmpty(directory2, nameof(directory2));

      using (new ProfileSection("Get distance", this))
      {
        ProfileSection.Argument("directory1", directory1);
        ProfileSection.Argument("directory2", directory2);

        directory1 = directory1.TrimEnd('\\');
        directory2 = directory2.TrimEnd('\\');

        var root1 = GetPathRoot(directory1);
        var root2 = GetPathRoot(directory2);
        Assert.IsTrue(HaveSameRoot(root1, root2), $"The '{directory1}' and '{directory2}' paths has different roots so unaplicable");

        var arr1 = directory1.Split('\\');
        var arr2 = directory2.Split('\\');
        var len = Math.Min(arr1.Length, arr2.Length) - 1;
        var common = 0;
        for (int i = 0; i < len && arr1[i].EqualsIgnoreCase(arr2[i]); ++i)
        {
          common++;
        }

        var distance = arr1.Length + arr2.Length - 2 * common - 2;

        return ProfileSection.Result(distance);
      }
    }

    public string[] GetFileSystemEntries(string path)
    {
      return Directory.GetFileSystemEntries(path);
    }

    public virtual string[] GetFiles(string path, string filter, SearchOption searchMode)
    {
      return Directory.GetFiles(path, filter, searchMode);
    }

    public virtual string[] GetFiles(string path, string filter)
    {
      return Directory.GetFiles(path, filter);
    }

    public virtual string[] GetFiles(string path)
    {
      return Directory.GetFiles(path);
    }

    public virtual DirectoryInfo GetParent(string path)
    {
      return Directory.GetParent(path);
    }

    public virtual string GetPathRoot(string path)
    {
      if (path.Length == 2 && path[1] == ':')
      {
        path += "\\";
      }

      return Path.GetPathRoot(path);
    }

    public TempFolder GetTempFolder(string path = null, bool? rooted = null)
    {
      return new TempFolder(FileSystem, path, rooted);
    }

    public string GetVirtualPath(string databaseFilePath)
    {
      return databaseFilePath.Substring("C:\\".Length);
    }

    public virtual bool HasDriveLetter([NotNull] string folder)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));

      return folder.Length >= 3 && char.IsLetter(folder[0]) && folder[1] == ':' && folder[2] == '\\';
    }

    public virtual bool HaveSameRoot(string directory1, string directory2)
    {
      using (new ProfileSection("Have same root", this))
      {
        ProfileSection.Argument("directory1", directory1);
        ProfileSection.Argument("directory2", directory2);

        var root1 = GetPathRoot(directory1);
        var root2 = GetPathRoot(directory2);
        bool haveSameRoot = root2.EqualsIgnoreCase(root1);

        return ProfileSection.Result(haveSameRoot);
      }
    }

    [NotNull]
    public virtual string MapPath([NotNull] string virtualPath, [NotNull] string rootPath)
    {
      Assert.ArgumentNotNull(virtualPath, nameof(virtualPath));
      Assert.ArgumentNotNullOrEmpty(rootPath, nameof(rootPath));
      if (HasDriveLetter(virtualPath))
      {
        return virtualPath;
      }

      return Path.Combine(rootPath, virtualPath.TrimStart("/", "~/"));
    }

    public virtual void Move(string path, string newPath)
    {
      Directory.Move(path, newPath);
    }

    public string RegisterTempFolder(string tempFolderPath)
    {
      var tempFoldersCacheFilePath = Path.Combine(ApplicationManager.TempFolder, "tempFolders.txt");
      System.IO.File.AppendAllLines(tempFoldersCacheFilePath, new[]
      {
        tempFolderPath
      });

      return tempFolderPath;
    }

    public virtual void TryDelete([NotNull] string path)
    {
      Assert.ArgumentNotNull(path, nameof(path));

      try
      {
        Delete(path);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Cannot delete the {path} file. {ex.Message}");
      }
    }

    #endregion

    #region Protected methods

    [NotNull]
    protected virtual DirectoryInfo GetChild([NotNull] DirectoryInfo extracted, [NotNull] string folderName)
    {
      Assert.ArgumentNotNull(extracted, nameof(extracted));
      Assert.ArgumentNotNullOrEmpty(folderName, nameof(folderName));

      DirectoryInfo[] websites = extracted.GetDirectories(folderName);
      Assert.IsTrue(websites != null && websites.Length > 0,
        $"Can't find extracted {folderName} folder here: {extracted.FullName}");

      return websites[0];
    }

    #endregion
  }
}