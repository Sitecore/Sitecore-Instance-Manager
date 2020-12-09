using System.Windows;
using JetBrains.Annotations;
using SIM.Tool.Base;
using SIM.Tool.Windows.Dialogs;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
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