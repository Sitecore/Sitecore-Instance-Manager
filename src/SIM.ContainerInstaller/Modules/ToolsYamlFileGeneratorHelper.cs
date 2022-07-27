using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class ToolsYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string ToolingImageNode = "TOOLING_IMAGE";
    private const string ToolingImagePath = "${SITECORE_TOOLS_REGISTRY}" + DockerSettings.SitecoreToolsImage + ":${TOOLS_VERSION}";

    public ToolsYamlFileGeneratorHelper()
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateToolingImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(ToolingImageNode), new YamlScalarNode(ToolingImagePath))
      };
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

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateIdArgs(int shortVersion, Topology topology)
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 100)
      {
        return GenerateToolingImageArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 100)
      {
        return GenerateToolingImageArgs();
      }
      return EmptyList;
    }

    public IDictionary<string, string> GetEnvironmentVariables(Role role)
    {
      return null;
    }
  }
}