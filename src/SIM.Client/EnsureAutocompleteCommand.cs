namespace SIM.Client
{
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using CommandLine;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using SIM.Client.Commands;
  using SIM.Core.Common;

  public class EnsureAutocompleteCommand
  {
    public MainCommandGroup Options { get; set; }

    public void Execute()
    {
      Assert.ArgumentNotNull(Options, nameof(Options));

      foreach (var propertyInfo in Options.GetType().GetProperties())
      {
        if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
        {
          var verb = propertyInfo.GetCustomAttributes().OfType<VerbOptionAttribute>().FirstOrDefault();
          if (verb == null)
          {
            continue;
          }

          var command = verb.LongName;
          if (File.Exists(command))
          {
            continue;
          }

          CreateEmptyFileInCurrentDirectory(command);
        }
      }
    }

    private void CreateEmptyFileInCurrentDirectory(string command)
    {
      try
      {
        File.Create(command).Close();
      }
      catch
      {
        Log.Warn($"Cannot create file: {command}");
      }
    }
  }
}