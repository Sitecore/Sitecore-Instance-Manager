using System.Collections.Generic;
using System.Text;

namespace SIM.ContainerInstaller.Modules
{
  public class JssDockerfileGeneratorHelper : IDockerfileGeneratorHelper
  {
    public StringBuilder GenerateCmArgs()
    {
      return new StringBuilder()
        .Append(@"ARG JSS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCmFroms()
    {
      return new StringBuilder()
        .Append(@"FROM ${JSS_IMAGE} as jss").AppendLine();
    }

    public StringBuilder GenerateCmCommands()
    {
      return new StringBuilder()
        .Append(@"# Add JSS module").AppendLine()
        .Append(@"COPY --from=jss C:\module\cm\content C:\inetpub\wwwroot").AppendLine()
        .Append(@"COPY --from=jss C:\module\tools C:\module\tools").AppendLine()
        .Append(@"RUN C:\module\tools\Initialize-Content.ps1 -TargetPath C:\inetpub\wwwroot; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\module -Recurse -Force;").AppendLine().AppendLine();
    }

    public StringBuilder GenerateCdArgs()
    {
      return new StringBuilder().Append(@"ARG JSS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCdFroms()
    {
      return new StringBuilder().Append(@"FROM ${JSS_IMAGE} as jss").AppendLine();
    }

    public StringBuilder GenerateCdCommands()
    {
      return new StringBuilder()
        .Append(@"# Add JSS module").AppendLine()
        .Append(@"COPY --from=jss C:\module\cd\content C:\inetpub\wwwroot").AppendLine()
        .Append(@"COPY --from=jss C:\module\tools C:\module\tools").AppendLine()
        .Append(@"RUN C:\module\tools\Initialize-Content.ps1 -TargetPath C:\inetpub\wwwroot; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\module -Recurse -Force;").AppendLine().AppendLine();
    }

    public StringBuilder GenerateIdArgs()
    {
      return null;
    }

    public StringBuilder GenerateIdFroms()
    {
      return null;
    }

    public StringBuilder GenerateIdCommands()
    {
      return null;
    }

    public StringBuilder GenerateMsSqlArgs()
    {
      return new StringBuilder()
        .Append(@"ARG JSS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlFroms()
    {
      return new StringBuilder()
        .Append(@"FROM ${JSS_IMAGE} as jss").AppendLine();
    }

    public StringBuilder GenerateMsSqlCommands()
    {
      return new StringBuilder()
        .Append(@"# Add JSS module").AppendLine()
        .Append(@"COPY --from=jss C:\module\db C:\jss_data").AppendLine()
        .Append(@"RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\jss_data; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\jss_data -Recurse -Force;").AppendLine().AppendLine();
    }

    public StringBuilder GenerateMsSqlInitArgs()
    {
      return new StringBuilder().Append(@"ARG JSS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlInitFroms()
    {
      return new StringBuilder().Append(@"FROM ${JSS_IMAGE} as jss").AppendLine();
    }

    public StringBuilder GenerateMsSqlInitCommands()
    {
      return new StringBuilder()
        .Append(@"# Add JSS module").AppendLine()
        .Append(@"COPY --from=jss C:\module\db C:\resources\jss").AppendLine().AppendLine();
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

    public IDictionary<string, StringBuilder> GenerateDockerfiles()
    {
      return null;
    }
  }
}