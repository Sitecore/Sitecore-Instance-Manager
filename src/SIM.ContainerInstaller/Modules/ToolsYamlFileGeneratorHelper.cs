using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class ToolsYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string ToolingImageNode = "TOOLING_IMAGE";
    private const string ToolingImage = "${SITECORE_TOOLS_REGISTRY}sitecore-docker-tools-assets:${TOOLS_VERSION}";

    public ToolsYamlFileGeneratorHelper()
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateToolingImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(ToolingImageNode), new YamlScalarNode(ToolingImage))
      };
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor100()
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrArgsFor100()
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor100()
    {
      return GenerateToolingImageArgs();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor100()
    {
      return GenerateToolingImageArgs();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor101()
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrInitArgsFor101()
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor101()
    {
      return GenerateToolingImageArgs();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor101()
    {
      return GenerateToolingImageArgs();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor102()
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlInitArgsFor102()
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrInitArgsFor102()
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor102()
    {
      return GenerateToolingImageArgs();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor102()
    {
      return GenerateToolingImageArgs();
    }
  }
}