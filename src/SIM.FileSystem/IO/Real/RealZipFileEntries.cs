namespace SIM.IO.Real
{
  using System;
  using System.Linq;
  using Ionic.Zip;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  
  public class RealZipFileEntries : IZipFileEntries, IDisposable
  {
    [NotNull]
    private ZipFile ZipFile { get; }

    public RealZipFileEntries([NotNull] RealZipFile zipFile)
    {
      Assert.ArgumentNotNull(zipFile, nameof(zipFile));

      ZipFile = new ZipFile(zipFile.File.FullName);
    }

    public bool Contains(string entryPath)
    {
      Assert.ArgumentNotNull(entryPath, nameof(entryPath));

      entryPath = entryPath.Replace("/", "\\");
      var entries = ZipFile.Entries;
      Assert.IsNotNull(entries, nameof(entries));

      return entries.Any(e => e.FileName.Equals(entryPath, StringComparison.OrdinalIgnoreCase));
    }

    public void Dispose()
    {
      ZipFile.Dispose();
    }
  }
}
