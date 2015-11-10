using System;
using System.IO;
using System.Linq;
using System.Threading;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Base.Annotations;

namespace SIM.FileSystem
{
  using Sitecore.Diagnostics.Logging;

  public class FileProvider
  {
    #region Fields

    private readonly FileSystem fileSystem;

    #endregion

    #region Constructors

    public FileProvider(FileSystem fileSystem)
    {
      this.fileSystem = fileSystem;
    }

    #endregion

    #region Public methods

    public virtual void AppendAllLines(string path, string[] lines)
    {
      File.AppendAllLines(path, lines);
    }

    public virtual void AppendAllText(string path, string text)
    {
      File.AppendAllText(path, text);
    }

    public virtual void AssertExists([NotNull] string path, [CanBeNull] string message = null, bool isError = true)
    {
      Assert.ArgumentNotNullOrEmpty(path, "path");
      if (string.IsNullOrEmpty(message))
      {
        message = "The '" + path + "' file doesn't exists";
      }

      Assert.IsTrue(File.Exists(path), message, isError);
    }

    public virtual void Copy(string path1, string path2, bool overwrite)
    {
      Assert.IsTrue(!path1.EqualsIgnoreCase(path2), "Source and destination are same: {0}", path1);

      File.Copy(path1, path2, overwrite);
    }

    public virtual void Copy(string path, string destFileName)
    {
      this.Copy(path, destFileName, true);
    }

    public virtual void Copy(string source, string target, bool sync = false, int timeout = 1000)
    {
      Log.Info("Copying the {0} file to {1}", source, target);
      if (File.Exists(target))
      {
        File.Delete(target);
      }

      File.Copy(source, target);
      if (sync && !File.Exists(target))
      {
        int sleep = 100;
        var times = timeout / sleep;
        for (int i = 0; i < times && !File.Exists(target); ++i)
        {
          Thread.Sleep(sleep);
        }

        this.AssertExists(target, "The attempt to copy the '{0}' file to '{1}' location was performed, but even after {3}ms timeout the target file didn't appear".FormatWith(source, target, timeout));
      }
    }

    public virtual void Delete(string path)
    {
      File.Delete(path);
    }

    public virtual void DeleteIfExists([CanBeNull] string path, string ignore = null)
    {
      if (!string.IsNullOrEmpty(path) && (Directory.Exists(path) || File.Exists(path)))
      {
        if (ignore == null)
        {
          this.Delete(path);
        }
        else
        {
          Assert.IsTrue(!ignore.Contains('\\') && !ignore.Contains('/'), "Multi-level ignore is not supported for deleting");
          foreach (var directory in Directory.GetDirectories(path))
          {
            string directoryName = new DirectoryInfo(directory).Name;
            if (!directoryName.EqualsIgnoreCase(ignore))
            {
              this.Delete(directory);
            }
          }

          foreach (var file in Directory.GetFiles(path))
          {
            this.Delete(file);
          }
        }
      }
    }

    public virtual bool Exists(string path)
    {
      return File.Exists(path);
    }

    public virtual DateTime GetCreationTimeUtc(string path)
    {
      return File.GetCreationTimeUtc(path);
    }

    public virtual long GetFileLength(string destFileName)
    {
      this.AssertExists(destFileName);
      return new FileInfo(destFileName).Length;
    }

    public virtual DateTime GetLastWriteTimeUtc(string path)
    {
      return File.GetLastWriteTimeUtc(path);
    }

    public virtual string[] GetNeighbourFiles(string filePath, string filter)
    {
      return Directory.GetFiles(Path.GetDirectoryName(filePath), filter);
    }

    public virtual void Move(string path, string destFileName, bool replace = false)
    {
      if (replace && this.Exists(destFileName))
      {
        var deletePath = destFileName + ".delete";
        this.Move(destFileName, deletePath);
        this.Delete(path);
      }

      File.Move(path, destFileName);
    }

    public virtual FileStream OpenRead(string filePath)
    {
      return File.OpenRead(filePath);
    }

    public virtual string[] ReadAllLines(string path)
    {
      return File.ReadAllLines(path);
    }

    public virtual string ReadAllText(string path)
    {
      return File.ReadAllText(path);
    }

    public virtual void WriteAllText(string path, string text)
    {
      File.WriteAllText(path, text);
    }

    #endregion
  }
}