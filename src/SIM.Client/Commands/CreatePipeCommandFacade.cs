namespace SIM.Client.Commands
{
  using CommandLine;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core.Commands;

  public class CreatePipeCommandFacade : CreatePipeCommand
  {
    [UsedImplicitly]
    public CreatePipeCommandFacade()
    {              
    }    

    [Option('n', "name", Required = true)]
    public override string Name { get; set; }

    [Option('f', "force")]
    public override bool? Force { get; set; }
  }
}