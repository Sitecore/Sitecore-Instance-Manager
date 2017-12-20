namespace SIM.Pipelines.Import
{
  public class UpdateDataFolder : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      var websiteFolderPath = args._RootPath;
      SetupWebsiteHelper.SetDataFolder(websiteFolderPath);
    }

    #endregion
  }
}