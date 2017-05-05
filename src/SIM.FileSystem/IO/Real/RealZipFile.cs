using System.IO.Compression;

namespace SIM.IO.Real
{
  using JetBrains.Annotations;

  public class RealZipFile : RealFile, IZipFile
  {
    private RealZipFileEntries _Entries;

    public RealZipFile([NotNull] RealFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {      
    }

    public void ExtractTo(IFolder folder)
    {
      ZipFile.ExtractToDirectory(FullName, folder.FullName);
    }

    public IZipFileEntries Entries => _Entries ?? (_Entries = new RealZipFileEntries(this));

    public void Dispose()
    {
      _Entries?.Dispose();
    }
  }
}