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

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSpeAndSxaImagesArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SpeImagePath)),
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SxaImagePath))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSxaImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SxaImagePath))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSpeAndSxaImagesArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SitecoreSpeImagePath)),
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SitecoreSxaImagePath))
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSpeImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SpeImageNode), new YamlScalarNode(SitecoreSpeImagePath)),
      };
    }

    private IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSitecoreSxaImageArgs()
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(SxaImageNode), new YamlScalarNode(SitecoreSxaImagePath))
      };
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 100 && shortVersion < 102)
      {
        return GenerateSpeAndSxaImagesArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlInitArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 102)
      {
        return GenerateSitecoreSpeImageArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrArgs(int shortVersion, Topology topology)
    {
      if (shortVersion == 100)
      {
        return GenerateSxaImageArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrInitArgs(int shortVersion, Topology topology)
    {
      if (shortVersion == 101)
      {
        return GenerateSxaImageArgs();
      }
      else if (shortVersion >= 102)
      {
        return GenerateSitecoreSxaImageArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 100 && shortVersion < 102)
      {
        return GenerateSxaImageArgs();
      }
      else if (shortVersion >= 102)
      {
        return GenerateSitecoreSxaImageArgs();
      }
      return EmptyList;
    }

    public IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgs(int shortVersion, Topology topology)
    {
      if (shortVersion >= 100 && shortVersion < 102)
      {
        return GenerateSpeAndSxaImagesArgs();
      }
      else if (shortVersion >= 102)
      {
        return GenerateSitecoreSpeAndSxaImagesArgs();
      }
      return EmptyList;
    }

    public IDictionary<string, string> GetEnvironmentVariables(Service service)
    {
      return new Dictionary<string, string>();
    }
  }
}