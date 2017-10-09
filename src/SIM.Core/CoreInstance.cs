namespace SIM.Core
{
  using SIM.Instances;

  public static class CoreInstance
  {
    public static void Browse(Instance instance, string virtualUrl = null)
    {
      CoreApp.OpenInBrowser(instance.GetUrl(virtualUrl), false);
    }
  }
}