namespace SIM.IO.Real
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  public class RealFile : FileSystemEntry, IFile
  {
    [CanBeNull]
    private IFolder _Parent;

    public RealFile([NotNull] IFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {
      FileInfo = new FileInfo(path);
    }

    [NotNull]   
    public FileInfo FileInfo { get; }
                
    public override IFolder Parent => _Parent ?? (_Parent = FileSystem.ParseFolder(Path.GetDirectoryName(FileInfo.FullName)));

    public override void Create()
    {
      FileInfo.OpenWrite().Close();
    }

    public override bool Exists => FileInfo.Exists;

    public IFile CopyTo(IFolder parent)
    {
      var newFullName = Path.Combine(parent.FullName, Name);
      var newFile = FileSystem.ParseFile(newFullName);
      newFile.Parent.Create();
      File.Copy(FullName, newFullName, true);

      return newFile;
    }

    public IFile MoveTo(IFolder parent)
    {
      var newFullName = Path.Combine(parent.FullName, Name);
      File.Delete(newFullName);
      File.Move(FullName, newFullName);

      return FileSystem.ParseFile(newFullName);
    }

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

    public override void TryDelete()
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