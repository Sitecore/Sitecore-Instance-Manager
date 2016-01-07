namespace SIM.Client
{
  using CommandLine;
  using Sitecore.Diagnostics.Base.Annotations;

  public class DisplayOptions
  {
    [CanBeNull]
    [Option("query")]
    public string Query { get; set; }
  }
}