namespace SIM.Pipelines.Install
{
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class Extract : InstallProcessor
  {
    #region Public Methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return InstallHelper.GetStepsCount(((InstallArgs)args).PackagePath);
    }

    #endregion

    #region Methods

    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      var packagePath = args.PackagePath;

      var webRootPath = args.WebRootPath;
      var databasesFolderPath = args.DatabasesFolderPath;
      var dataFolderPath = args.DataFolderPath;


      InstallHelper.ExtractFile(packagePath, webRootPath, databasesFolderPath, dataFolderPath, args.InstallRadControls, args.InstallDictionaries, this.Controller);
    }

    #endregion
  }
}