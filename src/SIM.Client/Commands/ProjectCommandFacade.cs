namespace SIM.Client.Commands
{
  using CommandLine;
  using SIM.Commands;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ProjectCommandFacade : ProjectCommand
  {
    #region Nested Commands

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("list", HelpText = "Update remote refs along with associated objects.")]
    public ListProjectsCommand ListProjectsCommand { get; set; }

    #endregion
  }
}
