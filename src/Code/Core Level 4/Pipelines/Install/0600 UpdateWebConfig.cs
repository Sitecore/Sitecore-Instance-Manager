#region Usings

using SIM.Base;

#endregion

namespace SIM.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   The set data folder.
  /// </summary>
  [UsedImplicitly]
  public class UpdateWebConfig : InstallProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      UpdateWebConfigHelper.Process(args.RootFolderPath, args.WebRootPath, args.DataFolderPath);
    }

    #endregion
  }
}