using System.IO.Compression;

namespace SIM.IO.Real
{
  using JetBrains.Annotations;

  public class RealZipFile : IZipFile
  {
    [NotNull]
    public IFile File { get; }

    private RealZipFileEntries _Entries;

    public RealZipFile([NotNull] IFile file)
    {
      File = file;
    }

    public void ExtractTo(IFolder folder)
    {
      ZipFile.ExtractToDirectory(File.FullName, folder.FullName);
    }

    public IZipFileEntries Entries => _Entries ?? (_Entries = new RealZipFileEntries(this));

    public void Dispose()
    {
      _Entries?.Dispose();
    }
  }
}