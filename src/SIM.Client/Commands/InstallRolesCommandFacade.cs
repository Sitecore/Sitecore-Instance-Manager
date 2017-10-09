namespace SIM.Client.Commands
{
  using CommandLine;
  using JetBrains.Annotations;
  using SIM.Core.Commands;
  using SIM.IO.Real;

  [CanBeNull]
  [UsedImplicitly]
  [Verb("installroles", HelpText = "Install Configuration Roles module.")]
  public class InstallRolesCommandFacade : InstallRolesCommand
  {
    [UsedImplicitly]
    public InstallRolesCommandFacade()
      : base(new RealFileSystem())
    {
    }

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }

    [Option('r', "roles")]
    public override string Roles { get; set; }
  }
}