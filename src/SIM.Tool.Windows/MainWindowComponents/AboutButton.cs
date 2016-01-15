namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Tool.Base;
  using SIM.Tool.Windows.Dialogs;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class AboutOnlyButton : WindowOnlyButton
  {
    #region Public methods
    
    protected override void OnClick(Window mainWindow)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      WindowHelper.ShowDialog<AboutDialog>(null, mainWindow);
    }

    #endregion
  }
}