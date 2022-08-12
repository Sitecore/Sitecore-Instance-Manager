using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class HorizonYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string IdHostVariable = "https://${ID_HOST}";
    private const string CmHostVariable = "https://${CM_HOST}";
    private const string HorizonHostVariable = "https://${HORIZON_HOST}";
    private const string HorizonAssetsImageNode = "HORIZON_IMAGE";
    private readonly string HorizonAssetsImagePath;
    public readonly string HorizonImagePath;
    public readonly string IsolationNode = "${ISOLATION}";

    public HorizonYamlFileGeneratorHelper(string topology)
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();
      HorizonImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreHorizonImage + ":${HORIZON_VERSION}";
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
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateHorizonImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(HorizonAssetsImageNode), new YamlScalarNode(HorizonAssetsImagePath))
      };
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgs(int shortVersion, Topology topology)
    {
      return GenerateHorizonImageArgs();
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
      return GenerateHorizonImageArgs();
    }

    public IDictionary<string, string> GetEnvironmentVariables(Service service)
    {
      Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

      switch (service)
      {
        case Service.Id:
          environmentVariables.Add("Sitecore_Sitecore__IdentityServer__Clients__DefaultClient__AllowedCorsOrigins__AllowedCorsOriginsGroup2", HorizonHostVariable);
          break;
        case Service.Cm:
          environmentVariables.Add("Sitecore_Horizon_ClientHost", HorizonHostVariable);
          break;
        default:
          break;
      }

      return environmentVariables;
    }

    public KeyValuePair<YamlNode, YamlNode> GenerateService(IEnumerable<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> nodes = new List<KeyValuePair<YamlNode, YamlNode>>();

      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("image"), new YamlScalarNode(HorizonImagePath)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("isolation"), new YamlScalarNode(IsolationNode)));

      bool includeSXA = false;
      if (helpers.OfType<SxaYamlFileGeneratorHelper>().Any())
      {
        includeSXA = true;
      }

      bool includeSCCH = false;
      // For future implementation when support of Sitecore Connected for Content Hub will be added
      /*if (helpers.OfType<SCCHYamlFileGeneratorHelper>().Any())
      {
        includeSCCH = true;
      }*/

      YamlMappingNode environmentYamlMappingNode = new YamlMappingNode();
      foreach (KeyValuePair<string, string> envVariable in GetHorizonEnvironmentVariables(includeSXA, includeSCCH))
      {
        if (bool.TryParse(envVariable.Value, out bool result))
        {
          environmentYamlMappingNode.Add(new YamlScalarNode(envVariable.Key), new YamlScalarNode(envVariable.Value) { Style = YamlDotNet.Core.ScalarStyle.SingleQuoted });
        }
        else
        {
          environmentYamlMappingNode.Add(new YamlScalarNode(envVariable.Key), new YamlScalarNode(envVariable.Value));
        }
      }
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("environment"), environmentYamlMappingNode));

      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("depends_on"), new YamlMappingNode(
        new YamlScalarNode("id"), new YamlMappingNode(new YamlScalarNode("condition"), new YamlScalarNode("service_started")),
        new YamlScalarNode("cm"), new YamlMappingNode(new YamlScalarNode("condition"), new YamlScalarNode("service_started"))
      )));

      YamlSequenceNode labelsYamlSequenceNode = new YamlSequenceNode();
      foreach (string label in GetHorizonLabels())
      {
        labelsYamlSequenceNode.Add(new YamlScalarNode(label) { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted });
      }
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("labels"), labelsYamlSequenceNode));

      return new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(DockerSettings.HorizonServiceName), new YamlMappingNode(nodes));
    }

    private IDictionary<string, string> GetHorizonEnvironmentVariables(bool includeSXA, bool includeSCCH)
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

    private IEnumerable<string> GetHorizonLabels()
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