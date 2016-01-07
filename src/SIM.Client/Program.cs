namespace SIM.Client
{
  using System;
  using System.Linq;
  using CommandLine;
  using Newtonsoft.Json;
  using SIM.Client.Commands;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public static class Program
  {
    public static void Main([NotNull] string[] args)
    {
      Assert.ArgumentNotNull(args, "args");

      Log.Initialize(new DummyLogProvider());

      var parser = new Parser(with => with.NotNull().HelpWriter = Console.Error);
      Assert.IsNotNull(parser, "parser");

      var options = new MainCommandGroup();
      if (!parser.ParseArguments(args, options, delegate { }))
      {
        Environment.Exit(Parser.DefaultExitCodeFail);
      }

      var result = options.SelectedCommand.Execute() as object;

      var displayOptions = new DisplayOptions();
      parser.ParseArguments(args, displayOptions);

      var query = displayOptions.Query;
      if (!string.IsNullOrEmpty(query))
      {
        var obj = result;
        foreach (var chunk in query.Split("./".ToCharArray()))
        {
          var type = obj.GetType();
          var newObj = type.GetProperties().FirstOrDefault(x => x.Name.Equals(chunk, StringComparison.OrdinalIgnoreCase));
          if (newObj == null)
          {
            Console.WriteLine("Cannot find '" + chunk + "' chunk of '" + query + "' query in the object: ");
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));

            return;
          }

          obj = newObj.GetValue(obj);
        }

        result = obj;
      }

      Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
  }
}
