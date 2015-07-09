namespace SIM.Pipelines.Import
{
  using SIM.Pipelines.Install;

  public class UpdateDataFolder : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      string websiteFolderPath = args.rootPath;
      SetupWebsiteHelper.SetDataFolder(websiteFolderPath);
    }

    #endregion
  }
}