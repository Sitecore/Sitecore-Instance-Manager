#region Usings

using SIM.Base;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The check package integrity.
  /// </summary>
  [UsedImplicitly]
  public class CheckPackageIntegrity : ReinstallProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      FileSystem.Local.Zip.CheckZip(args.PackagePath);
    }

    #endregion
  }
}