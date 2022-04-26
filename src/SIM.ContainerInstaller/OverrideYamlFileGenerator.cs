using JetBrains.Annotations;
using SIM.ContainerInstaller.Modules;
using System.IO;

namespace SIM.ContainerInstaller
{
  [UsedImplicitly]
  public class OverrideYamlFileGenerator
  {
    public const string dockerComposeFileName = "docker-compose.override.yml";

    public void Generate(string path, IModuleHelper moduleHelper)
    {
      using (StreamWriter writer = new StreamWriter(Path.Combine(path, dockerComposeFileName)))
      {
        moduleHelper.GenerateOverrideYamlFile().Save(writer, false);
      }
    }
  }
}