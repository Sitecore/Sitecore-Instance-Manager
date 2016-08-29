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

    [Option('n', "name", /*Required = true,*/ MutuallyExclusiveSet = "instance", HelpText = "Sitecore instance name")]
    public override string Name { get; set; }

    [Option('d', "database", /*Required = true,*/ MutuallyExclusiveSet = "instance", HelpText = "Database name")]
    public override string Database { get; set; }

    [Option('c', "connectionString", /*Required = true,*/ MutuallyExclusiveSet = "connectionString", HelpText = "Connection string (use it instead of -n and -d)")]
    public override string ConnectionString { get; set; }

    [Option('i', "item", HelpText = "Item ID or full path")]
    public override string ItemName { get; set; }              

    [Option('o', "output", Required = true, HelpText = "Output json file")]
    public override string OutputFile { get; set; }

    [Option('s', "system", DefaultValue = SystemFieldsDefault, HelpText = "Include system fields")]
    public override bool? SystemFields { get; set; }

    [Option("sort", DefaultValue = SortDefault, HelpText = "Sort items and fields by ID")]
    public override bool? Sort { get; set; }

    [Option("ignore", HelpText = "Specify IDs of fields to ignore in export")]
    public override string IgnoreFieldIDs { get; set; }
  }
}
