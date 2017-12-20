namespace SIM.IO
{
  using System.IO;
  using JetBrains.Annotations;

  public static class IFileSystemExtensions
  {
    [NotNull]
    public static IFolder ParseFolder(this IFileSystem fileSystem, DirectoryInfo dir)
    {
      return fileSystem.ParseFolder(dir.FullName);
    }

    [NotNull]
    public static IFile ParseFile(this IFileSystem fileSystem, FileInfo file)
    {
      return fileSystem.ParseFile(file.FullName);
    }
  }
}
