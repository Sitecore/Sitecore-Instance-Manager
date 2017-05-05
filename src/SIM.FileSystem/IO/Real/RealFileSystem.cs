namespace SIM.IO.Real
{
  using System.IO;

  public class RealFileSystem : IFileSystem
  {                                   
    public IFolder ParseFolder(string path)
    {
      return new RealFolder(this, path);
    }

    public IFolder ParseFolder(DirectoryInfo dir)
    { 
      return ParseFolder(dir.FullName);
    }

    public IFile ParseFile(string path)
    {
      return new RealFile(this, path);
    }

    public IFile ParseFile(FileInfo file)
    {
      return ParseFile(file.FullName);
    }

    public IZipFile ParseZipFile(string path)
    {
      return new RealZipFile(this, path);
      ;
    }
  }
}