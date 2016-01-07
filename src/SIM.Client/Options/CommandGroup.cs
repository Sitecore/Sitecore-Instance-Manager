namespace SIM.Client.Options
{
  using System.Linq;
  using CommandLine;
  using SIM.Commands;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class CommandGroup : IProgramOptions
  {
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
        if (propertyInfo == null || propertyInfo.CustomAttributes.ToArray().All(x => x == null || x.AttributeType != typeof(VerbOptionAttribute)))
        {
          continue;
        }

        var innerCommand = propertyInfo.GetValue(commandContainer) as AbstractCommand;
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