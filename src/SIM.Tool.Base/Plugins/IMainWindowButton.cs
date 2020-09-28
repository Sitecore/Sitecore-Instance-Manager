namespace SIM.Tool.Base.Plugins
{
  using System.Windows;
  using SIM.Instances;
  using JetBrains.Annotations;

  public interface IMainWindowButton
  {
    #region Public methods

    //string Label { get; set; }

    bool IsEnabled([NotNull] Window mainWindow, [CanBeNull] Instance instance);

    bool IsVisible([NotNull] Window mainWindow, [CanBeNull] Instance instance);

    void OnClick([NotNull] Window mainWindow, [CanBeNull] Instance instance);

    #endregion
  }
}