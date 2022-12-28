using System.Collections.Generic;
using System.Text;

namespace SIM.ContainerInstaller.Modules
{
  public class SpsDockerfileGeneratorHelper : IDockerfileGeneratorHelper
  {
    private const string SpsDockerfileFilePath = @"docker\build\sps";

    public StringBuilder GenerateCmArgs()
    {
      return new StringBuilder().Append(@"ARG SPS_ASSETS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCmFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPS_ASSETS_IMAGE} as sps_assets").AppendLine();
    }

    public StringBuilder GenerateCmCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPS module").AppendLine()
        .Append(@"COPY --from=sps_assets C:\module\cm\content C:\inetpub\wwwroot").AppendLine()
         //.Append(@"# If you are running Sitecore Publishing Service 6.1").AppendLine()
         //.Append(@"RUN Remove-Item -Path C:\inetpub\wwwroot\App_Config\Modules\PublishingService\Sitecore.Publishing.Service.SingleLinkDatabase.config -Force;")
        .AppendLine().AppendLine();
    }

    public StringBuilder GenerateCdArgs()
    {
      return new StringBuilder().Append(@"ARG SPS_ASSETS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateCdFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPS_ASSETS_IMAGE} as sps_assets").AppendLine();
    }

    public StringBuilder GenerateCdCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPS module").AppendLine()
        .Append(@"COPY --from=sps_assets C:\module\cd\content C:\inetpub\wwwroot").AppendLine()
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
      return new StringBuilder().Append(@"ARG SPS_ASSETS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPS_ASSETS_IMAGE} as sps_assets").AppendLine();
    }

    public StringBuilder GenerateMsSqlCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPS module").AppendLine()
        .Append(@"COPY --from=sps_assets C:\module\db C:\sps_data").AppendLine()
        .Append(@"RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\sps_data; `").AppendLine()
        .Append(@"    Remove-Item -Path C:\sps_data -Recurse -Force;").AppendLine().AppendLine();
    }

    public StringBuilder GenerateMsSqlInitArgs()
    {
      return new StringBuilder().Append(@"ARG SPS_ASSETS_IMAGE").AppendLine();
    }

    public StringBuilder GenerateMsSqlInitFroms()
    {
      return new StringBuilder().Append(@"FROM ${SPS_ASSETS_IMAGE} as sps_assets").AppendLine();
    }

    public StringBuilder GenerateMsSqlInitCommands()
    {
      return new StringBuilder()
        .Append(@"# Add SPS module").AppendLine()
        .Append(@"COPY --from=sps_assets C:\module\db C:\resources\sps").AppendLine().AppendLine();
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
      Dictionary<string, StringBuilder> dockerfiles = new Dictionary<string, StringBuilder>();
      dockerfiles.Add(SpsDockerfileFilePath, GenerateSpsDockerfile());
      return dockerfiles;
    }

    private StringBuilder GenerateSpsDockerfile()
    {
      return new StringBuilder().Append(@"# escape=`").AppendLine().AppendLine()
        .Append(@"ARG SPS_IMAGE").AppendLine()
        .Append(@"ARG SITECORE_CM_IMAGE").AppendLine().AppendLine()
        .Append(@"FROM ${SITECORE_CM_IMAGE} as sitecore").AppendLine().AppendLine()
        .Append(@"RUN mkdir C:/res_files").AppendLine()
        .Append(@"WORKDIR C:/res_files").AppendLine().AppendLine()
        .Append(@"RUN Copy-Item C:/inetpub/wwwroot/App_Data/items/master sitecore/master -Recurse -Force; `").AppendLine()
        .Append(@"Copy-Item C:/inetpub/wwwroot/App_Data/items/web sitecore/web -Recurse -Force; `").AppendLine()
        .Append("if(Test-Path -Path \\\"C:/inetpub/wwwroot/sitecore modules/items/master\\\") `").AppendLine()
        .Append(@"{ `").AppendLine()
        .Append("Copy-Item \\\"C:/inetpub/wwwroot/sitecore modules/items/master\\\" modules/master -Recurse -Force; `").AppendLine()
        .Append(@"} `").AppendLine()
        .Append("if(Test-Path -Path \\\"C:/inetpub/wwwroot/sitecore modules/items/web\\\") `").AppendLine()
        .Append(@"{ `").AppendLine()
        .Append("Copy-Item \\\"C:/inetpub/wwwroot/sitecore modules/items/web\\\" modules/web -Recurse -Force; `").AppendLine()
        .Append(@"}").AppendLine().AppendLine()
        .Append(@"FROM ${SPS_IMAGE} as base").AppendLine()
        .Append(@"COPY --from=sitecore C:/res_files/ C:/sps/items/");
    }
  }
}