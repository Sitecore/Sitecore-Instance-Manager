using System;
using System.IO;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Base.Annotations;

namespace SIM.FileSystem
{
  public class UncFileProvider : FileProvider
  {
    #region Fields

    [NotNull]
    private readonly FileSystem fileSystem;

    #endregion

    #region Constructors

    public UncFileProvider([NotNull] FileSystem fileSystem)
      : base(fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, "fileSystem");

      this.fileSystem = fileSystem;
    }

    #endregion

    #region Public methods

    public override void AppendAllLines(string path, string[] lines)
    {
      try
      {
        base.AppendAllLines(path, lines);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.AppendAllLines(path, lines);
      }
    }

    public override void AppendAllText(string path, string text)
    {
      try
      {
        base.AppendAllText(path, text);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.AppendAllText(path, text);
      }
    }

    public override void AssertExists(string path, string message = null, bool isError = true)
    {
      try
      {
        base.AssertExists(path, message, isError);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.AssertExists(path, message, isError);
      }
    }

    public override void Copy(string path, string destFileName)
    {
      try
      {
        base.Copy(path, destFileName);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);
        destFileName = this.fileSystem.Path.ToUncPath(destFileName);

        base.Copy(path, destFileName);
      }
    }

    public override void Copy(string path1, string path2, bool overwrite)
    {
      try
      {
        base.Copy(path1, path2, overwrite);
      }
      catch (PathTooLongException)
      {
        path1 = this.fileSystem.Path.ToUncPath(path1);
        path2 = this.fileSystem.Path.ToUncPath(path2);

        base.Copy(path1, path2, overwrite);
      }
    }

    public override void Copy(string source, string target, bool sync = false, int timeout = 1000)
    {
      try
      {
        base.Copy(source, target, sync, timeout);
      }
      catch (PathTooLongException)
      {
        source = this.fileSystem.Path.ToUncPath(source);
        target = this.fileSystem.Path.ToUncPath(target);

        base.Copy(source, target, sync, timeout);
      }
    }

    public override void Delete(string path)
    {
      try
      {
        base.Delete(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.Delete(path);
      }
    }

    public override void DeleteIfExists(string path, string ignore = null)
    {
      try
      {
        base.DeleteIfExists(path, ignore);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.DeleteIfExists(path, ignore);
      }
    }

    public override bool Exists(string path)
    {
      try
      {
        return base.Exists(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.Exists(path);
      }
    }

    public override DateTime GetCreationTimeUtc(string path)
    {
      try
      {
        return base.GetCreationTimeUtc(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetCreationTimeUtc(path);
      }
    }

    public override long GetFileLength(string path)
    {
      try
      {
        return base.GetFileLength(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetFileLength(path);
      }
    }

    public override DateTime GetLastWriteTimeUtc(string path)
    {
      try
      {
        return base.GetLastWriteTimeUtc(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetLastWriteTimeUtc(path);
      }
    }

    public override string[] GetNeighbourFiles(string path, string filter)
    {
      try
      {
        return base.GetNeighbourFiles(path, filter);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetNeighbourFiles(path, filter);
      }
    }

    public override void Move(string path, string destFileName, bool replace = false)
    {
      try
      {
        base.Move(path, destFileName, replace);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);
        destFileName = this.fileSystem.Path.ToUncPath(destFileName);

        base.Move(path, destFileName, replace);
      }
    }

    public override FileStream OpenRead(string path)
    {
      try
      {
        return base.OpenRead(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.OpenRead(path);
      }
    }

    public override string[] ReadAllLines(string path)
    {
      try
      {
        return base.ReadAllLines(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.ReadAllLines(path);
      }
    }

    public override string ReadAllText(string path)
    {
      try
      {
        return base.ReadAllText(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.ReadAllText(path);
      }
    }

    public override void WriteAllText(string path, string text)
    {
      try
      {
        base.WriteAllText(path, text);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.WriteAllText(path, text);
      }
    }

    #endregion
  }
}