using System.Text;

namespace SIM.ContainerInstaller.Modules
{
  public class HorizonDockerfileGeneratorHelper : IDockerfileGeneratorHelper
  {
    public StringBuilder GenerateCmArgs()
    {
      return new StringBuilder().Append(@"ARG HORIZON_RESOURCES_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCmFroms()
    {
      return new StringBuilder().Append(@"FROM ${HORIZON_RESOURCES_IMAGE} as horizon_resources").AppendLine();
    }

    public StringBuilder GenerateCmCommands()
    {
      return new StringBuilder().Append(@"WORKDIR C:\inetpub\wwwroot").AppendLine().AppendLine()
        .Append(@"# Add Horizon module").AppendLine()
        .Append(@"COPY --from=horizon_resources \module\cm\content .\").AppendLine().AppendLine();
    }

    public StringBuilder GenerateCdArgs()
    {
      return null;
    }

    public StringBuilder GenerateCdFroms()
    {
      return null;
    }

    public StringBuilder GenerateCdCommands()
    {
      return null;
    }

    public StringBuilder GenerateMsSqlArgs()
    {
      return null;
    }

    public StringBuilder GenerateMsSqlFroms()
    {
      return null;
    }

    public StringBuilder GenerateMsSqlCommands()
    {
      return null;
    }

    public StringBuilder GenerateMsSqlInitArgs()
    {
      return null;
    }

    public StringBuilder GenerateMsSqlInitFroms()
    {
      return null;
    }

    public StringBuilder GenerateMsSqlInitCommands()
    {
      return null;
    }

    public StringBuilder GenerateSolrArgs()
    {
      return null;
    }

    public StringBuilder GenerateSolrFroms()
    {
      return null;
    }

    public StringBuilder GenerateSolrCommands()
    {
      return null;
    }

    public StringBuilder GenerateSolrInitArgs()
    {
      return null;
    }

    public StringBuilder GenerateSolrInitFroms()
    {
      return null;
    }

    public StringBuilder GenerateSolrInitCommands()
    {
      return null;
    }
  }
}