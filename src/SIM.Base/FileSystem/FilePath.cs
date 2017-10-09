namespace SIM.FileSystem
{
  using System.IO;
  using JetBrains.Annotations;

  public sealed class FilePath
  {
    [NotNull]
    public string FullName { get; }

    public FilePath([NotNull] string fullname)
    {
      FullName = Path.GetFullPath(fullname);
    }

    [NotNull]
    public string Extension => Path.GetExtension(FullName);

    [NotNull]
    public static implicit operator string([NotNull] FilePath filePath)
    {
      return filePath.FullName;
    }

    public override string ToString()
    {
      return FullName;
    }
  }
}
