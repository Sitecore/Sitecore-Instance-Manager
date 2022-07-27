using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class HorizonYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string IdHostVariable = "https://${ID_HOST}";
    private const string CmHostVariable = "https://${CM_HOST}";
    private const string HorizonHostVariable = "https://${HORIZON_HOST}";
    private const string HorizonAssetsImageNode = "HORIZON_RESOURCES_IMAGE";
    private readonly string HorizonAssetsImagePath;
    public readonly string HorizonImagePath;
    public readonly string IsolationNode = "${ISOLATION}";

    public HorizonYamlFileGeneratorHelper(string topology)
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();

      switch (topology)
      {
        case "xm1":
          HorizonAssetsImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreHorizonAssetsXm1Image + ":${HORIZON_ASSETS_VERSION}";
          break;
        case "xp0":
          HorizonAssetsImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreHorizonAssetsXp0Image + ":${HORIZON_ASSETS_VERSION}";
          break;
        case "xp1":
          HorizonAssetsImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreHorizonAssetsXp1Image + ":${HORIZON_ASSETS_VERSION}";
          break;
        default:
          break;
      }

      HorizonImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreHorizonImage + ":${HORIZON_VERSION}";
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgs(int shortVersion, Topology topology)
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlInitArgs(int shortVersion, Topology topology)
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrArgs(int shortVersion, Topology topology)
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrInitArgs(int shortVersion, Topology topology)
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgs(int shortVersion, Topology topology)
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgs(int shortVersion, Topology topology)
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(HorizonAssetsImageNode), new YamlScalarNode(HorizonAssetsImagePath))
      };
    }

    public IDictionary<string, string> GetEnvironmentVariables(Role role)
    {
      Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

      switch (role)
      {
        case Role.Id:
          environmentVariables.Add("Sitecore_Sitecore__IdentityServer__Clients__DefaultClient__AllowedCorsOrigins__AllowedCorsOriginsGroup2", HorizonHostVariable);
          break;
        case Role.Cm:
          environmentVariables.Add("Sitecore_Horizon_ClientHost", HorizonHostVariable);
          break;
        default:
          break;
      }

      return environmentVariables;
    }

    public IDictionary<string, string> GetHorizonEnvironmentVariables(bool includeSXA, bool includeSCCH)
    {
      Dictionary<string, string> environmentVariables = new Dictionary<string, string>();
      environmentVariables.Add("Sitecore_License", "${SITECORE_LICENSE}");
      environmentVariables.Add("Sitecore_FederatedUI__HostBaseUrl", "http://hrz");
      environmentVariables.Add("Sitecore_SitecorePlatform__ContentManagementUrl", CmHostVariable);
      environmentVariables.Add("Sitecore_SitecorePlatform__ContentManagementInternalUrl", "http://cm");
      environmentVariables.Add("Sitecore_Sitecore__Authentication__OpenIdConnectOptions__RequireHttpsMetadata", "false");
      environmentVariables.Add("Sitecore_Sitecore__Authentication__OpenIdConnectOptions__Authority", IdHostVariable);
      environmentVariables.Add("Sitecore_Sitecore__Authentication__OpenIdConnectOptions__CallbackAuthority", HorizonHostVariable);
      environmentVariables.Add("Sitecore_Sitecore__Authentication__OpenIdConnectOptions__InternalAuthority", "http://id");
      environmentVariables.Add("Sitecore_Sitecore__Authentication__BearerAuthenticationOptions__Authority", IdHostVariable);
      environmentVariables.Add("Sitecore_Sitecore__Authentication__BearerAuthenticationOptions__InternalAuthority", "http://id");
      environmentVariables.Add("Sitecore_Sitecore__Authentication__BearerAuthenticationOptions__RequireHttpsMetadata", "false");
      if (includeSXA)
      {
        environmentVariables.Add("Sitecore_Plugins__Filters__ExperienceAccelerator", "+SXA");
      }
      if (includeSCCH)
      {
        environmentVariables.Add("Sitecore_Plugins__Filters__ContentHub", "+ContentHub");
      }
      return environmentVariables;
    }

    public IEnumerable<string> GetHorizonLabels()
    {
      return new List<string>()
      {
        "traefik.enable=true",
        "traefik.http.middlewares.force-STS-Header.headers.forceSTSHeader=true",
        "traefik.http.middlewares.force-STS-Header.headers.stsSeconds=31536000",
        "traefik.http.routers.sh-secure.entrypoints=websecure",
        "traefik.http.routers.sh-secure.rule=Host(`${HORIZON_HOST}`)",
        "traefik.http.routers.sh-secure.tls=true",
        "traefik.http.routers.sh-secure.middlewares=force-STS-Header",
        "traefik.http.services.sh.loadbalancer.server.port=80",
      };
    }
  }
}