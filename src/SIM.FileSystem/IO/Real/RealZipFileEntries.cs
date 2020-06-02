namespace SIM.IO.Real
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Ionic.Zip;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public class RealZipFileEntries : IZipFileEntries, IDisposable
  {
    [NotNull]
    private static char[] DelimiterOptions { get; } = { '\\', '/' };

    [NotNull]
    private ZipFile ZipFile { get; }

    public RealZipFileEntries([NotNull] RealZipFile zipFile)
    {
      Assert.ArgumentNotNull(zipFile, nameof(zipFile));

      ZipFile = new ZipFile(zipFile.File.FullName);
    }

    public IEnumerable<IZipFileEntry> GetEntries()
    {
      var entries = ZipFile.Entries;
      Assert.IsNotNull(entries, nameof(entries));

      List<RealZipFileEntry> zipEntries = entries.Select(e => new RealZipFileEntry(e)).ToList();

      return zipEntries;
    }

    public bool Contains(string entryPath)
    {
      Assert.ArgumentNotNull(entryPath, nameof(entryPath));

      var entries = ZipFile.Entries;
      Assert.IsNotNull(entries, nameof(entries));
      
      if (entryPath.IndexOfAny(DelimiterOptions) < 0)
      {
        // simple case with no delimiter
        return entries.Any(e => e.FileName.Equals(entryPath, StringComparison.OrdinalIgnoreCase));
      }

      // complex case with different delimiter options
      var delimiter = FindDelimiter(entries);
      if (string.IsNullOrEmpty(delimiter))
      {
        return false;
      }

      entryPath = entryPath
        .Replace("\\", delimiter)
        .Replace("/", delimiter);

      return entries.Any(e => e.FileName.Equals(entryPath, StringComparison.OrdinalIgnoreCase));
    }

    [CanBeNull]
    private static string FindDelimiter(ICollection<ZipEntry> entries)
    {
      var entryFileName = entries.FirstOrDefault(x => x.FileName.IndexOfAny(DelimiterOptions) >= 0)?.FileName;
      if (string.IsNullOrEmpty(entryFileName))
      {
        // delimiter is specified in query, but no zip entries with delimiter found
        return null;
      }

      var delimiterPosition = entryFileName.IndexOfAny(DelimiterOptions);
      if (delimiterPosition < 0)
      {
        throw new NotSupportedException("Impossible");
      }
      
      return entryFileName.Substring(delimiterPosition, 1);
    }

    public void Dispose()
    {
      ZipFile.Dispose();
    }
  }
}
