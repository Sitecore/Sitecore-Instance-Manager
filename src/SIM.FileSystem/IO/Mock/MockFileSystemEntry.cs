namespace SIM.IO.Mock
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public abstract class MockFileSystemEntry : FileSystemEntry
  {
    protected MockFileSystemEntry(MockFileSystem fileSystem, string fullPath)
      : base(fileSystem, fullPath)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
      Assert.ArgumentNotNullOrEmpty(fullPath, nameof(fullPath));
                                   
      MockFileSystem = fileSystem;         
    }

    [NotNull]
    protected MockFileSystem MockFileSystem { get; }       
  }
}