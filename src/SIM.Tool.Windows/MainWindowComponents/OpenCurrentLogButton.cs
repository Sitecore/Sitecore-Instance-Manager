namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class OpenCurrentLogButton : IMainWindowButton
  {
    #region Fields

    [CanBeNull]
    private string LogFileType { get; }

    #endregion

    #region Constructors

    public OpenCurrentLogButton()
    {
    }

    public OpenCurrentLogButton([NotNull] string param)
    {
      Assert.ArgumentNotNull(param, nameof(param));

      LogFileType = param;
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      return instance != null;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      if (instance != null && MainWindowHelper.IsSitecoreMember(instance))
      {
        return false;
      }

      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      Analytics.TrackEvent("OpenLog");

      if (instance != null)
      {
        InstanceHelperEx.OpenCurrentLogFile(instance, mainWindow, LogFileType);
      }
    }

    #endregion
  }
}