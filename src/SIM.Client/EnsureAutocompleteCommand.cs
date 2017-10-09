namespace SIM.Client
{
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using CommandLine;
  using Sitecore.Diagnostics.Logging;

  public class EnsureAutocompleteCommand
  {
    public void Execute()
    {
      foreach (var type in typeof(Program).Assembly.GetTypes())
      {
        var verb = type.GetCustomAttributes().OfType<VerbAttribute>().FirstOrDefault();
        if (verb == null)
        {
          continue;
        }

        var command = verb.Name;
        if (File.Exists(command))
        {
          continue;
        }

        CreateEmptyFileInCurrentDirectory(command);
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