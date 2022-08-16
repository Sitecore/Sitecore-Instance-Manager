using JetBrains.Annotations;
using SIM.ContainerInstaller.Modules;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SIM.ContainerInstaller
{
  [UsedImplicitly]
  public class DockerfileGenerator
  {
    private const string DockerfileFileName = "Dockerfile";
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

    private StringBuilder GenerateDockerfile(StringBuilder args, StringBuilder froms, StringBuilder commands)
    {
      return new StringBuilder().Append(@"# escape=`").AppendLine().AppendLine()
        .Append(@"ARG BASE_IMAGE").AppendLine()
        .Append(args).AppendLine()
        .Append(froms)
        .Append(@"FROM ${BASE_IMAGE}").AppendLine().AppendLine()
        .Append("SHELL [\"powershell\", \"-Command\", \"$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';\"]").AppendLine().AppendLine()
        .Append(commands);
    }

    public void GenerateSpecificDockerfile(string rootFolderPath, string dockerfileFilePath, StringBuilder dockerfileText)
    {
      string path = Path.Combine(rootFolderPath, dockerfileFilePath);
      CreateFolder(path);

      using (StreamWriter writer = new StreamWriter(Path.Combine(path, DockerfileFileName)))
      {
        writer.WriteLine(dockerfileText);
      }
    }

    private void GenerateMsSqlDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      StringBuilder args = new StringBuilder();
      StringBuilder froms = new StringBuilder();
      StringBuilder commands = new StringBuilder();
      foreach (IDockerfileGeneratorHelper helper in helpers)
      {
        args.Append(helper.GenerateMsSqlArgs());
        froms.Append(helper.GenerateMsSqlFroms());
        commands.Append(helper.GenerateMsSqlCommands());
      }
      GenerateSpecificDockerfile(rootFolderPath, MsSqlDockerfileFilePath, GenerateDockerfile(args, froms, commands));
    }

    private void GenerateMsSqlInitDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      StringBuilder args = new StringBuilder();
      StringBuilder froms = new StringBuilder();
      StringBuilder commands = new StringBuilder();
      foreach (IDockerfileGeneratorHelper helper in helpers)
      {
        args.Append(helper.GenerateMsSqlInitArgs());
        froms.Append(helper.GenerateMsSqlInitFroms());
        commands.Append(helper.GenerateMsSqlInitCommands());
      }
      GenerateSpecificDockerfile(rootFolderPath, MsSqlInitDockerfileFilePath, GenerateDockerfile(args, froms, commands));
    }

    private void GenerateSolrDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      StringBuilder args = new StringBuilder();
      StringBuilder froms = new StringBuilder();
      StringBuilder commands = new StringBuilder();
      foreach (IDockerfileGeneratorHelper helper in helpers)
      {
        args.Append(helper.GenerateSolrArgs());
        froms.Append(helper.GenerateSolrFroms());
        commands.Append(helper.GenerateSolrCommands());
      }
      GenerateSpecificDockerfile(rootFolderPath, SolrDockerfileFilePath, GenerateDockerfile(args, froms, commands));
    }

    private void GenerateSolrInitDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      StringBuilder args = new StringBuilder();
      StringBuilder froms = new StringBuilder();
      StringBuilder commands = new StringBuilder();
      foreach (IDockerfileGeneratorHelper helper in helpers)
      {
        args.Append(helper.GenerateSolrInitArgs());
        froms.Append(helper.GenerateSolrInitFroms());
        commands.Append(helper.GenerateSolrInitCommands());
      }
      GenerateSpecificDockerfile(rootFolderPath, SolrInitDockerfileFilePath, GenerateDockerfile(args, froms, commands));
    }

    private void GenerateCdDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      StringBuilder args = new StringBuilder();
      StringBuilder froms = new StringBuilder();
      StringBuilder commands = new StringBuilder();
      foreach (IDockerfileGeneratorHelper helper in helpers)
      {
        args.Append(helper.GenerateCdArgs());
        froms.Append(helper.GenerateCdFroms());
        commands.Append(helper.GenerateCdCommands());
      }
      GenerateSpecificDockerfile(rootFolderPath, CdDockerfileFilePath, GenerateDockerfile(args, froms, commands));
    }

    private void GenerateCmDockerfile(string rootFolderPath, List<IDockerfileGeneratorHelper> helpers)
    {
      StringBuilder args = new StringBuilder();
      StringBuilder froms = new StringBuilder();
      StringBuilder commands = new StringBuilder();
      foreach (IDockerfileGeneratorHelper helper in helpers)
      {
        args.Append(helper.GenerateCmArgs());
        froms.Append(helper.GenerateCmFroms());
        commands.Append(helper.GenerateCmCommands());
      }
      GenerateSpecificDockerfile(rootFolderPath, CmDockerfileFilePath, GenerateDockerfile(args, froms, commands));
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