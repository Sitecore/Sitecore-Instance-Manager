namespace SIM.Pipelines.Reinstall
{
  using System.IO;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class Extract : ReinstallProcessor
  {
    #region Public Methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return InstallHelper.GetStepsCount(((ReinstallArgs)args).PackagePath);
    }

    #endregion

    #region Methods

    protected override void Process(ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var installRadControls = Directory.Exists(Path.Combine(args.WebRootPath, InstallHelper.RadControls));
      var installDictionaries = Directory.Exists(Path.Combine(args.WebRootPath, InstallHelper.Dictionaries));

      InstallHelper.ExtractFile(args.PackagePath, args.WebRootPath, args.DatabasesFolderPath, args.DataFolderPath, installRadControls, installDictionaries, this.Controller);
    }

    #endregion
  }
}