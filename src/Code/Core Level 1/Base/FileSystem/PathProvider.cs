namespace SIM.Base
{
  public class PathProvider
  {
    private readonly FileSystem fileSystem;

    public PathProvider(FileSystem fileSystem)
    {
      this.fileSystem = fileSystem;
    }

    public virtual string EscapePath(string path, string escapeText = null)
    {
      escapeText = escapeText ?? string.Empty;
      return path
        .Replace(":", escapeText)
        .Replace("/", escapeText)
        .Replace("\\", escapeText)
        .Replace("*", escapeText)
        .Replace("|", escapeText)
        .Replace("?", escapeText)
        .Replace("<", escapeText)
        .Replace(">", escapeText);
    }

    [NotNull]
    public virtual string ToUncPath([NotNull] string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, "path");
      
      const string Prefix = @"\\127.0.0.1\";
      if (path.Length == 1 || path[1] != ':' || path.StartsWith(Prefix))
      {
        return path;
      }

      return Prefix + path.Replace(":", "$");
    }
  }
}