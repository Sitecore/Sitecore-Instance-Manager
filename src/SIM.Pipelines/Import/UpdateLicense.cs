namespace SIM.Pipelines.Import
{
  using JetBrains.Annotations;
  using SIM.Extensions;

  [UsedImplicitly]
  public class UpdateLicense : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      if (args.updateLicense)
      {
        var pathToDataFolder = args.rootPath.PathCombine("Data");
        FileSystem.FileSystem.Local.File.Copy(args.pathToLicenseFile, pathToDataFolder.PathCombine("license.xml"));
      }
    }

    #endregion
  }
}