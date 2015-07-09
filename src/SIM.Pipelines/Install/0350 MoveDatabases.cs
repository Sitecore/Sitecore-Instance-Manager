namespace SIM.Pipelines.Install
{
  public class MoveDatabases : InstallProcessor
  {
    #region Protected methods

    protected override bool IsRequireProcessing(InstallArgs args)
    {
      return AttachDatabasesHelper.IsRemoteSqlServer();
    }

    protected override void Process(InstallArgs args)
    {
      var databasesFolderPath = args.DatabasesFolderPath;
      AttachDatabasesHelper.MoveDatabases(databasesFolderPath);
    }

    #endregion
  }
}