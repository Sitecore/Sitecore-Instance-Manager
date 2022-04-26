using System.IO;
using YamlDotNet.RepresentationModel;
using static SIM.ContainerInstaller.DockerfileGenerator;

namespace SIM.ContainerInstaller.Modules
{
  public class SXAHelper : IModuleHelper
  {
    public YamlStream GenerateOverrideYamlFile()
    {
      return new YamlStream(
        new YamlDocument(
          new YamlMappingNode(
            new YamlScalarNode("version"), new YamlScalarNode("2.4") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
            new YamlScalarNode("services"), new YamlMappingNode(
              new YamlScalarNode("mssql"), new YamlMappingNode(
                new YamlScalarNode("image"), new YamlScalarNode("${COMPOSE_PROJECT_NAME}-xm1-mssql:${VERSION:-latest}"),
                new YamlScalarNode("build"), new YamlMappingNode(
                  new YamlScalarNode("context"), new YamlScalarNode("./docker/build/mssql"),
                  new YamlScalarNode("args"), new YamlMappingNode(
                    new YamlScalarNode("BASE_IMAGE"), new YamlScalarNode("${SITECORE_DOCKER_REGISTRY}sitecore-xm1-mssql:${SITECORE_VERSION}"),
                    new YamlScalarNode("SPE_IMAGE"), new YamlScalarNode("${SITECORE_MODULE_REGISTRY}spe-assets:${SPE_VERSION}"),
                    new YamlScalarNode("SXA_IMAGE"), new YamlScalarNode("${SITECORE_MODULE_REGISTRY}sxa-xm1-assets:${SXA_VERSION}")
                  )
                ),
                new YamlScalarNode("mem_limit"), new YamlScalarNode("2GB"),
                new YamlScalarNode("volumes"), new YamlSequenceNode(
                  new YamlScalarNode(".\\mssql-data:c:\\data")
                )
              ),
              new YamlScalarNode("solr"), new YamlMappingNode(
                new YamlScalarNode("volumes"), new YamlSequenceNode(
                  new YamlScalarNode(".\\solr-data:c:\\data")
                )
              ),
              new YamlScalarNode("solr-init"), new YamlMappingNode(
                new YamlScalarNode("image"), new YamlScalarNode("${COMPOSE_PROJECT_NAME}-xm1-solr-init:${VERSION:-latest}"),
                new YamlScalarNode("build"), new YamlMappingNode(
                  new YamlScalarNode("context"), new YamlScalarNode("./docker/build/solr-init"),
                  new YamlScalarNode("args"), new YamlMappingNode(
                    new YamlScalarNode("BASE_IMAGE"), new YamlScalarNode("${SITECORE_DOCKER_REGISTRY}sitecore-xm1-solr-init:${SITECORE_VERSION}"),
                    new YamlScalarNode("SXA_IMAGE"), new YamlScalarNode("${SITECORE_MODULE_REGISTRY}sxa-xm1-assets:${SXA_VERSION}")
                  )
                )
              ),
              new YamlScalarNode("cd"), new YamlMappingNode(
                new YamlScalarNode("image"), new YamlScalarNode("${COMPOSE_PROJECT_NAME}-xm1-cd:${VERSION:-latest}"),
                new YamlScalarNode("build"), new YamlMappingNode(
                  new YamlScalarNode("context"), new YamlScalarNode("./docker/build/cd"),
                  new YamlScalarNode("args"), new YamlMappingNode(
                    new YamlScalarNode("BASE_IMAGE"), new YamlScalarNode("${SITECORE_DOCKER_REGISTRY}sitecore-xm1-cd:${SITECORE_VERSION}"),
                    new YamlScalarNode("SXA_IMAGE"), new YamlScalarNode("${SITECORE_MODULE_REGISTRY}sxa-xm1-assets:${SXA_VERSION}"),
                    new YamlScalarNode("TOOLING_IMAGE"), new YamlScalarNode("${SITECORE_TOOLS_REGISTRY}sitecore-docker-tools-assets:${TOOLS_VERSION}")
                  )
                ),
                new YamlScalarNode("environment"), new YamlMappingNode(
                  new YamlScalarNode("SITECORE_DEVELOPMENT_PATCHES"), new YamlScalarNode("CustomErrorsOff")
                )
              ),
              new YamlScalarNode("cm"), new YamlMappingNode(
                new YamlScalarNode("image"), new YamlScalarNode("${COMPOSE_PROJECT_NAME}-xm1-cm:${VERSION:-latest}"),
                new YamlScalarNode("build"), new YamlMappingNode(
                  new YamlScalarNode("context"), new YamlScalarNode("./docker/build/cm"),
                  new YamlScalarNode("args"), new YamlMappingNode(
                    new YamlScalarNode("BASE_IMAGE"), new YamlScalarNode("${SITECORE_DOCKER_REGISTRY}sitecore-xm1-cm:${SITECORE_VERSION}"),
                    new YamlScalarNode("SPE_IMAGE"), new YamlScalarNode("${SITECORE_MODULE_REGISTRY}spe-assets:${SPE_VERSION}"),
                    new YamlScalarNode("SXA_IMAGE"), new YamlScalarNode("${SITECORE_MODULE_REGISTRY}sxa-xm1-assets:${SXA_VERSION}"),
                    new YamlScalarNode("TOOLING_IMAGE"), new YamlScalarNode("${SITECORE_TOOLS_REGISTRY}sitecore-docker-tools-assets:${TOOLS_VERSION}")
                  )
                ),
                new YamlScalarNode("environment"), new YamlMappingNode(
                  new YamlScalarNode("SITECORE_DEVELOPMENT_PATCHES"), new YamlScalarNode("CustomErrorsOff")
                )
              )
            )
          )
        )
      );
    }

