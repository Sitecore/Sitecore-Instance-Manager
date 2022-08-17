using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class SpsYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string PortNumber = "80";
    private const string SpsAssetsImageNode = "SPS_IMAGE";
    public readonly string SpsImagePath;
    private readonly string SpsAssetsImagePath;
    public readonly string IsolationNode = "${ISOLATION}";

    public SpsYamlFileGeneratorHelper(string topology)
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();
      SpsImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpsImage + ":${SPS_VERSION}";
      switch (topology)
      {
        case "xm1":
          SpsAssetsImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpsAssetsXm1Image + ":${SPS_ASSETS_VERSION}";
          break;
        case "xp0":
          SpsAssetsImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpsAssetsXp0Image + ":${SPS_ASSETS_VERSION}";
          break;
        case "xp1":
          SpsAssetsImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpsAssetsXp1Image + ":${SPS_ASSETS_VERSION}";
          break;
        default:
          break;
      }
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSpsAssetsImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpsAssetsImageNode), new YamlScalarNode(SpsAssetsImagePath))
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

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateServices(int shortVersion, IEnumerable<IYamlFileGeneratorHelper> helpers)
    {
      yield return GenerateSpsMsSqlInitService(shortVersion);
      yield return GenerateSpsService(shortVersion);
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateSpsMsSqlInitService(int shortVersion)
    {
      List<KeyValuePair<YamlNode, YamlNode>> nodes = new List<KeyValuePair<YamlNode, YamlNode>>();

      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("image"), new YamlScalarNode(SpsImagePath)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("isolation"), new YamlScalarNode(IsolationNode)));

      YamlMappingNode environmentYamlMappingNode = new YamlMappingNode();
      foreach (KeyValuePair<string, string> envVariable in GetSpsMsSqlInitEnvironmentVariables(shortVersion))
      {
        environmentYamlMappingNode.Add(new YamlScalarNode(envVariable.Key), new YamlScalarNode(envVariable.Value));
      }
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("environment"), environmentYamlMappingNode));

      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("command"), new YamlScalarNode("schema upgrade --force")));

      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("depends_on"), new YamlMappingNode(
        new YamlScalarNode(GetSpsMssqlInitDependsOn(shortVersion)), new YamlMappingNode(new YamlScalarNode("condition"), new YamlScalarNode("service_healthy"))
      )));

      return new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(DockerSettings.SpsMsSqlInitServiceName), new YamlMappingNode(nodes));
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateSpsService(int shortVersion)
    {
      List<KeyValuePair<YamlNode, YamlNode>> nodes = new List<KeyValuePair<YamlNode, YamlNode>>();

      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("image"), new YamlScalarNode(SpsImagePath)));
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("isolation"), new YamlScalarNode(IsolationNode)));

      YamlMappingNode environmentYamlMappingNode = new YamlMappingNode();
      foreach (KeyValuePair<string, string> envVariable in GetSpsEnvironmentVariables(shortVersion))
      {
        /*if (envVariable.Value.StartsWith("http"))
        {
          environmentYamlMappingNode.Add(new YamlScalarNode(envVariable.Key), new YamlScalarNode(envVariable.Value) { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted });
        }
        else
        {
          environmentYamlMappingNode.Add(new YamlScalarNode(envVariable.Key), new YamlScalarNode(envVariable.Value));
        }*/
        environmentYamlMappingNode.Add(new YamlScalarNode(envVariable.Key), new YamlScalarNode(envVariable.Value));
      }
      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("environment"), environmentYamlMappingNode));

      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("ports"), 
        new YamlSequenceNode(new YamlScalarNode(PortNumber) { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted })));

      nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("depends_on"),
        new YamlSequenceNode(new YamlScalarNode(DockerSettings.SpsMsSqlInitServiceName))));

      return new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(DockerSettings.SpsServiceName), new YamlMappingNode(nodes));
    }

    private IDictionary<string, string> GetSpsMsSqlInitEnvironmentVariables(int shortVersion)
    {
      Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

      if (shortVersion == 101)
      {
        environmentVariables.Add("SITECORE_Sitecore", "Publishing:ConnectionStrings:Core: Data Source=mssql;Initial Catalog=Sitecore.Core;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Sitecore", "Publishing:ConnectionStrings:Master: Data Source=mssql;Initial Catalog=Sitecore.Master;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Sitecore", "Publishing:ConnectionStrings:Web: Data Source=mssql;Initial Catalog=Sitecore.Web;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
      }
      else if (shortVersion >= 102)
      {
        environmentVariables.Add("SITECORE_License", "${SITECORE_LICENSE}");
        environmentVariables.Add("SITECORE_Publishing__ConnectionStrings__Core", "Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Core;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Publishing__ConnectionStrings__Master", "Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Master;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Publishing__ConnectionStrings__Service", "Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Master;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Publishing__ConnectionStrings__Web", "Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Web;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
      }

      return environmentVariables;
    }

    private IDictionary<string, string> GetSpsEnvironmentVariables(int shortVersion)
    {
      Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

      if (shortVersion == 101)
      {
        environmentVariables.Add("ASPNETCORE_URLS", "http://*:" + PortNumber);
        environmentVariables.Add("SITECORE_Sitecore", "Publishing:ConnectionStrings:Core: Data Source=mssql;Initial Catalog=Sitecore.Core;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Sitecore", "Publishing:ConnectionStrings:Master: Data Source=mssql;Initial Catalog=Sitecore.Master;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Sitecore", "Publishing:ConnectionStrings:Web: Data Source=mssql;Initial Catalog=Sitecore.Web;User ID=sa;Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
      }
      else if (shortVersion >= 102)
      {
        environmentVariables.Add("ASPNETCORE_URLS", "http://*:" + PortNumber);
        environmentVariables.Add("SITECORE_License", "${SITECORE_LICENSE}");
        environmentVariables.Add("SITECORE_Publishing__ConnectionStrings__Core", "Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Core;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Publishing__ConnectionStrings__Master", "Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Master;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Publishing__ConnectionStrings__Service", "Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Master;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
        environmentVariables.Add("SITECORE_Publishing__ConnectionStrings__Web", "Data Source=${SQL_SERVER};Initial Catalog=Sitecore.Web;User ID=${SQL_SA_LOGIN};Password=${SQL_SA_PASSWORD};MultipleActiveResultSets=True");
      }

      return environmentVariables;
    }

    private string GetSpsMssqlInitDependsOn(int shortVersion)
    {
      if (shortVersion == 101)
      {
        return "mssql";
      }

      return "mssql-init";
    }
  }
}