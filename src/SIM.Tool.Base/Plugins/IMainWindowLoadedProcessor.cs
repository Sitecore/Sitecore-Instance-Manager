namespace SIM.Tool.Base.Plugins
{
  using System.Windows;

  public interface IMainWindowLoadedProcessor
  {
    #region Public methods

    void Process(Window mainWindow);

    #endregion
  }
}