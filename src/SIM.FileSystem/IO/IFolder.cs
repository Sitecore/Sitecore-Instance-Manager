namespace SIM.IO
{
  using System;

  public interface IFolder : IFileSystemEntry, IEquatable<IFolder>
  {
    bool TryCreate();

    void Create();

    bool Exists { get; }
  }
}