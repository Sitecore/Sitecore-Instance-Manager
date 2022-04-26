using JetBrains.Annotations;
using SIM.ContainerInstaller.Modules;
using System.IO;

namespace SIM.ContainerInstaller
{
  [UsedImplicitly]
  public class DockerfileGenerator
  {
    private const string dockerfileFileName = "Dockerfile";

    public enum Role
    {
      cm,
      cd,
      mssql,
      solrinit
    }

    public void Generate(string path, IModuleHelper moduleHelper)
    {
      string cmPath = Path.Combine(path, @"docker\build\cm");
      CreateFolder(cmPath);
      moduleHelper.GenerateDockerfile(Path.Combine(cmPath, dockerfileFileName), Role.cm);

      string cdPath = Path.Combine(path, @"docker\build\cd");
      CreateFolder(cdPath);
      moduleHelper.GenerateDockerfile(Path.Combine(cdPath, dockerfileFileName), Role.cd);

      string mssqlPath = Path.Combine(path, @"docker\build\mssql");
      CreateFolder(mssqlPath);
      moduleHelper.GenerateDockerfile(Path.Combine(mssqlPath, dockerfileFileName), Role.mssql);

      string solrInitPath = Path.Combine(path, @"docker\build\solr-init");
      CreateFolder(solrInitPath);
      moduleHelper.GenerateDockerfile(Path.Combine(solrInitPath, dockerfileFileName), Role.solrinit);
    }

    private void CreateFolder(string path)
    {
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
    }
  }
}