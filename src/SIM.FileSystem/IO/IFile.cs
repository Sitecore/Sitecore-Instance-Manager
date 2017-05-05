namespace SIM.IO
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  public interface IFile : IFileSystemEntry, IEquatable<IFile>
  {
    [NotNull]
    Stream Open(OpenFileMode mode, OpenFileAccess access, OpenFileShare share);

    bool Exists { get; }

    IFile MoveTo(IFolder parent);
  }
}
