namespace SIM.Pipelines.Import
{
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