namespace SIM.Tool.Base.Plugins
{
  /// <summary>
  ///   A processor that implements this interface will be loaded before showing main window. The
  ///   SIM.Tools.Base.Profile.ProfileManager, SIM.Pipelines.PipelineManager and SIM.Tool.App.WizardPipelineManager are
  ///   available for using.
  /// </summary>
  public interface IInitProcessor
  {
    #region Public methods

    void Process();

    #endregion
  }
}