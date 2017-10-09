namespace SIM.FileSystem
{
  using System;

  public static class PathUtils
  {
    public static string EscapePath(string path, string escapeText = null)
    {
      escapeText = escapeText ?? String.Empty;
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
  }
}