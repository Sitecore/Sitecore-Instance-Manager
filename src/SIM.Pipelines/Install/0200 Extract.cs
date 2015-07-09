namespace SIM.Pipelines.Install
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class Extract : InstallProcessor
  {
    #region Public Methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return FileSystem.FileSystem.Local.File.GetFileLength(((InstallArgs)args).PackagePath);
    }

    #endregion

    #region Methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var ignore = Settings.CoreInstallRadControls.Value ? null : "Website/sitecore/shell/RadControls";
      var controller = this.Controller;
      if (controller != null)
      {
        FileSystem.FileSystem.Local.Zip.UnpackZip(args.PackagePath, args.UniqueTempFolder, controller.IncrementProgress, ignore);
        return;
      }

      FileSystem.FileSystem.Local.Zip.UnpackZip(args.PackagePath, args.UniqueTempFolder, null, ignore);
    }

    #endregion
  }
}