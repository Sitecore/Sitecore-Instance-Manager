namespace SIM.Client.Commands
{
  using CommandLine;
  using CommandLine.Text;
  using JetBrains.Annotations;

  public class MainCommandGroup : MainCommandGroupBase
  {
    [CanBeNull]
    [UsedImplicitly]
    [HelpVerbOption]
    public string GetUsage([CanBeNull] string verb)
    {
      return HelpText.AutoBuild(this, verb);
    }

    #region Nested Commands

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("list", HelpText = "Show already installed instances.")]
    public ListCommandFacade ListCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("state", HelpText = "Show state of an instance.")]
    public StateCommandFacade StateCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("config", HelpText = "Show config of an instance.")]
    public ConfigCommandFacade ConfigCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("start", HelpText = "Start stopped instance.")]
    public StartCommandFacade StartCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("stop", HelpText = "Stop an instance.")]
    public StopCommandFacade StopCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("browse", HelpText = "Open an instance in default browser.")]
    public BrowseCommandFacade BrowseCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("login", HelpText = "Log in user as admin to an instance.")]
    public LoginCommandFacade LoginCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("repository", HelpText = "Show contents of repository.")]
    public RepositoryCommandFacade RepositoryCommandFacade { get; set; }

    [CanBeNull]
    [UsedImplicitly]
    [VerbOption("sync", HelpText = "One-way sync files between source instance and targets.")]
    public SyncCommandFacade SyncCommandFacade { get; set; }

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
  }
}