namespace SIM.IO
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  public interface IFile : IFileSystemEntry, IEquatable<IFile>
  {
    [NotNull]
    Stream Open(OpenFileMode mode, OpenFileAccess access, OpenFileShare share);

    IFile CopyTo(IFolder parent);

    IFile MoveTo(IFolder parent);
  }
}
