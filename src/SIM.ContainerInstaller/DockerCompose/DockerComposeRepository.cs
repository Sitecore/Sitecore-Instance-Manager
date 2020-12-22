using SIM.ContainerInstaller.DockerCompose.Model;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using System.IO;
using Sitecore.Diagnostics.Base;
using YamlDotNet.Serialization.NamingConventions;

namespace SIM.ContainerInstaller.DockerCompose
{
  public class DockerComposeRepository : IRepository<object>
  {
    private DockerComposeModel _dockerCompose;
    private readonly string _pathToComposeFile;

    private DockerComposeModel DockerCompose
    {
      get
      {
        if (_dockerCompose == null)
        {
          _dockerCompose = DeserializeFile(_pathToComposeFile);
        }

        return _dockerCompose;
      }
    }

    public DockerComposeRepository(string pathToComposeFile)
    {
      this._pathToComposeFile = pathToComposeFile;
    }

    protected virtual DockerComposeModel DeserializeFile(string pathToComposeFile)
    {
      string yamlDocument = File.ReadAllText(pathToComposeFile);

      return DeserializeDocument(yamlDocument);
    }

    protected internal virtual DockerComposeModel DeserializeDocument(string ymlDocument)
    {
      Assert.ArgumentNotNullOrEmpty(ymlDocument, "ymlDocument");

      StringReader input = new StringReader(ymlDocument);

      IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

      var dockerComposeModel = deserializer.Deserialize<DockerComposeModel>(input);

      return dockerComposeModel;
    }

    public IDictionary<string, object> GetServices()
    {
      return DockerCompose.Services;
    }
  }
}