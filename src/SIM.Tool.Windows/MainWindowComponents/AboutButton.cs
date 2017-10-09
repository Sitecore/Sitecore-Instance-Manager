namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Windows.Dialogs;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class AboutOnlyButton : WindowOnlyButton
  {
    #region Public methods
    
    protected override void OnClick(Window mainWindow)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      WindowHelper.ShowDialog<AboutDialog>(null, mainWindow);
    }

    #endregion
  }
}