using JetBrains.Annotations;
using SIM.ContainerInstaller.Modules;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SIM.ContainerInstaller
{
  [UsedImplicitly]
  public class DockerfileGenerator
  {
    private const string DockerfileFileName = "Dockerfile";
    private const string IdDockerfileFilePath = @"docker\build\id";
    private const string CmDockerfileFilePath = @"docker\build\cm";
    private const string CdDockerfileFilePath = @"docker\build\cd";
    private const string MsSqlDockerfileFilePath = @"docker\build\mssql";
    private const string MsSqlInitDockerfileFilePath = @"docker\build\mssql-init";
    private const string SolrDockerfileFilePath = @"docker\build\solr";
    private const string SolrInitDockerfileFilePath = @"docker\build\solr-init";

    public void Generate(string path, List<IDockerfileGeneratorHelper> helpers, int shortVersion, Topology topology)
    {

      if (ShouldMsSqlDockerfileBeGenerated(shortVersion))
      {
        GenerateMsSqlDockerfile(path, helpers);
      }
      if (ShouldMsSqlInitDockerfileBeGenerated(shortVersion))
      {
        GenerateMsSqlInitDockerfile(path, helpers);
      }
      if (ShouldSolrDockerfileBeGenerated(shortVersion))
      {
        GenerateSolrDockerfile(path, helpers);
      }
      if (ShouldSolrInitDockerfileBeGenerated(shortVersion))
      {
        GenerateSolrInitDockerfile(path, helpers);
      }
      if (ShouldIdDockerfileBeGenerated(shortVersion, helpers))
      {
        GenerateIdDockerfile(path, helpers);
      }
      if (ShouldCdDockerfileBeGenerated(shortVersion, topology))
      {
        GenerateCdDockerfile(path, helpers);
      }
      if (ShouldCmDockerfileBeGenerated(shortVersion))
      {
        GenerateCmDockerfile(path, helpers);
      }
    }

    private bool ShouldMsSqlDockerfileBeGenerated(int shortVersion)
    {
      if (shortVersion >= 100 && shortVersion < 102)
      {
        return true;
      }
      return false;
    }

    private bool ShouldMsSqlInitDockerfileBeGenerated(int shortVersion)
    {
      if (shortVersion >= 102)
      {
        return true;
      }
      return false;
    }

    private bool ShouldSolrDockerfileBeGenerated(int shortVersion)
    {
      if (shortVersion == 100)
      {
        return true;
      }
      return false;
    }

    private bool ShouldSolrInitDockerfileBeGenerated(int shortVersion)
    {
      if (shortVersion >= 101)
      {
        return true;
      }
      return false;
    }

    private bool ShouldIdDockerfileBeGenerated(int shortVersion, List<IDockerfileGeneratorHelper> helpers)
    {
      if (shortVersion >= 100)
      {
        return true;
      }
      return false;
    }

    private bool ShouldCdDockerfileBeGenerated(int shortVersion, Topology topology)
    {
      if (shortVersion >= 100 && (topology == Topology.Xm1 || topology == Topology.Xp1))
      {
        return true;
      }
      return false;
    }

    private bool ShouldCmDockerfileBeGenerated(int shortVersion)
    {
      if (shortVersion >= 100)
      {
        return true;
      }
      return false;
    }

    private StringBuilder GenerateBaseDockerfile(StringBuilder args, StringBuilder froms, StringBuilder commands)
    {
      StringBuilder dockerfile = new StringBuilder();
      dockerfile.Append(@"# escape=`").AppendLine().AppendLine()
        .Append(@"ARG BASE_IMAGE").AppendLine();

      if (!string.IsNullOrEmpty(args.ToString()))
      {
        dockerfile.Append(args).AppendLine();
      }

      if (!string.IsNullOrEmpty(froms.ToString()))
      {
        dockerfile.Append(froms);
      }

      dockerfile.Append(@"FROM ${BASE_IMAGE}").AppendLine().AppendLine();

      if (!string.IsNullOrEmpty(commands.ToString()))
      {
        dockerfile.Append("SHELL [\"powershell\", \"-Command\", \"$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';\"]")
          .AppendLine().AppendLine().Append(commands);
      }

      return dockerfile;
    }

    public void GenerateSpecificDockerfile(string rootFolderPath, string dockerfileFilePath, StringBuilder dockerfileText)
    {
      string path = Path.Combine(rootFolderPath, dockerfileFilePath);
      CreateFolder(path);

      using (StreamWriter writer = new StreamWriter(Path.Combine(path, DockerfileFileName)))
      {
        writer.WriteLine(dockerfileText.ToString().TrimEnd());
      }
    }

    public void GenerateDockerfile(Service service, string rootFolderPath, string dockerfileFilePath, List<IDockerfileGeneratorHelper> helpers)
    {
      StringBuilder args = new StringBuilder();
      StringBuilder froms = new StringBuilder();
      StringBuilder commands = new StringBuilder();
      switch (service)
      {
        case Service.MsSql:
          foreach (IDockerfileGeneratorHelper helper in helpers)
          {
            args.Append(helper.GenerateMsSqlArgs());
            froms.Append(helper.GenerateMsSqlFroms());
            commands.Append(helper.GenerateMsSqlCommands());
          }
          break;
        case Service.MsSqlInit:
          foreach (IDockerfileGeneratorHelper helper in helpers)
          {
            args.Append(helper.GenerateMsSqlInitArgs());
            froms.Append(helper.GenerateMsSqlInitFroms());
            commands.Append(helper.GenerateMsSqlInitCommands());
          }
          break;
        case Service.Solr:
          foreach (IDockerfileGeneratorHelper helper in helpers)
          {
            args.Append(helper.GenerateSolrArgs());
            froms.Append(helper.GenerateSolrFroms());
            commands.Append(helper.GenerateSolrCommands());
          }
          break;
        case Service.SolrInit:
          foreach (IDockerfileGeneratorHelper helper in helpers)
          {
            args.Append(helper.GenerateSolrInitArgs());
            froms.Append(helper.GenerateSolrInitFroms());
            commands.Append(helper.GenerateSolrInitCommands());
          }
          break;
        case Service.Id:
          foreach (IDockerfileGeneratorHelper helper in helpers)
          {
            args.Append(helper.GenerateIdArgs());
            froms.Append(helper.GenerateIdFroms());
            commands.Append(helper.GenerateIdCommands());
          }
          break;
        case Service.Cd:
          foreach (IDockerfileGeneratorHelper helper in helpers)
          {
            args.Append(helper.GenerateCdArgs());
            froms.Append(helper.GenerateCdFroms());
            commands.Append(helper.GenerateCdCommands());
          }
          break;
        case Service.Cm:
          foreach (IDockerfileGeneratorHelper helper in helpers)
          {
            args.Append(helper.GenerateCmArgs());
            froms.Append(helper.GenerateCmFroms());
            commands.Append(helper.GenerateCmCommands());
          }
          break;
        default:
          break;
      }

      if (args.Length > 0 || froms.Length > 0 || commands.Length > 0)
      {
        GenerateSpecificDockerfile(rootFolderPath, dockerfileFilePath, GenerateBaseDockerfile(args, froms, commands));
      }
    }

    private void GenerateMsSqlDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      GenerateDockerfile(Service.MsSql, rootFolderPath, MsSqlDockerfileFilePath, helpers);
    }

    private void GenerateMsSqlInitDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      GenerateDockerfile(Service.MsSqlInit, rootFolderPath, MsSqlInitDockerfileFilePath, helpers);
    }

    private void GenerateSolrDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      GenerateDockerfile(Service.Solr, rootFolderPath, SolrDockerfileFilePath, helpers);
    }

    private void GenerateSolrInitDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      GenerateDockerfile(Service.SolrInit, rootFolderPath, SolrInitDockerfileFilePath, helpers);
    }

    private void GenerateIdDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      GenerateDockerfile(Service.Id, rootFolderPath, IdDockerfileFilePath, helpers);
    }

    private void GenerateCdDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      GenerateDockerfile(Service.Cd, rootFolderPath, CdDockerfileFilePath, helpers);
    }

    private void GenerateCmDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      GenerateDockerfile(Service.Cm, rootFolderPath, CmDockerfileFilePath, helpers);
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