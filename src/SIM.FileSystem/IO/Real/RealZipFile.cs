using System.IO.Compression;

namespace SIM.IO.Real
{
  using JetBrains.Annotations;

  public class RealZipFile : RealFile, IZipFile
  {
    public RealZipFile([NotNull] RealFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {      
    }

    public void ExtractTo(IFolder folder)
    {
      ZipFile.ExtractToDirectory(FullName, folder.FullName);
    }
  }
}