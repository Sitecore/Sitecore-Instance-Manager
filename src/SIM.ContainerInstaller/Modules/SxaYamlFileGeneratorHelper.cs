using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class SxaYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string SpeImageNode = "SPE_IMAGE";
    private const string SxaImageNode = "SXA_IMAGE";
    private readonly string SpeImagePath;
    private readonly string SxaImagePath;
    private readonly string SitecoreSpeImagePath;
    private readonly string SitecoreSxaImagePath;

    public SxaYamlFileGeneratorHelper(string toplogy)
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();
      SpeImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SpeImage + ":${SPE_VERSION}";
      SitecoreSpeImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreSpeImage + ":${SPE_VERSION}";
      if (toplogy == "xm1")
      {
        SxaImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SxaXm1Image + ":${SXA_VERSION}";
        SitecoreSxaImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreSxaXm1Image + ":${SXA_VERSION}";
      }
      else
      {
        SxaImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SxaXpImage + ":${SXA_VERSION}";
        SitecoreSxaImagePath = "${SITECORE_MODULE_REGISTRY}" + DockerSettings.SitecoreSxaXpImage + ":${SXA_VERSION}";
      }
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSpeAndSxaImagesArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SpeImagePath)),
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SxaImagePath))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSxaImageArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SxaImagePath))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSpeAndSxaImagesArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SitecoreSpeImagePath)),
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SitecoreSxaImagePath))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSpeImageArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SitecoreSpeImagePath)),
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSxaImageArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SitecoreSxaImagePath))
      };
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor100()
    {
      return GenerateSpeAndSxaImagesArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrArgsFor100()
    {
      return GenerateSxaImageArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor100()
    {
      return GenerateSxaImageArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor100()
    {
      return GenerateSpeAndSxaImagesArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor101()
    {
      return GenerateSpeAndSxaImagesArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrInitArgsFor101()
    {
      return GenerateSxaImageArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor101()
    {
      return GenerateSxaImageArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor101()
    {
      return GenerateSpeAndSxaImagesArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor102()
    {
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlInitArgsFor102()
    {
      return GenerateSitecoreSpeImageArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrInitArgsFor102()
    {
      return GenerateSitecoreSxaImageArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor102()
    {
      return GenerateSitecoreSxaImageArgsXm1();
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor102()
    {
      return GenerateSitecoreSpeAndSxaImagesArgsXm1();
    }
  }
}