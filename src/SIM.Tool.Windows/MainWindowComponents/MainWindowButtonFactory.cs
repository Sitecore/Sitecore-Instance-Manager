using SIM.Instances;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Windows.MainWindowComponents.Buttons;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public static class MainWindowButtonFactory
  {
    public static IMainWindowButton GetBrowseButton(Instance instance)
    {
      if (instance != null && instance.Type == Instance.InstanceType.SitecoreContainer)
      {
        return new BrowseSitecoreContainerWebsiteButton();
      }

      return new BrowseHomePageButton();
    }
  }
}
