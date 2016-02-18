namespace SIM.Client.Commands
{
  using System.Linq;
  using CommandLine;
  using CommandLine.Text;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core.Common;

  public abstract class MainCommandGroupBase
  {
    [NotNull]
    public ICommand SelectedCommand
    {
      get
      {
        var command = this.FindCommand(this);
        Assert.IsNotNull(command, "There is no selected command");

        return command;
      }
    }

    [CanBeNull]
    private ICommand FindCommand([NotNull] object commandContainer)
    {
      Assert.ArgumentNotNull(commandContainer, "commandContainer");

      var properties = commandContainer.GetType().GetProperties();
      foreach (var propertyInfo in properties)
      {
        if (propertyInfo == null || propertyInfo.GetCustomAttributes(true).ToArray().All(x => x == null || x is VerbOptionAttribute))
        {
          continue;
        }

        var innerCommand = propertyInfo.GetValue(commandContainer, null) as ICommand;
        if (innerCommand != null)
        {
          var command = this.FindCommand(innerCommand);
          if (command != null)
          {
            return command;
          }
        }
      }

      return commandContainer as ICommand;
    }
  }
}