namespace SIM.IO
{
  using JetBrains.Annotations;

  public interface IFileSystemEntry
  {             
    [NotNull]
    SIM.IO.IFileSystem FileSystem { get; }

    [NotNull]
    string FullName { get; }

    [NotNull]
    string Name { get; }

    void TryDelete();
  }
}