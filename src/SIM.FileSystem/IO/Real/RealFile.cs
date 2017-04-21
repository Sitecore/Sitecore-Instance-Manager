namespace SIM.IO.Real
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  public class RealFile : FileSystemEntry, IFile
  {                              
    public RealFile([NotNull] RealFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {
      FileInfo = new FileInfo(path);
      Folder = fileSystem.ParseFolder(Path.GetDirectoryName(path));
    }

    [NotNull]   
    public FileInfo FileInfo { get; }
                
    public IFolder Folder { get; }

    public bool Exists => FileInfo.Exists;

    public Stream Open(OpenFileMode mode, OpenFileAccess access, OpenFileShare share)
    {
      return FileInfo.Open((FileMode)mode, (FileAccess)access, (FileShare)share);
    }

    public bool Equals(IFile other)
    {
      return FullName.Equals(other?.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      return Equals((RealFile)obj);
    }                   

    public void TryDelete()
    {
      try
      {
        File.Delete(FullName);
      }
      catch
      {
        // we don't care if we cannot delete the file whatever error happens
      }
    }
  }
}