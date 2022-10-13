using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class SpsYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string PortNumber = "80";
    private const string SpsAssetsImageNode = "SPS_ASSETS_IMAGE";
    private const string SpsImageNode = "SPS_IMAGE";
    private const string CmImageNode = "SITECORE_CM_IMAGE";
    private readonly string NewSpsImage;
    public readonly string BaseSpsImage;
    private readonly string SpsAssetsImage;
    public readonly string IsolationNode = "${ISOLATION}";
    private const string SpsContext = "./docker/build/sps";

    public SpsYamlFileGeneratorHelper(string topology)
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();
      NewSpsImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-sps:${VERSION:-latest}";
      BaseSpsImage = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpsImage + ":${SPS_VERSION}";
      switch (topology)
      {
        case "xm1":
          SpsAssetsImage = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpsAssetsXm1Image + ":${SPS_ASSETS_VERSION}";
          break;
        case "xp0":
          SpsAssetsImage = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpsAssetsXp0Image + ":${SPS_ASSETS_VERSION}";
          break;
        case "xp1":
          SpsAssetsImage = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpsAssetsXp1Image + ":${SPS_ASSETS_VERSION}";
          break;
        default:
          break;
      }
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSpsAssetsImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpsAssetsImageNode), new YamlScalarNode(SpsAssetsImage))
      };
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 100 && shortVersion < 102)
      {
        return GenerateSpsAssetsImageArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlInitArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 102)
      {
        return GenerateSpsAssetsImageArgs();
      }
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
      return GenerateSpsAssetsImageArgs();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgs(int shortVersion, Topology topology)
    {
      return GenerateSpsAssetsImageArgs();
    }

    public IDictionary<string, string> GetEnvironmentVariables(Service service)
    {
      Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

      if (service == Service.Cm)
      {
        environmentVariables.Add("Sitecore_Publishing_Service_Url", "http://sps/");
      }

      return environmentVariables;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateServices(int shortVersion, IEnumerable<IYamlFileGeneratorHelper> helpers, string newCmImage)
    {
      yield return GenerateSpsMsSqlInitService(shortVersion);
      yield return GenerateSpsService(shortVersion, newCmImage);
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateSpsMsSqlInitService(int shortVersion)
    {
      List<KeyValuePair<YamlNode, YamlNode>> nodes = new List<KeyValuePair<YamlNode, YamlNode>>();
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("image"), new YamlScalarNode(BaseSpsImage)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("isolation"), new YamlScalarNode(IsolationNode)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("environment"), GenerateEnvironmentYamlNodeForMsSqlInit(shortVersion)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("command"), new YamlScalarNode("schema upgrade --force")));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("depends_on"), new YamlMappingNode(
        new YamlScalarNode(GetSpsMssqlInitDependsOn(shortVersion)), new YamlMappingNode(new YamlScalarNode("condition"), new YamlScalarNode("service_healthy"))
      )));

      return new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(DockerSettings.SpsMsSqlInitServiceName), new YamlMappingNode(nodes));
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateSpsService(int shortVersion, string newCmImage)
    {
      List<KeyValuePair<YamlNode, YamlNode>> nodes = new List<KeyValuePair<YamlNode, YamlNode>>();
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("image"), new YamlScalarNode(NewSpsImage)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("build"), new YamlMappingNode(
        new YamlScalarNode("context"), new YamlScalarNode(SpsContext),
        new YamlScalarNode("args"), new YamlMappingNode(
          new YamlScalarNode(SpsImageNode), new YamlScalarNode(BaseSpsImage),
          new YamlScalarNode(CmImageNode), new YamlScalarNode(newCmImage)
      ))));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("isolation"), new YamlScalarNode(IsolationNode)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("environment"), GenerateEnvironmentYamlNodeForSps(shortVersion)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("ports"), new YamlSequenceNode(
        new YamlScalarNode(PortNumber) { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
      )));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("depends_on"), new YamlSequenceNode(
        new YamlScalarNode(DockerSettings.SpsMsSqlInitServiceName), 
        new YamlScalarNode(DockerSettings.CmServiceName)
      )));

      return new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(DockerSettings.SpsServiceName), new YamlMappingNode(nodes));
    }

    private YamlMappingNode GenerateEnvironmentYamlNodeForMsSqlInit(int shortVersion)
    {
      YamlMappingNode environmentYamlMappingNode = new YamlMappingNode();

      if (shortVersion == 101)
      {
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_License"), new YamlScalarNode("${SITECORE_LICENSE}"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Core"),
          new YamlScalarNode("Data Source=mssql;Initial Catalog=Sitecore.Core;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Master"),
          new YamlScalarNode("Data Source=mssql;Initial Catalog=Sitecore.Master;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Service"),
          new YamlScalarNode("Data Source=mssql;Initial Catalog=Sitecore.Master;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Web"),
          new YamlScalarNode("Data Source=mssql;Initial Catalog=Sitecore.Web;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
      }
      else if (shortVersion >= 102)
      {
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_License"), new YamlScalarNode("${SITECORE_LICENSE}"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Core"),
          new YamlScalarNode("Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Core;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Master"),
          new YamlScalarNode("Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Master;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Service"),
          new YamlScalarNode("Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Master;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Web"),
          new YamlScalarNode("Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Web;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
      }

      return environmentYamlMappingNode;
    }

    private YamlMappingNode GenerateEnvironmentYamlNodeForSps(int shortVersion)
    {
      YamlMappingNode environmentYamlMappingNode = new YamlMappingNode();

      if (shortVersion == 101)
      {
        environmentYamlMappingNode.Add(new YamlScalarNode("ASPNETCORE_URLS"), "http://*:" + PortNumber);
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_License"), "${SITECORE_LICENSE}");
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Core"),
          new YamlScalarNode("Data Source=mssql;Initial Catalog=Sitecore.Core;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Master"),
          new YamlScalarNode("Data Source=mssql;Initial Catalog=Sitecore.Master;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Service"),
          new YamlScalarNode("Data Source=mssql;Initial Catalog=Sitecore.Master;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Web"),
          new YamlScalarNode("Data Source=mssql;Initial Catalog=Sitecore.Web;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
      }
      else if (shortVersion >= 102)
      {
        environmentYamlMappingNode.Add(new YamlScalarNode("ASPNETCORE_URLS"), "http://*:" + PortNumber);
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_License"), "${SITECORE_LICENSE}");
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Core"),
          new YamlScalarNode("Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Core;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Master"),
          new YamlScalarNode("Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Master;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Service"),
          new YamlScalarNode("Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Master;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
        environmentYamlMappingNode.Add(new YamlScalarNode("SITECORE_Publishing__ConnectionStrings__Web"),
          new YamlScalarNode("Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Web;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True"));
      }

      return environmentYamlMappingNode;
    }

    private string GetSpsMssqlInitDependsOn(int shortVersion)
    {
      if (shortVersion >= 102)
      {
        return DockerSettings.MsSqlInitServiceName;
      }

      return DockerSettings.MsSqlServiceName;
    }
  }
}