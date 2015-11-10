namespace SIM.Tool.Base.Plugins
{
  using System.Windows;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base.Annotations;

  public interface IMainWindowButton
  {
    #region Public methods

    bool IsEnabled([NotNull] Window mainWindow, [CanBeNull] Instance instance);

    void OnClick([NotNull] Window mainWindow, [CanBeNull] Instance instance);

    #endregion
  }
}