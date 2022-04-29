using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public class SxaYamlFileGeneratorHelper : IYamlFileGeneratorHelper
  {
    private readonly List<KeyValuePair<YamlNode, YamlNode>> EmptyList;
    private const string SpeImageNode = "SPE_IMAGE";
    private const string SxaImageNode = "SXA_IMAGE";
    private readonly string SpeImage;
    private readonly string SxaImage;
    private readonly string SitecoreSpeImage;
    private readonly string SitecoreSxaImage;

    public SxaYamlFileGeneratorHelper(string toplogy)
    {
      EmptyList = new List<KeyValuePair<YamlNode, YamlNode>>();
      SpeImage = "${SITECORE_MODULE_REGISTRY}spe-assets:${SPE_VERSION}";
      SitecoreSpeImage = "${SITECORE_MODULE_REGISTRY}sitecore-spe-assets:${SPE_VERSION}";
      if (toplogy == "xm1")
      {
        SxaImage = "${SITECORE_MODULE_REGISTRY}sxa-xm1-assets:${SXA_VERSION}";
        SitecoreSxaImage = "${SITECORE_MODULE_REGISTRY}sitecore-sxa-xm1-assets:${SXA_VERSION}";
      }
      else
      {
        SxaImage = "${SITECORE_MODULE_REGISTRY}sxa-xp1-assets:${SXA_VERSION}";
        SitecoreSxaImage = "${SITECORE_MODULE_REGISTRY}sitecore-sxa-xp1-assets:${SXA_VERSION}";
      }
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSpeAndSxaImagesArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SpeImage)),
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SxaImage))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSxaImageArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SxaImage))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSpeAndSxaImagesArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SitecoreSpeImage)),
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SitecoreSxaImage))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSpeImageArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SitecoreSpeImage)),
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSxaImageArgsXm1()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SitecoreSxaImage))
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