using YamlDotNet.RepresentationModel;
using static SIM.ContainerInstaller.DockerfileGenerator;

namespace SIM.ContainerInstaller.Modules
{
  public interface IModuleHelper
  {
    YamlStream GenerateOverrideYamlFile();

    void GenerateDockerfile(string path, Role role);
  }
}