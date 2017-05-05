namespace SIM.IO.Real
{
  using System;
  using System.Linq;
  using Ionic.Zip;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Extensions;

  public class RealZipFileEntries : IZipFileEntries, IDisposable
  {
    [NotNull]
    private ZipFile ZipFile { get; }

    public RealZipFileEntries([NotNull] RealZipFile zipFile)
    {
      Assert.ArgumentNotNull(zipFile, nameof(zipFile));

      ZipFile = new ZipFile(zipFile.FullName);
    }

    public bool Contains(string entryPath)
    {
      var entries = ZipFile.Entries;
      Assert.IsNotNull(entries, nameof(entries));

      return entries.Any(e => e.FileName.EqualsIgnoreCase(entryPath));
    }

    public void Dispose()
    {
      ZipFile.Dispose();
    }
  }
}
