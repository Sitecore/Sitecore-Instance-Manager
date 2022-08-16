using System.Text;

namespace SIM.ContainerInstaller.Modules
{
  public interface IDockerfileGeneratorHelper
  {
    StringBuilder GenerateCmArgs();

    StringBuilder GenerateCmFroms();

    StringBuilder GenerateCmCommands();

    StringBuilder GenerateCdArgs();

    StringBuilder GenerateCdFroms();

    StringBuilder GenerateCdCommands();

    StringBuilder GenerateMsSqlArgs();

    StringBuilder GenerateMsSqlFroms();

    StringBuilder GenerateMsSqlCommands();

    StringBuilder GenerateMsSqlInitArgs();

    StringBuilder GenerateMsSqlInitFroms();

    StringBuilder GenerateMsSqlInitCommands();

    StringBuilder GenerateSolrArgs();

    StringBuilder GenerateSolrFroms();

    StringBuilder GenerateSolrCommands();

    StringBuilder GenerateSolrInitArgs();

    StringBuilder GenerateSolrInitFroms();

    StringBuilder GenerateSolrInitCommands();
  }
}