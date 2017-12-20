using System.Collections.Generic;

namespace SIM.IO
{
  using System;

  public interface IFolder : IFileSystemEntry, IEquatable<IFolder>
  {
    IReadOnlyList<IFileSystemEntry> GetChildren();

    IReadOnlyList<IFile> GetFiles();

    IReadOnlyList<IFolder> GetFolders();

    IFolder CopyTo(IFolder parent);

    IFolder MoveTo(IFolder parent);
  }
}