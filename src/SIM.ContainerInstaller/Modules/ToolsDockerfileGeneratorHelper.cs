using System.Text;

namespace SIM.ContainerInstaller.Modules
{
  public class ToolsDockerfileGeneratorHelper : IDockerfileGeneratorHelper
  {
    public StringBuilder GenerateCmArgs()
    {
      return new StringBuilder().Append(@"ARG TOOLING_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCmFroms()
    {
      return new StringBuilder().Append(@"FROM ${TOOLING_IMAGE} as tooling").AppendLine();
    }

    public StringBuilder GenerateCmCommands()
    {
      return new StringBuilder().Append(@"# Copy development tools and entrypoint").AppendLine().Append(@"COPY --from=tooling \tools\ \tools\").AppendLine().AppendLine();
    }

    public StringBuilder GenerateCdArgs()
    {
      return new StringBuilder().Append(@"ARG TOOLING_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCdFroms()
    {
      return new StringBuilder().Append(@"FROM ${TOOLING_IMAGE} as tooling").AppendLine();
    }

    public StringBuilder GenerateCdCommands()
    {
      return new StringBuilder().Append(@"# Copy development tools and entrypoint").AppendLine().Append(@"COPY --from=tooling \tools\ \tools\").AppendLine().AppendLine();
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