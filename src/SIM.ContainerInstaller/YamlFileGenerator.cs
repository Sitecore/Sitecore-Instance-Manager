using JetBrains.Annotations;
using SIM.ContainerInstaller.Modules;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller
{
  [UsedImplicitly]
  public class YamlFileGenerator
  {
    private const string DockerComposeFileName = "docker-compose.override.yml";
    private const string FileVersion = "2.4";
    private const string MsSqlContext = "./docker/build/mssql";
    private const string MsSqlInitContext = "./docker/build/mssql-init";
    private const string SolrContext = "./docker/build/solr";
    private const string SolrInitContext = "./docker/build/solr-init";
    private const string CmContext = "./docker/build/cm";
    private const string CdContext = "./docker/build/cd";
    private readonly IEnumerable<string> MsSqlVolumes;
    private readonly IEnumerable<string> SolrVolumes;
    private const string BaseImageNodeName = "BASE_IMAGE";
    private readonly string NewMsSqlImage;
    private readonly string BaseMsSqlImage;
    private readonly string NewMsSqlInitImage;
    private readonly string BaseMsSqlInitImage;
    private readonly string NewSolrImage;
    private readonly string BaseSolrImage;
    private readonly string NewSolrInitImage;
    private readonly string BaseSolrInitImage;
    private readonly string NewCdImage;
    private readonly string BaseCdImage;
    private readonly string NewCmImage;
    private readonly string BaseCmImage;

    public YamlFileGenerator(string topology)
    {
      MsSqlVolumes = new List<string>() { ".\\mssql-data:c:\\data" };
      SolrVolumes = new List<string>() { ".\\solr-data:c:\\data" };
      NewMsSqlImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-mssql:${VERSION:-latest}";
      BaseMsSqlImage = "${SITECORE_DOCKER_REGISTRY}sitecore-" + topology + "-mssql:${SITECORE_VERSION}";
      NewMsSqlInitImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-mssql-init:${VERSION:-latest}";
      if (topology == "xp0")
      {
        BaseMsSqlInitImage = "${SITECORE_DOCKER_REGISTRY}sitecore-xp1-mssql-init:${SITECORE_VERSION}";
      }
      else
      {
        BaseMsSqlInitImage = "${SITECORE_DOCKER_REGISTRY}sitecore-" + topology + "-mssql-init:${SITECORE_VERSION}";
      }
      NewSolrImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-solr:${VERSION:-latest}";
      BaseSolrImage = "${SITECORE_DOCKER_REGISTRY}sitecore-" + topology + "-solr:${SITECORE_VERSION}";
      NewSolrInitImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-solr-init:${VERSION:-latest}";
      BaseSolrInitImage = "${SITECORE_DOCKER_REGISTRY}sitecore-" + topology + "-solr-init:${SITECORE_VERSION}";
      NewCdImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-cd:${VERSION:-latest}";
      BaseCdImage = "${SITECORE_DOCKER_REGISTRY}sitecore-" + topology + "-cd:${SITECORE_VERSION}";
      NewCmImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-cm:${VERSION:-latest}";
      BaseCmImage = "${SITECORE_DOCKER_REGISTRY}sitecore-" + topology + "-cm:${SITECORE_VERSION}";
    }

    public void Generate(string path, List<IYamlFileGeneratorHelper> helpers, string versionAndTopology)
    {
      YamlStream yamlStream = null;

      switch (versionAndTopology)
      {
        case "100xm1":
          yamlStream = GenerateYamlFileFor100Xm1(helpers);
          break;
        case "100xp0":
          yamlStream = GenerateYamlFileFor100Xp0(helpers);
          break;
        case "100xp1":
          yamlStream = GenerateYamlFileFor100Xp1(helpers);
          break;
        case "101xm1":
          yamlStream = GenerateYamlFileFor101Xm1(helpers);
          break;
        case "101xp0":
          yamlStream = GenerateYamlFileFor101Xp0(helpers);
          break;
        case "101xp1":
          yamlStream = GenerateYamlFileFor101Xp1(helpers);
          break;
        case "102xm1":
          yamlStream = GenerateYamlFileFor102Xm1(helpers);
          break;
        case "102xp0":
          yamlStream = GenerateYamlFileFor102Xp0(helpers);
          break;
        case "102xp1":
          yamlStream = GenerateYamlFileFor102Xp1(helpers);
          break;
        default:
          break;
      }

      if (yamlStream != null)
      {
        using (StreamWriter writer = new StreamWriter(Path.Combine(path, DockerComposeFileName)))
        {
          yamlStream.Save(writer, false);
        }
      }
    }

    private YamlStream GenerateYamlFile(List<KeyValuePair<YamlNode, YamlNode>> services)
    {
      return new YamlStream(
       new YamlDocument(
         new YamlMappingNode(
           new YamlScalarNode("version"), new YamlScalarNode(FileVersion) { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
           new YamlScalarNode("services"), new YamlMappingNode(services)
         )
       )
     );
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateService(string service, string image = null, 
      string context = null, List<KeyValuePair<YamlNode, YamlNode>> args = null, string memoryLimit = null, IEnumerable<string> volumes = null)
    {
      List<KeyValuePair<YamlNode, YamlNode>> nodes = new List<KeyValuePair<YamlNode, YamlNode>>();

      if (image != null)
      {
        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("image"), new YamlScalarNode(image)));
      }

      if (context != null && args != null)
      {
        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("build"), new YamlMappingNode(
          new YamlScalarNode("context"), new YamlScalarNode(context),
          new YamlScalarNode("args"), new YamlMappingNode(args)
        )));
      }

      if (memoryLimit != null)
      {
        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("mem_limit"), new YamlScalarNode(memoryLimit)));
      }

      if (volumes != null)
      {
        YamlSequenceNode yamlSequenceNode = new YamlSequenceNode();
        foreach (string volume in volumes)
        {
          yamlSequenceNode.Add(new YamlScalarNode(volume));
        }
        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("volumes"), yamlSequenceNode));
      }

      return new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(service), new YamlMappingNode(nodes));
    }

    private List<KeyValuePair<YamlNode, YamlNode>> GenerateBaseImageArgs(string baseImage)
    {
      return new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(BaseImageNodeName), new YamlScalarNode(baseImage))
      };
    }

    private YamlStream GenerateYamlFileFor100Xm1(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlArgs = GenerateBaseImageArgs(BaseMsSqlImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrArgs = GenerateBaseImageArgs(BaseSolrImage);
      List<KeyValuePair<YamlNode, YamlNode>> cdArgs = GenerateBaseImageArgs(BaseCdImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlArgs.AddRange(helper.GenerateMsSqlArgsFor100());
        solrArgs.AddRange(helper.GenerateSolrArgsFor100());
        cdArgs.AddRange(helper.GenerateCdArgsFor100());
        cmArgs.AddRange(helper.GenerateCmArgsFor100());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", NewMsSqlImage, MsSqlContext, msSqlArgs, "2GB", MsSqlVolumes),
        GenerateService("solr", NewSolrImage, SolrContext, solrArgs, "1GB", SolrVolumes),
        GenerateService("cd", NewCdImage, CdContext, cdArgs, null, null),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }

    private YamlStream GenerateYamlFileFor100Xp0(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlArgs = GenerateBaseImageArgs(BaseMsSqlImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrArgs = GenerateBaseImageArgs(BaseSolrImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlArgs.AddRange(helper.GenerateMsSqlArgsFor100());
        solrArgs.AddRange(helper.GenerateSolrArgsFor100());
        cmArgs.AddRange(helper.GenerateCmArgsFor100());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", NewMsSqlImage, MsSqlContext, msSqlArgs, "2GB", MsSqlVolumes),
        GenerateService("solr", NewSolrImage, SolrContext, solrArgs, "1GB", SolrVolumes),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }

    private YamlStream GenerateYamlFileFor100Xp1(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlArgs = GenerateBaseImageArgs(BaseMsSqlImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrArgs = GenerateBaseImageArgs(BaseSolrImage);
      List<KeyValuePair<YamlNode, YamlNode>> cdArgs = GenerateBaseImageArgs(BaseCdImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlArgs.AddRange(helper.GenerateMsSqlArgsFor100());
        solrArgs.AddRange(helper.GenerateSolrArgsFor100());
        cdArgs.AddRange(helper.GenerateCdArgsFor100());
        cmArgs.AddRange(helper.GenerateCmArgsFor100());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", NewMsSqlImage, MsSqlContext, msSqlArgs, "2GB", MsSqlVolumes),
        GenerateService("solr", NewSolrImage, SolrContext, solrArgs, "1GB", SolrVolumes),
        GenerateService("cd", NewCdImage, CdContext, cdArgs, null, null),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }

    private YamlStream GenerateYamlFileFor101Xm1(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlArgs = GenerateBaseImageArgs(BaseMsSqlImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrInitArgs = GenerateBaseImageArgs(BaseSolrInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> cdArgs = GenerateBaseImageArgs(BaseCdImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlArgs.AddRange(helper.GenerateMsSqlArgsFor101());
        solrInitArgs.AddRange(helper.GenerateSolrInitArgsFor101());
        cdArgs.AddRange(helper.GenerateCdArgsFor101());
        cmArgs.AddRange(helper.GenerateCmArgsFor101());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", NewMsSqlImage, MsSqlContext, msSqlArgs, "2GB", MsSqlVolumes),
        GenerateService("solr", null, null, null, null, SolrVolumes),
        GenerateService("solr-init", NewSolrInitImage, SolrInitContext, solrInitArgs, null, null),
        GenerateService("cd", NewCdImage, CdContext, cdArgs, null, null),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }

    private YamlStream GenerateYamlFileFor101Xp0(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlArgs = GenerateBaseImageArgs(BaseMsSqlImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrInitArgs = GenerateBaseImageArgs(BaseSolrInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlArgs.AddRange(helper.GenerateMsSqlArgsFor101());
        solrInitArgs.AddRange(helper.GenerateSolrInitArgsFor101());
        cmArgs.AddRange(helper.GenerateCmArgsFor101());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", NewMsSqlImage, MsSqlContext, msSqlArgs, "2GB", MsSqlVolumes),
        GenerateService("solr", null, null, null, null, SolrVolumes),
        GenerateService("solr-init", NewSolrInitImage, SolrInitContext, solrInitArgs, null, null),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }

    private YamlStream GenerateYamlFileFor101Xp1(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlArgs = GenerateBaseImageArgs(BaseMsSqlImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrInitArgs = GenerateBaseImageArgs(BaseSolrInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> cdArgs = GenerateBaseImageArgs(BaseCdImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlArgs.AddRange(helper.GenerateMsSqlArgsFor101());
        solrInitArgs.AddRange(helper.GenerateSolrInitArgsFor101());
        cdArgs.AddRange(helper.GenerateCdArgsFor101());
        cmArgs.AddRange(helper.GenerateCmArgsFor101());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", NewMsSqlImage, MsSqlContext, msSqlArgs, "2GB", MsSqlVolumes),
        GenerateService("solr", null, null, null, null, SolrVolumes),
        GenerateService("solr-init", NewSolrInitImage, SolrInitContext, solrInitArgs, null, null),
        GenerateService("cd", NewCdImage, CdContext, cdArgs, null, null),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }

    private YamlStream GenerateYamlFileFor102Xm1(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlInitArgs = GenerateBaseImageArgs(BaseMsSqlInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrInitArgs = GenerateBaseImageArgs(BaseSolrInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> cdArgs = GenerateBaseImageArgs(BaseCdImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlInitArgs.AddRange(helper.GenerateMsSqlInitArgsFor102());
        solrInitArgs.AddRange(helper.GenerateSolrInitArgsFor102());
        cdArgs.AddRange(helper.GenerateCdArgsFor102());
        cmArgs.AddRange(helper.GenerateCmArgsFor102());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", null, null, null, "2GB", MsSqlVolumes),
        GenerateService("mssql-init", NewMsSqlInitImage, MsSqlInitContext, msSqlInitArgs, null, null),
        GenerateService("solr", null, null, null, null, SolrVolumes),
        GenerateService("solr-init", NewSolrInitImage, SolrInitContext, solrInitArgs, null, null),
        GenerateService("cd", NewCdImage, CdContext, cdArgs, null, null),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }

    private YamlStream GenerateYamlFileFor102Xp0(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlInitArgs = GenerateBaseImageArgs(BaseMsSqlInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrInitArgs = GenerateBaseImageArgs(BaseSolrInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlInitArgs.AddRange(helper.GenerateMsSqlInitArgsFor102());
        solrInitArgs.AddRange(helper.GenerateSolrInitArgsFor102());
        cmArgs.AddRange(helper.GenerateCmArgsFor102());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", null, null, null, "2GB", MsSqlVolumes),
        GenerateService("mssql-init", NewMsSqlInitImage, MsSqlInitContext, msSqlInitArgs, null, null),
        GenerateService("solr", null, null, null, null, SolrVolumes),
        GenerateService("solr-init", NewSolrInitImage, SolrInitContext, solrInitArgs, null, null),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }

    private YamlStream GenerateYamlFileFor102Xp1(List<IYamlFileGeneratorHelper> helpers)
    {
      List<KeyValuePair<YamlNode, YamlNode>> msSqlInitArgs = GenerateBaseImageArgs(BaseMsSqlInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> solrInitArgs = GenerateBaseImageArgs(BaseSolrInitImage);
      List<KeyValuePair<YamlNode, YamlNode>> cdArgs = GenerateBaseImageArgs(BaseCdImage);
      List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        msSqlInitArgs.AddRange(helper.GenerateMsSqlInitArgsFor102());
        solrInitArgs.AddRange(helper.GenerateSolrInitArgsFor102());
        cdArgs.AddRange(helper.GenerateCdArgsFor102());
        cmArgs.AddRange(helper.GenerateCmArgsFor102());
      }

      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>()
      {
        GenerateService("mssql", null, null, null, "2GB", MsSqlVolumes),
        GenerateService("mssql-init", NewMsSqlInitImage, MsSqlInitContext, msSqlInitArgs, null, null),
        GenerateService("solr", null, null, null, null, SolrVolumes),
        GenerateService("solr-init", NewSolrInitImage, SolrInitContext, solrInitArgs, null, null),
        GenerateService("cd", NewCdImage, CdContext, cdArgs, null, null),
        GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null),
      };

      return GenerateYamlFile(services);
    }
  }
}