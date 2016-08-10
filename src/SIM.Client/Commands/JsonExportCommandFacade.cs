namespace SIM.Client.Commands
{
  using CommandLine;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core.Commands;

  public class JsonExportCommandFacade : JsonExportCommand
  {
    [UsedImplicitly]
    public JsonExportCommandFacade()
    {
    }

    [Option('n', "name", Required = true, HelpText = "Sitecore instance name")]
    public override string Name { get; set; }

    [Option('d', "database", Required = true, HelpText = "Database name")]
    public override string Database { get; set; }

    [Option('i', "item", HelpText = "Item ID or full path")]
    public override string ItemName { get; set; }              

    [Option('o', "output", Required = true, HelpText = "Output json file")]
    public override string OutputFile { get; set; }

    [Option('s', "system", HelpText = "Include system fields")]
    public override bool? SystemFields { get; set; }
  }
}
