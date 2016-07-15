namespace SIM.Extensions.DirectoryInfo
{
  using System.IO;
  using Sitecore.Diagnostics.Base;

  public static class DirectoryInfoExtensions
  {
    public static FileInfo GetFile(this DirectoryInfo dir, string name)
    {
      Assert.ArgumentNotNull(dir, nameof(dir));
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      return new FileInfo(Path.Combine(dir.FullName, name));
    }                                
  }
}
