namespace SIM.IO.Mock
{
  using System;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.IO.Real;

  public class MockFolder : FileSystemEntry, IFolder
  {
    public MockFolder([NotNull] MockFileSystem fileSystem, [NotNull] string fullPath) : base(fileSystem, fullPath)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));
      Assert.ArgumentNotNullOrEmpty(fullPath, nameof(fullPath));
    }

    public bool Exists { get; private set; }

    public bool TryCreate()
    {
      var existedBefore = Exists;

      Exists = true;

      return !existedBefore;
    }

    public void Create()
    {
      Exists = true;
    }
              
    public bool Equals(IFolder other)
    {
      return FullName.Equals(other?.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      return Equals((MockFolder)obj);
    }
  }
}