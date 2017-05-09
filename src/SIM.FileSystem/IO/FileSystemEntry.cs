using System.IO;

namespace SIM.IO
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public abstract class FileSystemEntry : IFileSystemEntry
  {
    protected FileSystemEntry([NotNull] IFileSystem fileSystem, [NotNull] string path)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      FileSystem = fileSystem;
      FullName = System.IO.Path.GetFullPath(path);
    }

    public IFileSystem FileSystem { get; }

    public string FullName { get; }

    public string Name => Path.GetFileName(FullName);

    public abstract void TryDelete();

    public abstract void Create();

    public abstract bool Exists { get; }

    public abstract IFolder Parent { get; }

    public override int GetHashCode()
    {
      return FullName.GetHashCode();
    }

    public override string ToString()
    {
      return FullName;
    }
  }
}