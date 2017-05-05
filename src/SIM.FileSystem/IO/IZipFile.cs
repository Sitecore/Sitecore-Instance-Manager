namespace SIM.IO
{
  using System;
  public interface IZipFile : IFile, IDisposable
  {
    void ExtractTo(IFolder folder);

    IZipFileEntries Entries { get; }
  }
}