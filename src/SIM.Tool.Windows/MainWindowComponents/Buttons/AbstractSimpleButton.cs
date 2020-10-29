using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Plugins;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public abstract class WindowOnlyButton : IMainWindowButton
  {
    #region Public methods

    public virtual bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public virtual bool IsVisible(Window mainWindow, Instance instance)
    {
      return true;
    }

    public virtual void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      OnClick(mainWindow);
    }

    #endregion

    #region Protected methods

    protected abstract void OnClick([NotNull] Window mainWindow);

    #endregion
  }
}