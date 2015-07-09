using System;

namespace SIM.FileSystem
{
  public class TempFolder : IDisposable
  {
    #region Fields

    public readonly string Path;
    private readonly FileSystem fileSystem;

    #endregion

    #region Constructors

    public TempFolder(FileSystem fileSystem, string path = null)
    {
      this.fileSystem = fileSystem;
      if (path != null)
      {
        this.Path = fileSystem.Directory.Ensure(System.IO.Path.Combine(System.IO.Path.GetPathRoot(path), Guid.NewGuid().ToString()));
      }
      else
      {
        this.Path = fileSystem.Directory.Ensure(System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName()));
      }
    }

    #endregion

    #region Public methods

    public void Dispose()
    {
      this.fileSystem.Directory.DeleteIfExists(this.Path);
    }

    public override string ToString()
    {
      return this.Path;
    }

    #endregion
  }
}