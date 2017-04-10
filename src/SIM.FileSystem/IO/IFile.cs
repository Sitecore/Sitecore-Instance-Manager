namespace SIM.IO
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  public interface IFile : IFileSystemEntry, IEquatable<IFile>
  {
    [NotNull]
    Stream Open(OpenFileMode mode, OpenFileAccess access);
                                            
    [NotNull]
    IFolder Folder { get; }

    bool Exists { get; }
  }
}
