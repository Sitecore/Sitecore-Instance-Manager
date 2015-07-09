namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class MoveDatabases : ReinstallProcessor
  {
    #region Protected methods

    protected override bool IsRequireProcessing(ReinstallArgs args)
    {
      return AttachDatabasesHelper.IsRemoteSqlServer();
    }

    protected override void Process(ReinstallArgs args)
    {
      var databasesFolderPath = args.DatabasesFolderPath;
      AttachDatabasesHelper.MoveDatabases(databasesFolderPath);
    }

    #endregion
  }
}