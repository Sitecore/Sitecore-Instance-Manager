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
      if (args._UpdateLicense)
      {
        var pathToDataFolder = args._RootPath.PathCombine("Data");
        FileSystem.FileSystem.Local.File.Copy(args._PathToLicenseFile, pathToDataFolder.PathCombine("license.xml"));
      }
    }

    #endregion
  }
}