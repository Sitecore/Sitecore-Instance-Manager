namespace SIM.Pipelines.Install
{
  using System;
  using System.IO;
  using System.Linq;
  using Ionic.Zip;
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

    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      var packagePath = args.PackagePath;

      var webRootPath = args.WebRootPath;
      var databasesFolderPath = args.DatabasesFolderPath;
      var dataFolderPath = args.DataFolderPath;


      InstallHelper.ExtractFile(packagePath, webRootPath, databasesFolderPath, dataFolderPath, this.Controller);
    }

    #endregion
  }
}