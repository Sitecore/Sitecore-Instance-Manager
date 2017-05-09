namespace SIM.IO
{
  using System;
  public interface IZipFile : IDisposable
  {
    IFile File { get; }

    void ExtractTo(IFolder folder);

    IZipFileEntries Entries { get; }
  }
}