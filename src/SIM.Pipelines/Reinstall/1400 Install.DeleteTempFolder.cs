namespace SIM.Pipelines.Reinstall
{

  #region

  #endregion

  public class DeleteTempFolder : ReinstallProcessor
  {
    #region Protected methods

    protected override void Process(ReinstallArgs args)
    {
      FileSystem.FileSystem.Local.Directory.DeleteIfExists(args.TempFolder);
    }

    #endregion
  }
}