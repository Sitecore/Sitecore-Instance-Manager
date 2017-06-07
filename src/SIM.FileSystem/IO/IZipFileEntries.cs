namespace SIM.IO
{
  using JetBrains.Annotations;

  public interface IZipFileEntries
  {
    bool Contains([NotNull] string entryPath);
  }
}