namespace SIM.Pipelines.Reinstall
{
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class Extract : ReinstallProcessor
  {
    #region Constants

    private const int K = 40;

    #endregion

    #region Public Methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 1; // K;
    }

    #endregion

    #region Methods

    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var ignore1 = Settings.CoreInstallRadControls.Value ? null : "Website/sitecore/shell/RadControls";

      var ignore2 = Settings.CoreInstallDictionaries.Value ? null : "Website/sitecore/shell/Controls/Rich Text Editor/Dictionaries";

      var controller = this.Controller;
      if (controller != null)
      {
        FileSystem.FileSystem.Local.Zip.UnpackZip(args.PackagePath, args.TempFolder, controller.IncrementProgress, ignore1, ignore2);
        return;
      }

      FileSystem.FileSystem.Local.Zip.UnpackZip(args.PackagePath, args.TempFolder, null, ignore1, ignore2);
    }

    #endregion
  }
}