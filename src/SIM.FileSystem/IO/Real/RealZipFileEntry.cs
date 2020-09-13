using JetBrains.Annotations;
using Ionic.Zip;
using Sitecore.Diagnostics.Base;

namespace SIM.IO.Real
{
  public class RealZipFileEntry: IZipFileEntry
  {
    [NotNull]
    private ZipEntry ZipEntry { get; }

    public RealZipFileEntry([NotNull] ZipEntry zipEntry)
    {
      Assert.ArgumentNotNull(zipEntry, nameof(zipEntry));

      ZipEntry = zipEntry;
    }

    public string Name => ZipEntry.FileName;
  }
}