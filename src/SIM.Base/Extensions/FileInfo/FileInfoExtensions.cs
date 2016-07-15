namespace SIM.Extensions.FileInfo
{
  using System.IO;

  public static class FileInfoExtensions
  {
    public static void WriteAllText(this FileInfo file, string contents)
    {
      File.WriteAllText(file.FullName, contents);
    }
  }
}