    public void GenerateDockerfile(string path, Role role)
    {
      using (StreamWriter writer = new StreamWriter(path))
      {
        switch (role)
        {
          case Role.cm:
            writer.WriteLine(@"# escape=`");
            writer.WriteLine();
            writer.WriteLine(@"ARG BASE_IMAGE");
            writer.WriteLine(@"ARG SXA_IMAGE");
            writer.WriteLine(@"ARG SPE_IMAGE");
            writer.WriteLine(@"ARG TOOLING_IMAGE");
            writer.WriteLine();
            writer.WriteLine(@"FROM ${TOOLING_IMAGE} as tooling");
            writer.WriteLine(@"FROM ${SPE_IMAGE} as spe");
            writer.WriteLine(@"FROM ${SXA_IMAGE} as sxa");
            writer.WriteLine(@"FROM ${BASE_IMAGE}");
            writer.WriteLine();
            writer.WriteLine("SHELL [\"powershell\", \"-Command\", \"$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';\"]");
            writer.WriteLine();
            writer.WriteLine(@"# Copy development tools and entrypoint");
            writer.WriteLine(@"COPY --from=tooling \tools\ \tools\");
            writer.WriteLine();
            writer.WriteLine(@"WORKDIR C:\inetpub\wwwroot");
            writer.WriteLine();
            writer.WriteLine(@"# Add SPE module");
            writer.WriteLine(@"COPY --from=spe \module\cm\content .\");
            writer.WriteLine();
            writer.WriteLine(@"# Add SXA module");
            writer.WriteLine(@"COPY --from=sxa \module\cm\content .\");
            writer.WriteLine(@"COPY --from=sxa \module\tools \module\tools");
            writer.WriteLine(@"RUN C:\module\tools\Initialize-Content.ps1 -TargetPath .\; `");
            writer.WriteLine(@"    Remove-Item -Path C:\module -Recurse -Force;");
            break;
          case Role.cd:
            writer.WriteLine(@"# escape=`");
            writer.WriteLine();
            writer.WriteLine(@"ARG BASE_IMAGE");
            writer.WriteLine(@"ARG SXA_IMAGE");
            writer.WriteLine(@"ARG TOOLING_IMAGE");
            writer.WriteLine();
            writer.WriteLine(@"FROM ${TOOLING_IMAGE} as tooling");
            writer.WriteLine(@"FROM ${SXA_IMAGE} as sxa");
            writer.WriteLine(@"FROM ${BASE_IMAGE}");
            writer.WriteLine();
            writer.WriteLine("SHELL [\"powershell\", \"-Command\", \"$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';\"]");
            writer.WriteLine();
            writer.WriteLine(@"# Copy development tools and entrypoint");
            writer.WriteLine(@"COPY --from=tooling \tools\ \tools\");
            writer.WriteLine();
            writer.WriteLine(@"WORKDIR C:\inetpub\wwwroot");
            writer.WriteLine();
            writer.WriteLine(@"# Add SXA module");
            writer.WriteLine(@"COPY --from=sxa \module\cd\content .\");
            writer.WriteLine(@"COPY --from=sxa \module\tools \module\tools");
            writer.WriteLine(@"RUN C:\module\tools\Initialize-Content.ps1 -TargetPath .\; `");
            writer.WriteLine(@"    Remove-Item -Path C:\module -Recurse -Force;");
            break;
          case Role.mssql:
            writer.WriteLine(@"# escape=`");
            writer.WriteLine();
            writer.WriteLine(@"ARG BASE_IMAGE");
            writer.WriteLine(@"ARG SXA_IMAGE");
            writer.WriteLine(@"ARG SPE_IMAGE");
            writer.WriteLine();
            writer.WriteLine(@"FROM ${SPE_IMAGE} as spe");
            writer.WriteLine(@"FROM ${SXA_IMAGE} as sxa");
            writer.WriteLine(@"FROM ${BASE_IMAGE}");
            writer.WriteLine();
            writer.WriteLine("SHELL [\"powershell\", \"-Command\", \"$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';\"]");
            writer.WriteLine();
            writer.WriteLine(@"# Add SPE module");
            writer.WriteLine(@"COPY --from=spe \module\db \spe_data");
            writer.WriteLine(@"RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\spe_data; `");
            writer.WriteLine(@"    Remove-Item -Path C:\spe_data -Recurse -Force;");
            writer.WriteLine();
            writer.WriteLine(@"# Add SXA module");
            writer.WriteLine(@"COPY --from=sxa \module\db \sxa_data");
            writer.WriteLine(@"RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\sxa_data; `");
            writer.WriteLine(@"    Remove-Item -Path C:\sxa_data -Recurse -Force;");
            break;
          case Role.solrinit:
            writer.WriteLine(@"# escape=`");
            writer.WriteLine();
            writer.WriteLine(@"ARG BASE_IMAGE");
            writer.WriteLine(@"ARG SXA_IMAGE");
            writer.WriteLine();
            writer.WriteLine(@"FROM ${SXA_IMAGE} as sxa");
            writer.WriteLine(@"FROM ${BASE_IMAGE}");
            writer.WriteLine();
            writer.WriteLine("SHELL [\"powershell\", \"-Command\", \"$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';\"]");
            writer.WriteLine();
            writer.WriteLine(@"# Add SXA module");
            writer.WriteLine(@"COPY --from=sxa C:\module\solr\cores-sxa.json C:\data\cores-sxa.json");
            break;
          default:
            break;
        }
      }
    }
  }
}