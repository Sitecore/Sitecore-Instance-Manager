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

  public enum Role
  {
    MsSql,
    MsSqlInit,
    Solr,
    SolrInit,
    Id,
    Cd,
    Cm
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
    public const string SitecoreHorizonImage = "sitecore-horizon";
    public const string SitecoreHorizonAssetsXm1Image = "horizon-integration-xm1-assets";
    public const string SitecoreHorizonAssetsXp0Image = "horizon-integration-xp0-assets";
    public const string SitecoreHorizonAssetsXp1Image = "horizon-integration-xp1-assets";

    public static string SitecoreToolsImagePath = $"{SitecoreToolsNamespace}/{SitecoreToolsImage}";
    public static string SpeImagePath = $"{SitecoreModuleNamespace}/{SpeImage}";
    public static string SitecoreSpeImagePath = $"{SitecoreModuleNamespace}/{SitecoreSpeImage}";
    public static string SxaXm1ImagePath = $"{SitecoreModuleNamespace}/{SxaXm1Image}";
    public static string SitecoreSxaXm1ImagePath = $"{SitecoreModuleNamespace}/{SitecoreSxaXm1Image}";
    public static string SxaXpImagePath = $"{SitecoreModuleNamespace}/{SxaXpImage}";
    public static string SitecoreSxaXpImagePath = $"{SitecoreModuleNamespace}/{SitecoreSxaXpImage}";
    public static string SitecoreHorizonImagePath = $"{SitecoreModuleNamespace}/{SitecoreHorizonImage}";
    public static string SitecoreHorizonAssetsXm1ImagePath = $"{SitecoreModuleNamespace}/{SitecoreHorizonAssetsXm1Image}";
    public static string SitecoreHorizonAssetsXp0ImagePath = $"{SitecoreModuleNamespace}/{SitecoreHorizonAssetsXp0Image}";
    public static string SitecoreHorizonAssetsXp1ImagePath = $"{SitecoreModuleNamespace}/{SitecoreHorizonAssetsXp1Image}";

    public const string DockerComposeOverrideFileName = "docker-compose.override.yml";
    // This value must be the same as in the "SIM.Pipelines.Install.Containers.GenerateEnvironmentData.siteTypes" list.
    public const string HorizonServiceName = "hrz";
    // Horizon only works correctly if its, Identity Server and Content Management instances belong to the same site (domain) as mentioned in Horizon Installation Guide.
    public const string HostNameTemplate = "{0}.{1}";
    public const string HostNameKeyPattern = "([A-Za-z0-9]{1,3})_HOST";
  }
}