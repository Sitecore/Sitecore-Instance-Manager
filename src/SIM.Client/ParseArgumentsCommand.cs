namespace SIM.Client
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using CommandLine;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Client.Commands;
  using SIM.Core.Common;

  public class ParseArgumentsCommand
  {
    public IReadOnlyList<string> Args { get; set; }

    public bool? Autocomplete { get; set; }

    public TextWriter HelpWriter { get; set; }
    
    [CanBeNull]
    public ICommand Execute()
    {
      Assert.ArgumentNotNull(Args, nameof(Args));

      var parser = new Parser(with =>
      {
        Assert.ArgumentNotNull(with, nameof(with));

        with.MutuallyExclusive = true;

        if (HelpWriter != null)
        {
          with.HelpWriter = HelpWriter;
        }
      });

      Assert.IsNotNull(parser, nameof(parser));

      var options = new MainCommandGroup();
      if (Autocomplete == true)
      {
        var ensureAutocomplete = new EnsureAutocompleteCommand
        {
          Options = options
        };

        ensureAutocomplete.Execute();
      }

      ICommand selectedCommand = null;
      if (!parser.ParseArguments(Args.ToArray(), options, (verb, command) => selectedCommand = (ICommand)command))
      {
        return null;
      }

      Assert.IsNotNull(selectedCommand, nameof(selectedCommand));

      return selectedCommand;
    }
  }
}