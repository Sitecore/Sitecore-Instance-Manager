namespace SIM.IO.Real
{
  public class RealFileSystem : IFileSystem
  {                                   
    public IFolder ParseFolder(string path)
    {
      return new RealFolder(this, path);
    }

    public IFile ParseFile(string path)
    {
      return new RealFile(this, path);
    }

    public IZipFile ParseZipFile(string path)
    {
      return new RealZipFile(this, path);
      ;
    }
  }
}