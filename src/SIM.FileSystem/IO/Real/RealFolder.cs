namespace SIM.IO.Real
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  public class RealFolder : FileSystemEntry, IFolder
  {
    public RealFolder([NotNull] SIM.IO.IFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {
      DirectoryInfo = new DirectoryInfo(FullName);
    }

    [NotNull]
    public DirectoryInfo DirectoryInfo { get; }

    public bool Exists => DirectoryInfo.Exists;

    public bool TryCreate()
    {
      if (!DirectoryInfo.Exists)
      {
        return false;
      }

      try
      {
        DirectoryInfo.Create();

        return true;
      }
      catch
      {
        return false;
      }
    }

    public void Create()
    {
      DirectoryInfo.Create();
    }

    public bool Equals(IFolder other)
    {
      return FullName.Equals(other?.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      return Equals((RealFolder)obj);
    }             
  }
}