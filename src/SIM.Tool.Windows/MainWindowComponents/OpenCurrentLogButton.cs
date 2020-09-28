namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

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