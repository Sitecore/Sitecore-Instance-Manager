namespace SIM.Client.Commands
{
  using CommandLine;
  using CommandLine.Text;
  using Sitecore.Diagnostics.Base.Annotations;

  public class MainCommandGroup : MainCommandGroupBase
  {
    #region Nested Commands

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("list", HelpText = "Show already installed instances.")]
    public ListCommandFacade ListCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("profile", HelpText = "Show profile.")]
    public ProfileCommandFacade ProfileCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("install", HelpText = "Install Sitecore instance.")]
    public InstallCommandFacade InstallCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("installmodule", HelpText = "Install Sitecore module.")]
    public InstallModuleCommandFacade InstallModuleCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("delete", HelpText = "Delete Sitecore instance.")]
    public DeleteCommandFacade DeleteCommandFacade { get; set; }

    #endregion

    [CanBeNull]
    [UsedImplicitly]
    [HelpVerbOption]
    public string GetUsage([CanBeNull] string verb)
    {
      return HelpText.AutoBuild(this, verb);
    }
  }
}