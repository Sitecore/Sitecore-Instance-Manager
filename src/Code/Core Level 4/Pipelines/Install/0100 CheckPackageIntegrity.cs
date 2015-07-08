#region Usings

using SIM.Adapters;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   The check package integrity.
  /// </summary>
  [UsedImplicitly]
  public class CheckPackageIntegrity : InstallProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      FileSystem.Local.Zip.CheckZip(args.PackagePath);
    }

    #endregion
  }
}