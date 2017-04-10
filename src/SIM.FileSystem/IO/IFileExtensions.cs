namespace SIM.IO
{
  using System.IO;
  using JetBrains.Annotations;

  public static class IFileExtensions
  {                      
    [NotNull]
    public static Stream OpenRead(this IFile file)
    {
      return file.Open(OpenFileMode.Open, OpenFileAccess.Read, OpenFileShare.Read);
    }

    [NotNull]
    public static Stream OpenWrite(this IFile file)
    {                              
      return file.Open(OpenFileMode.OpenOrCreate, OpenFileAccess.Write, OpenFileShare.None);
    }
  }
}