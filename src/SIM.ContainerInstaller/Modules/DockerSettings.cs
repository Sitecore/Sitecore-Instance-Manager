using System.ComponentModel;

namespace SIM.ContainerInstaller.Modules
{
  public enum Topology
  {
    Xm1,
    Xp0,
    Xp1
  }

  public enum Module
  {
    [Description("SXA")]
    SXA,
    [Description("JSS")]
    JSS,
    [Description("Horizon")]
    Horizon,
    [Description("Publishing Service")]
    PublishingService
  }

  public static class DockerSettings
  {
    public const string SitecoreContainerRegistryHost = "scr.sitecore.com";
    public const string SitecoreToolsNamespace = "tools";
    public const string SitecoreModuleNamespace = "sxp/modules";
    public const string SitecoreToolsImage = "sitecore-docker-tools-assets";
    public const string SpeImage = "spe-assets";
    public const string SitecoreSpeImage = "sitecore-spe-assets";
    public const string SxaXm1Image = "sxa-xm1-assets";
    public const string SitecoreSxaXm1Image = "sitecore-sxa-xm1-assets";
    public const string SxaXpImage = "sxa-xp1-assets";
    public const string SitecoreSxaXpImage = "sitecore-sxa-xp1-assets";

    public static string SitecoreToolsImagePath = $"{SitecoreToolsNamespace}/{SitecoreToolsImage}";
    public static string SpeImagePath = $"{SitecoreModuleNamespace}/{SpeImage}";
    public static string SitecoreSpeImagePath = $"{SitecoreModuleNamespace}/{SitecoreSpeImage}";
    public static string SxaXm1ImagePath = $"{SitecoreModuleNamespace}/{SxaXm1Image}";
    public static string SitecoreSxaXm1ImagePath = $"{SitecoreModuleNamespace}/{SitecoreSxaXm1Image}";
    public static string SxaXpImagePath = $"{SitecoreModuleNamespace}/{SxaXpImage}";
    public static string SitecoreSxaXpImagePath = $"{SitecoreModuleNamespace}/{SitecoreSxaXpImage}";
  }
}
