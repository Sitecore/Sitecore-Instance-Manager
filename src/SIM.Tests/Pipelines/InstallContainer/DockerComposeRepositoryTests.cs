using SIM.ContainerInstaller.DockerCompose;
using SIM.ContainerInstaller.DockerCompose.Model;
using NSubstitute;
using Xunit;

namespace SIM.Tests.Pipelines.InstallContainer
{
  public class DockerComposeRepositoryTests
  {
    class DockerComposeRepositoryTest : DockerComposeRepository
    {
      public DockerComposeRepositoryTest(string pathToFile) : base(pathToFile)
      {
      }

      protected internal override DockerComposeModel DeserializeDocument(string document)
      {
        return base.DeserializeDocument(document);
      }

      public DockerComposeModel DeserialzeDoc(string document)
      {
        return this.DeserializeDocument(document);
      }
    }

    [Fact]
    public void DeserializeDoc_Should_ReturnModel()
    {
      //Arrange
      DockerComposeRepositoryTest repository = new DockerComposeRepositoryTest(Arg.Any<string>());

      //Act
      string yamlDocument = GetTestYamlDocument();

      DockerComposeModel model = repository.DeserialzeDoc(yamlDocument);

      //Assert
      Assert.NotNull(model);
    }

    [Fact]
    public void DeserializeDoc_Should_ParseProperServiceName()
    {
      //Arrange
      DockerComposeRepositoryTest repository = new DockerComposeRepositoryTest(Arg.Any<string>());

      //Act
      string yamlDocument = GetTestYamlDocument();

      DockerComposeModel model = repository.DeserialzeDoc(yamlDocument);

      //Assert
      Assert.True(model.Services.ContainsKey("cm"));
    }

    private string GetTestYamlDocument()
    {
      return @"
version: ""2.4""
services:
  traefik:
    isolation: ${ TRAEFIK_ISOLATION}
  cd:
    condition: service_healthy
  cm:
    condition: service_healthy
  redis:
    isolation: ${ ISOLATION}
  solr:
    isolation: ${ ISOLATION}
  id:
    isolation: ${ ISOLATION}
  prc:
      isolation: ${ ISOLATION}
  rep:
    isolation: ${ ISOLATION}
  xdbcollection:
    isolation: ${ ISOLATION}
  xdbsearch:
    isolation: ${ ISOLATION}
  xdbautomation:
    isolation: ${ ISOLATION}
  xdbautomationrpt:
    isolation: ${ ISOLATION}
  cortexprocessing:
    isolation: ${ ISOLATION}
  cortexreporting:
    isolation: ${ ISOLATION}
  xdbrefdata:
    isolation: ${ ISOLATION}
  xdbsearchworker:
    isolation: ${ ISOLATION}
  xdbautomationworker:
    isolation: ${ ISOLATION}
  cortexprocessingworker:
    isolation: ${ ISOLATION}
";
    }
  }
}

