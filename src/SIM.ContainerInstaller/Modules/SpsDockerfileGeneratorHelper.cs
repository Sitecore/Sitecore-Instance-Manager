using System.Text;

namespace SIM.ContainerInstaller.Modules
{
  public class SpsDockerfileGeneratorHelper : IDockerfileGeneratorHelper
  {
    public StringBuilder GenerateCmArgs()
    {
      return new StringBuilder().Append(@"ARG SPS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCmFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPS_IMAGE} as sps").AppendLine();
    }

    public StringBuilder GenerateCmCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPS module").AppendLine()
        .Append(@"COPY --from=sps C:\module\cm\content C:\inetpub\wwwroot").AppendLine()
         //.Append(@"# If you are running Sitecore Publishing Service 6.1").AppendLine()
         //.Append(@"RUN Remove-Item -Path C:\inetpub\wwwroot\App_Config\Modules\PublishingService\Sitecore.Publishing.Service.SingleLinkDatabase.config -Force;")
        .AppendLine().AppendLine();
    }

    public StringBuilder GenerateCdArgs()
    {
      return new StringBuilder().Append(@"ARG SPS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCdFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPS_IMAGE} as sps").AppendLine();
    }

    public StringBuilder GenerateCdCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPS module").AppendLine()
        .Append(@"COPY --from=sps C:\module\cd\content C:\inetpub\wwwroot").AppendLine()
        //.Append(@"# If you are running Sitecore Publishing Service 6.1").AppendLine()
        //.Append(@"RUN Remove-Item -Path C:\inetpub\wwwroot\App_Config\Modules\PublishingService\Sitecore.Publishing.Service.SingleLinkDatabase.config -Force;")
        .AppendLine().AppendLine();
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
      return new StringBuilder().Append(@"ARG SPS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPS_IMAGE} as sps").AppendLine();
    }

    public StringBuilder GenerateMsSqlCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPS module").AppendLine()
        .Append(@"COPY --from=sps C:\module\db C:\sps_data").AppendLine()
        .Append(@"RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\sps_data; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\sps_data -Recurse -Force;").AppendLine().AppendLine();
    }

    public StringBuilder GenerateMsSqlInitArgs()
    {
      return new StringBuilder().Append(@"ARG SPS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlInitFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPS_IMAGE} as sps").AppendLine();
    }

    public StringBuilder GenerateMsSqlInitCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPS module").AppendLine()
        .Append(@"COPY --from=sps C:\module\db C:\resources\sps").AppendLine().AppendLine();
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