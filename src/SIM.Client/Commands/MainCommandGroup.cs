namespace SIM.Client.Commands
{
  using CommandLine;
  using CommandLine.Text;
  using SIM.Client.Options;
  using Sitecore.Diagnostics.Base.Annotations;

  public class MainCommandGroup : CommandGroup
  {
    #region Nested Commands

    [CanBeNull]
    [VerbOption("project", HelpText = "Record changes to the repository.")]
    public ListProjectsCommandFacade ProjectAbstractVerb { get; set; }

    #endregion

    [HelpVerbOption]
    public string GetUsage(string verb)
    {
      return HelpText.AutoBuild(this, verb);
    }
  }
}