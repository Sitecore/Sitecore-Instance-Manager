#region Usings

using SIM.Base;
using SIM.Pipelines.Install;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The set data folder.
  /// </summary>
  [UsedImplicitly]
  public class UpdateWebConfig : ReinstallProcessor
  {
    #region Constants

    /// <summary>
    ///   The data folder.
    /// </summary>
    private const string DataFolder = "dataFolder";

    #endregion

    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      UpdateWebConfigHelper.Process(args.RootPath, args.WebRootPath, args.DataFolderPath);
    }

    #endregion
  }
}