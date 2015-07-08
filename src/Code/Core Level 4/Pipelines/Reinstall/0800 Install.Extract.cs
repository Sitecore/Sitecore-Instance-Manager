#region Usings

using SIM.Base;
using SIM.Pipelines.Install;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The extract.
  /// </summary>
  [UsedImplicitly]
  public class Extract : ReinstallProcessor
  {
    #region Constants

    /// <summary>
    ///   The k.
    /// </summary>
    private const int K = 40;

    #endregion

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

      return 1; // K;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var ignore1 = Settings.CoreInstallRadControls.Value ? null : "Website/sitecore/shell/RadControls";

      var ignore2 = Settings.CoreInstallDictionaries.Value ? null : "Website/sitecore/shell/Controls/Rich Text Editor/Dictionaries";

      var controller = this.Controller;
      if (controller != null)
      {
        FileSystem.Local.Zip.UnpackZip(args.PackagePath, args.TempFolder, controller.IncrementProgress, ignore1, ignore2);
        return;
      }

      FileSystem.Local.Zip.UnpackZip(args.PackagePath, args.TempFolder, null, ignore1, ignore2);
    }

    #endregion
  }
}