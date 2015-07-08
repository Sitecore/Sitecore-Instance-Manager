#region Usings

using SIM.Base;
using SIM.Pipelines.Install;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The move data.
  /// </summary>
  [UsedImplicitly]
  public class MoveDatabases : ReinstallProcessor
  {
    protected override bool IsRequireProcessing(ReinstallArgs args)
    {
      return AttachDatabasesHelper.IsRemoteSqlServer();
    }

    protected override void Process(ReinstallArgs args)
    {
      var databasesFolderPath = args.DatabasesFolderPath;
      AttachDatabasesHelper.MoveDatabases(databasesFolderPath);
    }
  }
}