namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  [Verb("install", HelpText = "Install Sitecore instance.")]
  public class InstallCommandFacade : InstallCommand
  {
    [UsedImplicitly]
    public InstallCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }

    [Option('s', "sqlPrefix", HelpText = "Logical names prefix of SQL databases, by default equals to instance name")]
    public override string SqlPrefix { get; set; }

    [Option('d', "distributionPackage", HelpText = "Path to the Sitecore standalone product's distribution package path")]
    public override string DistributionPackagePath { get; set; }

    [Option('p', "product")]
    public override string Product { get; set; }

    [Option('v', "version")]
    public override string Version { get; set; }

    [Option('r', "revision")]
    public override string Revision { get; set; }

    [Option('a', "attach", HelpText = "Attach SQL databases, or just update ConnectionStrings.config", Default = AttachDatabasesDefault)]
    public override bool AttachDatabases { get; set; }

    [Option('u', "skipUnnecessaryFiles", HelpText = "Skip unnecessary files to speed up installation", Default = AttachDatabasesDefault)]
    public override bool SkipUnnecessaryFiles { get; set; }
  }
}