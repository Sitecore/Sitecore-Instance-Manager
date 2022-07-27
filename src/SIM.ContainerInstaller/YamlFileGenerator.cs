using JetBrains.Annotations;
using SIM.ContainerInstaller.Modules;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace SIM.ContainerInstaller
{
  [UsedImplicitly]
  public class YamlFileGenerator
  {
    private const string FileVersion = "2.4";
    private const string MsSqlContext = "./docker/build/mssql";
    private const string MsSqlInitContext = "./docker/build/mssql-init";
    private const string SolrContext = "./docker/build/solr";
    private const string SolrInitContext = "./docker/build/solr-init";
    private const string IdContext = "./docker/build/id";
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
    private readonly string NewIdImage;
    private readonly string BaseIdImage;
    private readonly string NewCdImage;
    private readonly string BaseCdImage;
    private readonly string NewCmImage;
    private readonly string BaseCmImage;
    private readonly KeyValuePair<YamlNode, YamlNode> EmptyKeyValuePair;

    public YamlFileGenerator(string topology)
    {
      EmptyKeyValuePair = new KeyValuePair<YamlNode, YamlNode>();
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
      NewIdImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-id:${VERSION:-latest}";
      BaseIdImage = "${SITECORE_DOCKER_REGISTRY}sitecore-id6:${SITECORE_VERSION}";
      NewCdImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-cd:${VERSION:-latest}";
      BaseCdImage = "${SITECORE_DOCKER_REGISTRY}sitecore-" + topology + "-cd:${SITECORE_VERSION}";
      NewCmImage = "${COMPOSE_PROJECT_NAME}-" + topology + "-cm:${VERSION:-latest}";
      BaseCmImage = "${SITECORE_DOCKER_REGISTRY}sitecore-" + topology + "-cm:${SITECORE_VERSION}";
    }

    public void Generate(string path, List<IYamlFileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {
      List<KeyValuePair<YamlNode, YamlNode>> services = new List<KeyValuePair<YamlNode, YamlNode>>();
      KeyValuePair<YamlNode, YamlNode> msSqlService = GenerateMsSqlService(helpers, shortVersion, topology);
      if (msSqlService.Key != null)
      {
        services.Add(msSqlService);
      }
      KeyValuePair<YamlNode, YamlNode> msSqlInitService = GenerateMsSqlInitService(helpers, shortVersion, topology);
      if (msSqlInitService.Key != null)
      {
        services.Add(msSqlInitService);
      }
      KeyValuePair<YamlNode, YamlNode> solrService = GenerateSolrService(helpers, shortVersion, topology);
      if (solrService.Key != null)
      {
        services.Add(solrService);
      }
      KeyValuePair<YamlNode, YamlNode> solrInitService = GenerateSolrInitService(helpers, shortVersion, topology);
      if (solrInitService.Key != null)
      {
        services.Add(solrInitService);
      }
      KeyValuePair<YamlNode, YamlNode> idService = GenerateIdService(helpers, shortVersion);
      if (idService.Key != null)
      {
        services.Add(idService);
      }
      KeyValuePair<YamlNode, YamlNode> cdService = GenerateCdService(helpers, shortVersion, topology);
      if (cdService.Key != null)
      {
        services.Add(cdService);
      }
      KeyValuePair<YamlNode, YamlNode> cmService = GenerateCmService(helpers, shortVersion, topology);
      if (cmService.Key != null)
      {
        services.Add(cmService);
      }
      KeyValuePair<YamlNode, YamlNode> hrzService = GenerateHorizonService(helpers, shortVersion, topology);
      if (hrzService.Key != null)
      {
        services.Add(hrzService);
      }

      YamlDocument yamlDocument = GenerateYamlFile(services);

      if (yamlDocument != null)
      {
        Serializer serializer = new Serializer();
        using (FileStream fileStream = File.OpenWrite(Path.Combine(path, DockerSettings.DockerComposeOverrideFileName)))
        using (StreamWriter streamWriter = new StreamWriter(fileStream))
        {
          serializer.Serialize(streamWriter, yamlDocument.RootNode);
        }
      }
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateMsSqlService(List<IYamlFileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {
      if (helpers.OfType<SxaYamlFileGeneratorHelper>().Any())
      {
        if (shortVersion >= 100 && shortVersion < 102)
        {
          List<KeyValuePair<YamlNode, YamlNode>> msSqlArgs = GenerateBaseImageArgs(BaseMsSqlImage);
          foreach (IYamlFileGeneratorHelper helper in helpers)
          {
            msSqlArgs.AddRange(helper.GenerateMsSqlArgs(shortVersion, topology));
          }
          return GenerateService("mssql", NewMsSqlImage, MsSqlContext, msSqlArgs, "2GB", MsSqlVolumes);
        }
        else if (shortVersion >= 102)
        {
          return GenerateService("mssql", null, null, null, "2GB", MsSqlVolumes);
        }
      }
      return EmptyKeyValuePair;
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateMsSqlInitService(List<IYamlFileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {
      if (shortVersion >= 102 && helpers.OfType<SxaYamlFileGeneratorHelper>().Any())
      {
        List<KeyValuePair<YamlNode, YamlNode>> msSqlInitArgs = GenerateBaseImageArgs(BaseMsSqlInitImage);
        foreach (IYamlFileGeneratorHelper helper in helpers)
        {
          msSqlInitArgs.AddRange(helper.GenerateMsSqlInitArgs(shortVersion, topology));
        }
        return GenerateService("mssql-init", NewMsSqlInitImage, MsSqlInitContext, msSqlInitArgs, null, null);
      }
      return EmptyKeyValuePair;
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateSolrService(List<IYamlFileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {
      if (shortVersion == 100 && helpers.OfType<SxaYamlFileGeneratorHelper>().Any())
      {
        List<KeyValuePair<YamlNode, YamlNode>> solrArgs = GenerateBaseImageArgs(BaseSolrImage);
        foreach (IYamlFileGeneratorHelper helper in helpers)
        {
          solrArgs.AddRange(helper.GenerateSolrArgs(shortVersion, topology));
        }
        return GenerateService("solr", NewSolrImage, SolrContext, solrArgs, "1GB", SolrVolumes);
      }
      return EmptyKeyValuePair;
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateSolrInitService(List<IYamlFileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {
      if (shortVersion >= 101 && helpers.OfType<SxaYamlFileGeneratorHelper>().Any())
      {
        List<KeyValuePair<YamlNode, YamlNode>> solrInitArgs = GenerateBaseImageArgs(BaseSolrInitImage);
        foreach (IYamlFileGeneratorHelper helper in helpers)
        {
          solrInitArgs.AddRange(helper.GenerateSolrInitArgs(shortVersion, topology));
        }
        return GenerateService("solr-init", NewSolrInitImage, SolrInitContext, solrInitArgs, null, null);
      }
      return EmptyKeyValuePair;
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateIdService(List<IYamlFileGeneratorHelper> helpers, int shortVersion)
    {
      if (shortVersion >= 100 && helpers.OfType<HorizonYamlFileGeneratorHelper>().Any())
      {
        Dictionary<string, string> envenvironmentVariables = GetEnvironmentVariables(helpers, Role.Id);

        if (envenvironmentVariables != null && envenvironmentVariables.Count > 0)
        {
          List<KeyValuePair<YamlNode, YamlNode>> idArgs = GenerateBaseImageArgs(BaseIdImage);
          return GenerateService("id", NewIdImage, IdContext, idArgs, null, null, envenvironmentVariables);
        }
      }
      return EmptyKeyValuePair;
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateCdService(List<IYamlFileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {
      if (shortVersion >= 100 && (topology == Topology.Xm1 || topology == Topology.Xp1) && helpers.OfType<SxaYamlFileGeneratorHelper>().Any())
      {
        List<KeyValuePair<YamlNode, YamlNode>> cdArgs = GenerateBaseImageArgs(BaseCdImage);

        foreach (IYamlFileGeneratorHelper helper in helpers)
        {
          cdArgs.AddRange(helper.GenerateCdArgs(shortVersion, topology));
        }
        return GenerateService("cd", NewCdImage, CdContext, cdArgs, null, null);
      }
      return EmptyKeyValuePair;
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateCmService(List<IYamlFileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {
      if (shortVersion >= 100 && (helpers.OfType<SxaYamlFileGeneratorHelper>().Any() || helpers.OfType<HorizonYamlFileGeneratorHelper>().Any()))
      {
        List<KeyValuePair<YamlNode, YamlNode>> cmArgs = GenerateBaseImageArgs(BaseCmImage);

        foreach (IYamlFileGeneratorHelper helper in helpers)
        {
          cmArgs.AddRange(helper.GenerateCmArgs(shortVersion, topology));
        }

        Dictionary<string, string> envenvironmentVariables = GetEnvironmentVariables(helpers, Role.Cm);

        return GenerateService("cm", NewCmImage, CmContext, cmArgs, null, null, envenvironmentVariables);
      }
      return EmptyKeyValuePair;
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateHorizonService(List<IYamlFileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {
      if (shortVersion >= 100 && helpers.OfType<HorizonYamlFileGeneratorHelper>().Any())
      {
        HorizonYamlFileGeneratorHelper horizonYamlFileGeneratorHelper = helpers.OfType<HorizonYamlFileGeneratorHelper>().FirstOrDefault();

        List<KeyValuePair<YamlNode, YamlNode>> nodes = new List<KeyValuePair<YamlNode, YamlNode>>();

        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("image"), new YamlScalarNode(horizonYamlFileGeneratorHelper.HorizonImagePath)));
        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("isolation"), new YamlScalarNode(horizonYamlFileGeneratorHelper.IsolationNode)));

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
        foreach (KeyValuePair<string, string> envVariable in horizonYamlFileGeneratorHelper.GetHorizonEnvironmentVariables(includeSXA, includeSCCH))
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
        foreach (string label in horizonYamlFileGeneratorHelper.GetHorizonLabels())
        {
          labelsYamlSequenceNode.Add(new YamlScalarNode(label) { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted });
        }
        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("labels"), labelsYamlSequenceNode));

        return new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(DockerSettings.HorizonServiceName), new YamlMappingNode(nodes));
      }
      return EmptyKeyValuePair;
    }

    private YamlDocument GenerateYamlFile(List<KeyValuePair<YamlNode, YamlNode>> services)
    {
      return new YamlDocument(
         new YamlMappingNode(
           new YamlScalarNode("version"), new YamlScalarNode(FileVersion) { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
           new YamlScalarNode("services"), new YamlMappingNode(services)
         )
       );
    }

    private KeyValuePair<YamlNode, YamlNode> GenerateService(string service, string image = null, 
      string context = null, List<KeyValuePair<YamlNode, YamlNode>> args = null, string memoryLimit = null, 
      IEnumerable<string> volumes = null, IDictionary<string, string> environmentVariables = null)
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

      if (volumes != null && volumes.Any())
      {
        YamlSequenceNode yamlSequenceNode = new YamlSequenceNode();
        foreach (string volume in volumes)
        {
          yamlSequenceNode.Add(new YamlScalarNode(volume));
        }
        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("volumes"), yamlSequenceNode));
      }

      if (environmentVariables != null && environmentVariables.Count > 0)
      {
        YamlMappingNode yamlMappingNode = new YamlMappingNode();
        foreach (KeyValuePair<string, string> envVariable in environmentVariables)
        {
          yamlMappingNode.Add(new YamlScalarNode(envVariable.Key), new YamlScalarNode(envVariable.Value));
        }
        nodes.Add(new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode("environment"), yamlMappingNode));
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

    private Dictionary<string, string> GetEnvironmentVariables(List<IYamlFileGeneratorHelper> helpers, Role role)
    {

      Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

      foreach (IYamlFileGeneratorHelper helper in helpers)
      {
        foreach (KeyValuePair<string, string> envVariable in helper.GetEnvironmentVariables(role))
        {
          environmentVariables.Add(envVariable.Key, envVariable.Value);
        }
      }

      return environmentVariables;
    }
  }
}