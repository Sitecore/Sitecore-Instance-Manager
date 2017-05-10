using System;

namespace SIM.FileSystem
{
  public class TempFolder : IDisposable
  {
    #region Fields

    public string Path { get; }
    private FileSystem FileSystem { get; }

    #endregion

    #region Constructors

    public TempFolder(FileSystem fileSystem, string path, bool? rooted)
    {
      FileSystem = fileSystem;
      if (path != null)
      {
        if (rooted != false)
        Path = fileSystem.Directory.Ensure(System.IO.Path.Combine(System.IO.Path.GetPathRoot(path), Guid.NewGuid().ToString()));
        else
        {
          Path = fileSystem.Directory.Ensure(System.IO.Path.Combine(path, Guid.NewGuid().ToString("N")));
        }
      }
      else
      {
        Path = fileSystem.Directory.Ensure(System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName()));
      }
    }

    #endregion

    #region Public methods

    public void Dispose()
    {
      FileSystem.Directory.DeleteIfExists(Path);
    }

    public override string ToString()
    {
      return Path;
    }

    #endregion
  }
}