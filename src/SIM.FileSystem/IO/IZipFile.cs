namespace SIM.IO
{
  public interface IZipFile : IFile
  {
    void ExtractTo(IFolder folder);
  }
}