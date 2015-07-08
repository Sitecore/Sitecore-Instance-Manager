using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace SIM.Base
{
  public class FileProvider
  {
    private readonly FileSystem fileSystem;

    public FileProvider(FileSystem fileSystem)
    {
      this.fileSystem = fileSystem;
    }

    public virtual void Copy(string path1, string path2, bool overwrite)
    {
      Assert.IsTrue(!path1.EqualsIgnoreCase(path2), "Source and destination are same: " + path1);

      File.Copy(path1, path2, overwrite);
    }

    public virtual void Copy(string path, string destFileName)
    {
      Copy(path, destFileName, true);
    }

    public virtual DateTime GetLastWriteTimeUtc(string path)
    {
      return File.GetLastWriteTimeUtc(path);
    }

    public virtual DateTime GetCreationTimeUtc(string path)
    {
      return File.GetCreationTimeUtc(path);
    }

    public virtual string[] GetNeighbourFiles(string filePath, string filter)
    {
      return Directory.GetFiles(Path.GetDirectoryName(filePath), filter);
    }

    public virtual long GetFileLength(string destFileName)
    {
      AssertExists(destFileName);
      return new FileInfo(destFileName).Length;
    }

    public virtual void Copy(string source, string target, bool sync = false, int timeout = 1000)
    {
      Log.Info("Copying the {0} file to {1}".FormatWith(source, target), typeof(FileSystem));
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

        AssertExists(target, "The attempt to copy the '{0}' file to '{1}' location was performed, but even after {3}ms timeout the target file didn't appear".FormatWith(source, target, timeout));
      }
    }

    public virtual FileStream OpenRead(string filePath)
    {
      return File.OpenRead(filePath);
    }
    
    public virtual bool Exists(string path)
    {
      return File.Exists(path);
    }

    public virtual string[] ReadAllLines(string path)
    {
      return File.ReadAllLines(path);
    }

    public virtual void AppendAllText(string path, string text)
    {
      File.AppendAllText(path, text);
    }

    public virtual void AppendAllLines(string path, string[] lines)
    {
      File.AppendAllLines(path, lines);
    }

    public virtual void Delete(string path)
    {
      File.Delete(path);
    }

    public virtual string ReadAllText(string path)
    {
      return File.ReadAllText(path);
    }

    public virtual void WriteAllText(string path, string text)
    {
      File.WriteAllText(path, text);
    }

    public virtual void Move(string path, string destFileName, bool replace = false)
    {
      if(replace && Exists(destFileName))
      {
        var deletePath = destFileName + ".delete";
        Move(destFileName, deletePath);
        Delete(path);
      }

      File.Move(path, destFileName);
    }

    /// <summary>
    /// The delete if exists.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    public virtual void DeleteIfExists([CanBeNull] string path, string ignore = null)
    {
      if (!string.IsNullOrEmpty(path) && (Directory.Exists(path) || File.Exists(path)))
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
            string directoryName = new DirectoryInfo(directory).Name;
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

    /// <summary>
    /// The assert file exists.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <param name="isError">If does not exist would it be an error?</param>
    public virtual void AssertExists([NotNull] string path, [CanBeNull] string message = null, bool isError = true)
    {
      Assert.ArgumentNotNullOrEmpty(path, "path");
      if (string.IsNullOrEmpty(message))
      {
        message = "The '" + path + "' file doesn't exists";
      }

      Assert.IsTrue(File.Exists(path), message, isError);
    }
  }
}