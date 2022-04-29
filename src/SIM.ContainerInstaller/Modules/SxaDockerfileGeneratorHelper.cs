using System.Text;

namespace SIM.ContainerInstaller.Modules
{
  public class SxaDockerfileGeneratorHelper : IDockerfileGeneratorHelper
  {
    public StringBuilder GenerateCmArgs()
    {
      return new StringBuilder().Append(@"ARG SXA_IMAGE").AppendLine()
        .Append(@"ARG SPE_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCmFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPE_IMAGE} as spe").AppendLine()
        .Append(@"FROM ${SXA_IMAGE} as sxa").AppendLine();
    }

    public StringBuilder GenerateCmCommands()
    {
      return new StringBuilder().Append(@"WORKDIR C:\inetpub\wwwroot").AppendLine().AppendLine()
        .Append(@"# Add SPE module").AppendLine()
        .Append(@"COPY --from=spe \module\cm\content .\").AppendLine().AppendLine()
        .Append(@"# Add SXA module").AppendLine()
        .Append(@"COPY --from=sxa \module\cm\content .\").AppendLine()
        .Append(@"COPY --from=sxa \module\tools \module\tools").AppendLine()
        .Append(@"RUN C:\module\tools\Initialize-Content.ps1 -TargetPath .\; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\module -Recurse -Force;");
    }

    public StringBuilder GenerateCdArgs()
    {
      return new StringBuilder().Append(@"ARG SXA_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCdFroms()
    {
      return new StringBuilder().Append(@"FROM ${SXA_IMAGE} as sxa").AppendLine();
    }

    public StringBuilder GenerateCdCommands()
    {
      return new StringBuilder().Append(@"WORKDIR C:\inetpub\wwwroot").AppendLine().AppendLine()
        .Append(@"# Add SXA module").AppendLine()
        .Append(@"COPY --from=sxa \module\cd\content .\").AppendLine()
        .Append(@"COPY --from=sxa \module\tools \module\tools").AppendLine()
        .Append(@"RUN C:\module\tools\Initialize-Content.ps1 -TargetPath .\; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\module -Recurse -Force;");
    }

    public StringBuilder GenerateMsSqlArgs()
    {
      return new StringBuilder().Append(@"ARG SXA_IMAGE").AppendLine()
        .Append(@"ARG SPE_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPE_IMAGE} as spe").AppendLine()
        .Append(@"FROM ${SXA_IMAGE} as sxa").AppendLine();
    }

    public StringBuilder GenerateMsSqlCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPE module").AppendLine()
        .Append(@"COPY --from=spe \module\db \spe_data").AppendLine()
        .Append(@"RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\spe_data; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\spe_data -Recurse -Force;").AppendLine().AppendLine()
        .Append(@"# Add SXA module").AppendLine()
        .Append(@"COPY --from=sxa \module\db \sxa_data").AppendLine()
        .Append(@"RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\sxa_data; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\sxa_data -Recurse -Force;");
    }

    public StringBuilder GenerateMsSqlInitArgs()
    {
      return new StringBuilder().Append(@"ARG SPE_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlInitFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPE_IMAGE} as spe").AppendLine();
    }

    public StringBuilder GenerateMsSqlInitCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPE module").AppendLine()
        .Append(@"COPY --from=spe C:\module\db C:\resources\spe");
    }

    public StringBuilder GenerateSolrArgs()
    {
      return new StringBuilder().Append(@"ARG SXA_IMAGE").AppendLine();
    }

    public StringBuilder GenerateSolrFroms()
    {
      return new StringBuilder().Append(@"FROM ${SXA_IMAGE} as sxa").AppendLine();
    }

    public StringBuilder GenerateSolrCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SXA module").AppendLine()
        .Append(@"COPY --from=sxa \module\solr \sxa_data").AppendLine()
        .Append(@"RUN C:\Add-SolrCores.ps1 -SolrPath C:\solr -SolrSchemaPath C:\sxa_data\managed-schema -SolrCoreNames 'sitecore_sxa_master_index,sitecore_sxa_web_index'; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\sxa_data -Recurse -Force;");
    }

    public StringBuilder GenerateSolrInitArgs()
    {
      return new StringBuilder().Append(@"ARG SXA_IMAGE").AppendLine();
    }

    public StringBuilder GenerateSolrInitFroms()
    {
      return new StringBuilder().Append(@"FROM ${SXA_IMAGE} as sxa").AppendLine();
    }

    public StringBuilder GenerateSolrInitCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SXA module").AppendLine()
        .Append(@"COPY --from=sxa C:\module\solr\cores-sxa.json C:\data\cores-sxa.json");
    }
  }
}