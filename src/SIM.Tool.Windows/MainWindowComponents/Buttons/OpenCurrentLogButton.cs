using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class OpenCurrentLogButton : InstanceOnlyButton
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

    public override void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      if (instance != null)
      {
        InstanceHelperEx.OpenCurrentLogFile(instance, mainWindow, LogFileType);
      }
    }

    #endregion
  }
}