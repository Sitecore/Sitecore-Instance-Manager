namespace SIM.Pipelines.Reinstall
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class SetupWebsite : ReinstallProcessor
  {
    #region Constants

    private const string NetFrameworkV2 = "v2.0";

    #endregion

    #region Protected methods

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

    #endregion
  }
}