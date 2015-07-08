using System;

namespace SIM.Base
{
  public class TempFolder : IDisposable
  {
    private readonly FileSystem fileSystem;
    public readonly string Path;
    public override string ToString()
    {
      return this.Path;
    }

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

    public void Dispose()
    {
      this.fileSystem.Directory.DeleteIfExists(Path);
    }
  }
}