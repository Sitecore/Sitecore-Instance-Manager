using System.Collections.Generic;
using System.Linq;

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

      this.FileSystem = fileSystem;
    }

    [NotNull]
    private new MockFileSystem FileSystem { get; }

    public override void TryDelete()
    {
      FileSystem.Folders.Remove(FullName);
    }

    public bool Exists { get; private set; }
    
    public IReadOnlyList<IFileSystemEntry> GetChildren()
    {
      return GetFolders()
        .Cast<IFileSystemEntry>()
        .Concat(GetFiles())
        .ToArray();
    }

    public IReadOnlyList<IFile> GetFiles()
    {
      var prefix = $"{FullName}\\";

      return FileSystem.Files
        .Where(x => x.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        .Where(x => x.Key.IndexOf("\\", prefix.Length + 1) >= 0)
        .Select(x => x.Value)
        .ToArray();
    }

    public IReadOnlyList<IFolder> GetFolders()
    {
      var prefix = $"{FullName}\\";

      return FileSystem.Folders
        .Where(x => x.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        .Where(x => x.Key.IndexOf("\\", prefix.Length + 1) >= 0)
        .Select(x => x.Value)
        .ToArray();
    }

    public IFolder MoveTo(IFolder parent)
    {
      throw new NotImplementedException();
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