using System.Text;

namespace SIM.ContainerInstaller.Modules
{
  public class HorizonDockerfileGeneratorHelper : IDockerfileGeneratorHelper
  {
    public StringBuilder GenerateCmArgs()
    {
      return new StringBuilder().Append(@"ARG HORIZON_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCmFroms()
    {
      return new StringBuilder().Append(@"FROM ${HORIZON_IMAGE} as horizon").AppendLine();
    }

    public StringBuilder GenerateCmCommands()
    {
      return new StringBuilder()
        .Append(@"# Add Horizon module").AppendLine()
        .Append(@"COPY --from=horizon C:\module\cm\content C:\inetpub\wwwroot").AppendLine().AppendLine();
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

    public StringBuilder GenerateIdArgs()
    {
      return null;
    }

    public StringBuilder GenerateIdFroms()
    {
      return new StringBuilder()
        .Append($@"# This Dockerfile is only needed to add environment variables defined in {DockerSettings.DockerComposeOverrideFileName}").AppendLine();
    }

    public StringBuilder GenerateIdCommands()
    {
      return null;
    }

    public StringBuilder GenerateMsSqlArgs()
    {
      return new StringBuilder().Append(@"ARG HORIZON_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlFroms()
    {
      return new StringBuilder().Append(@"FROM ${HORIZON_IMAGE} as horizon").AppendLine();
    }

    public StringBuilder GenerateMsSqlCommands()
    {
      return new StringBuilder()
        .Append(@"# Add Horizon module").AppendLine()
        .Append(@"COPY --from=horizon C:\module\db C:\horizon_integration_data").AppendLine()
        .Append(@"RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\horizon_integration_data; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\horizon_integration_data -Recurse -Force;").AppendLine().AppendLine();
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