namespace SIM.IO.Real
{
  using System.IO;
  using JetBrains.Annotations;

  public class RealTempFile : RealFile, ITempFile
  {
    public RealTempFile([NotNull] RealFileSystem fileSystem) : base(fileSystem, Path.GetTempFileName())
    {
    }

    public void Dispose()
    {
      TryDelete();
    }
  }
}