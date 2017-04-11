namespace SIM.Client.Groups
{
  using System.Linq;
  using CommandLine;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;

  public abstract class MainCommandGroupBase
  {        
    [CanBeNull]
    private ICommand FindCommand([NotNull] object commandContainer)
    {
      Assert.ArgumentNotNull(commandContainer, nameof(commandContainer));

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
          var command = FindCommand(innerCommand);
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