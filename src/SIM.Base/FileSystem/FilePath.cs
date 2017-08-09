namespace SIM.Base.FileSystem
{
  using JetBrains.Annotations;
  using System.IO;

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
