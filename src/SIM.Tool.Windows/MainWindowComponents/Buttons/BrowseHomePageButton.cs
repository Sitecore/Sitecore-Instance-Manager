using JetBrains.Annotations;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class BrowseHomePageButton : BrowseButton
  {
    public BrowseHomePageButton() : base("/")
    {
    }
  }
}