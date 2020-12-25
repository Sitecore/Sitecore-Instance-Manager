using System.Windows;
using SIM.Instances;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Windows.MainWindowComponents.Groups
{
  public class WindowOnlyGroup : IMainWindowGroup
  {
    #region Public methods

    public virtual bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    #endregion
  }
}