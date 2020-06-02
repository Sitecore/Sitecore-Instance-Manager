using System.Collections.Generic;

namespace SIM.IO
{
  using JetBrains.Annotations;

  public interface IZipFileEntries
  {
    bool Contains([NotNull] string entryPath);

    IEnumerable<IZipFileEntry> GetEntries();
  }
}