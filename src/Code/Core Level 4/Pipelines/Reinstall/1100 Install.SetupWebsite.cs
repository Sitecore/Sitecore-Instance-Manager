#region Usings

using SIM.Base;
using SIM.Pipelines.Install;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The setup website.
  /// </summary>
  [UsedImplicitly]
  public class SetupWebsite : ReinstallProcessor
  {
    #region Constants

    /// <summary>
    ///   The net framework v 2.
    /// </summary>
    private const string NetFrameworkV2 = "v2.0";

    #endregion

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      string name = args.Name;
      var bindings = args.Bindings;
      string webRootPath = args.WebRootPath;
      bool enable32BitAppOnWin64 = args.Is32Bit;
      bool forceNetFramework4 = args.ForceNetFramework4;
      bool isClassic = args.IsClassic;
      SetupWebsiteHelper.SetupWebsite(enable32BitAppOnWin64, webRootPath, forceNetFramework4, isClassic, bindings, name);
    }

  }
}