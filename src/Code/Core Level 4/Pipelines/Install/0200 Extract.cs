#region Usings

using SIM.Adapters;
using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   The extract.
  /// </summary>
  [UsedImplicitly]
  public class Extract : InstallProcessor
  {                            
    #region Public Methods

    /// <summary>
    /// The evaluate steps count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The evaluate steps count. 
    /// </returns>
    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return FileSystem.Local.File.GetFileLength(((InstallArgs)args).PackagePath); 
    }

    #endregion

    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var ignore = Settings.CoreInstallRadControls.Value ? null : "Website/sitecore/shell/RadControls";
      var controller = this.Controller;
      if (controller != null)
      {
        FileSystem.Local.Zip.UnpackZip(args.PackagePath, args.UniqueTempFolder, controller.IncrementProgress, ignore);
        return;
      }

      FileSystem.Local.Zip.UnpackZip(args.PackagePath, args.UniqueTempFolder, null, ignore);
    }

    #endregion
  }
}