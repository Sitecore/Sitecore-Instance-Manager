using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class JssYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string JssImageNode = "JSS_IMAGE";
    private readonly string JssImagePath;
    private readonly string SitecoreHeadlessServicesImagePath;

    public JssYamlFileGeneratorHelper(string topology)
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();
      switch (topology)
      {
        case "xm1":
          JssImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.JssXm1Image + ":${JSS_VERSION}";
          SitecoreHeadlessServicesImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreHeadlessServicesXm1Image + ":${JSS_VERSION}";
          break;
        case "xp0":
        case "xp1":
          JssImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.JssXpImage + ":${JSS_VERSION}";
          SitecoreHeadlessServicesImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreHeadlessServicesXpImage + ":${JSS_VERSION}";
          break;
        default:
          break;
      }
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateJssImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(JssImageNode), new YamlScalarNode(JssImagePath))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreHeadlessServicesImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(JssImageNode), new YamlScalarNode(SitecoreHeadlessServicesImagePath))
      };
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgs(int shortVersion, Topology topology)
    {
      if (shortVersion == 100)
      {
        return GenerateJssImageArgs();
      }
      else if (shortVersion == 101)
      {
        return GenerateSitecoreHeadlessServicesImageArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlInitArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 102)
      {
        return GenerateSitecoreHeadlessServicesImageArgs();
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
      if (shortVersion == 100)
      {
        return GenerateJssImageArgs();
      }
      else if (shortVersion >= 101)
      {
        return GenerateSitecoreHeadlessServicesImageArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgs(int shortVersion, Topology topology)
    {
      if (shortVersion == 100)
      {
        return GenerateJssImageArgs();
      }
      else if (shortVersion >= 101)
      {
        return GenerateSitecoreHeadlessServicesImageArgs();
      }
      return EmptyList;
    }

    public IDictionary<string, string> GetEnvironmentVariables(Service service)
    {
      return new Dictionary<string, string>();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateServices(int shortVersion, IEnumerable<IYamlFileGeneratorHelper> helpers, string newCmImage)
    {
      return EmptyList;
    }
  }
}