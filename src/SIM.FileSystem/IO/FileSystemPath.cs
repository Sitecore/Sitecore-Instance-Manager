namespace SIM.IO
{
  using System.IO;

  public sealed class FileSystemPath
  {
    public string Path { get; }

    public FileSystemPath(string path)
    {
      Path = System.IO.Path.GetFullPath(path);
    }
  }
}