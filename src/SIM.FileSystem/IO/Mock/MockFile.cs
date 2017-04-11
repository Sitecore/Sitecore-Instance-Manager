namespace SIM.IO.Mock
{
  using System;
  using System.IO;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.IO.Real;

  public class MockFile : MockFileSystemEntry, IFile
  {                          
    public MockFile(MockFileSystem fileSystem, string path, [NotNull] string contents)
      : base(fileSystem, path)
    {
      Assert.ArgumentNotNullOrEmpty(contents, nameof(contents));

      Contents = contents;
      Folder = fileSystem.ParseFolder(Path.GetDirectoryName(path));
    }

    public string Contents { get; set; }

    public Stream Open(OpenFileMode mode, OpenFileAccess access, OpenFileShare share)
    {
      throw new NotImplementedException();
    }

    public IFolder Folder { get; }

    public bool Exists
    {
      get
      {
        return MockFileSystem.Contains(FullName);
      }

      set
      {
        if (!MockFileSystem.Contains(FullName))
        {
          MockFileSystem.Add(FullName, this);
        }                
      }
    }                      

    public bool Equals(IFile other)
    {
      return FullName.Equals(other?.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      return Equals((IFile)obj);
    }               
  }
}