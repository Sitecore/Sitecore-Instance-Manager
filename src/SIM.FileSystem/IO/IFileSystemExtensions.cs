namespace SIM.IO
{
  using System.IO;

  public static class IFileSystemExtensions
  {
    public static IFolder ParseFolder(this IFileSystem fileSystem, DirectoryInfo dir)
    {
      return fileSystem.ParseFolder(dir.FullName);
    }

    public static IFile ParseFile(this IFileSystem fileSystem, FileInfo file)
    {
      return fileSystem.ParseFile(file.FullName);
    }
  }
}
