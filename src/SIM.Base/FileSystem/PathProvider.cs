using Sitecore.Diagnostics.Base;
using JetBrains.Annotations;

namespace SIM.FileSystem
{
  public class PathProvider
  {
    #region Fields

    private FileSystem FileSystem { get; }

    #endregion

    #region Constructors

    public PathProvider(FileSystem fileSystem)
    {
      FileSystem = fileSystem;
    }

    #endregion

    #region Public methods

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

    #endregion
  }
}