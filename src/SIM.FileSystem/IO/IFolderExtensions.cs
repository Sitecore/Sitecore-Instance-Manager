namespace SIM.IO
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public static class IFolderExtensions
  {
    [NotNull]
    public static IFile GetChildFile([NotNull] this IFolder folder, [NotNull] string fileName)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));
      Assert.ArgumentNotNullOrEmpty(fileName, nameof(fileName));

      return folder.FileSystem.ParseFile(System.IO.Path.Combine(folder.FullName, fileName));
    }
  }
}