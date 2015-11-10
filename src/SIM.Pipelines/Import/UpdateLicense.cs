namespace SIM.Pipelines.Import
{
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class UpdateLicense : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      if (args.updateLicense)
      {
        string pathToDataFolder = args.rootPath.PathCombine("Data");
        FileSystem.FileSystem.Local.File.Copy(args.pathToLicenseFile, pathToDataFolder.PathCombine("license.xml"));
      }
    }

    #endregion
  }
}