#region Usings

using SIM.Base;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete temp folder.
  /// </summary>
  public class DeleteTempFolder : ReinstallProcessor
  {
    #region Protected methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process(ReinstallArgs args)
    {
      FileSystem.Local.Directory.DeleteIfExists(args.TempFolder);
    }

    #endregion
  }
}