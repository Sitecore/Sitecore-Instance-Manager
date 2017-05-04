using System.Collections.Generic;

namespace SIM.IO
{
  using System;

  public interface IFolder : IFileSystemEntry, IEquatable<IFolder>
  {
    bool TryCreate();

    void Create();

    bool Exists { get; }

    IReadOnlyList<IFileSystemEntry> GetChildren();

    IReadOnlyList<IFile> GetFiles();

    IReadOnlyList<IFolder> GetFolders();

    void MoveTo(IFolder parent);
  }
}