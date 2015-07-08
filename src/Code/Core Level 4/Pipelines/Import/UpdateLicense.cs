using SIM.Base;


namespace SIM.Pipelines.Import
{
  [UsedImplicitly]
  public class UpdateLicense : ImportProcessor
  {
    protected override void Process(ImportArgs args)
    {
      if (args.updateLicense)
      {
        string pathToDataFolder = args.rootPath.PathCombine("Data");
        FileSystem.Local.File.Copy(args.pathToLicenseFile, pathToDataFolder.PathCombine("license.xml"));
      }
    }
  }
}
