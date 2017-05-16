namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  [Verb("profile", HelpText = "Show profile.")]
  public class ProfileCommandFacade : ProfileCommand
  {
    [UsedImplicitly]
    public ProfileCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('c', "connectionString")]
    public override string ConnectionString { get; set; }

    [Option('i', "instancesRoot")]
    public override string InstancesRoot { get; set; }

    [Option('l', "license")]
    public override string License { get; set; }

    [Option('r', "repository")]
    public override string Repository { get; set; }
  }
}