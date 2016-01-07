namespace SIM.Client.Options
{
  using System;
  using System.Linq;
  using CommandLine;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ProgramOptionsParser
  {
    public bool Parse([NotNull] string[] args, [NotNull] IProgramOptions options)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(options, "options");

      var silentParser = new Parser();
      var parser = new Parser(with => with.NotNull().HelpWriter = Console.Error);
      Assert.IsNotNull(parser, "parser");

      var currentOptions = options as object;
      var currentArgs = args;

      while (true)
      {
        var exit = true;

        this.ParseWithVerbs(silentParser, currentArgs, currentOptions);

        currentArgs = currentArgs.Skip(1).ToArray();
        if (currentArgs.Length == 0)
        {
          break;
        }

        var properties = currentOptions.GetType().GetProperties();
        foreach (var propertyInfo in properties)
        {
          if (propertyInfo == null || propertyInfo.CustomAttributes.ToArray().All(x => x == null || x.AttributeType != typeof(VerbOptionAttribute)))
          {
            continue;
          }

          var propertyValue = propertyInfo.GetValue(currentOptions);
          if (propertyValue == null)
          {
            continue;
          }

          currentOptions = propertyValue;

          exit = false;
          break;
        }

        if (exit)
        {
          break;
        }
      }

      // last options object does not have verbs so parse without verb
      return this.ParseWithoutVerbs(parser, currentArgs, currentOptions);
    }

    private bool ParseWithoutVerbs([NotNull] Parser parser, [NotNull] string[] currentArgs, [NotNull] object currentOptions)
    {
      Assert.ArgumentNotNull(parser, "parser");
      Assert.ArgumentNotNull(currentArgs, "currentArgs");
      Assert.ArgumentNotNull(currentOptions, "currentOptions");

      return parser.ParseArguments(currentArgs, currentOptions);
    }

    private void ParseWithVerbs([NotNull] Parser parser, [NotNull] string[] currentArgs, [NotNull] object currentOptions)
    {
      Assert.ArgumentNotNull(parser, "parser");
      Assert.ArgumentNotNull(currentArgs, "currentArgs");
      Assert.ArgumentNotNull(currentOptions, "currentOptions");

      parser.ParseArguments(currentArgs, currentOptions, delegate { });
    }
  }
}