namespace SIM.Client.Commands
{
  using System.Linq;
  using CommandLine;
  using CommandLine.Text;
  using SIM.Commands.Common;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class MainCommandGroup
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
    [VerbOption("delete", HelpText = "Delete Sitecore instance.")]
    public DeleteCommandFacade DeleteCommandFacade { get; set; }

    #endregion

    [HelpVerbOption]
    public string GetUsage(string verb)
    {
      return HelpText.AutoBuild(this, verb);
    }

    [NotNull]
    public AbstractCommand SelectedCommand
    {
      get
      {
        var command = this.FindCommand(this);
        Assert.IsNotNull(command, "There is no selected command");

        return command;
      }
    }

    [CanBeNull]
    private AbstractCommand FindCommand([NotNull] object commandContainer)
    {
      Assert.ArgumentNotNull(commandContainer, "commandContainer");

      var properties = commandContainer.GetType().GetProperties();
      foreach (var propertyInfo in properties)
      {
        if (propertyInfo == null || propertyInfo.GetCustomAttributes(true).ToArray().All(x => x == null || x is VerbOptionAttribute))
        {
          continue;
        }

        var innerCommand = propertyInfo.GetValue(commandContainer, null) as AbstractCommand;
        if (innerCommand != null)
        {
          var command = this.FindCommand(innerCommand);
          if (command != null)
          {
            return command;
          }
        }
      }

      return commandContainer as AbstractCommand;
    }
  }
}