namespace SIM.Client
{
  using System.Collections.Generic;
  using System.IO;
  using CommandLine;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Client.Commands;
  using SIM.Core.Common;

  public class ParseArgumentsCommand
  {
    public IReadOnlyList<string> Args { get; set; }

    public bool Autocomplete { get; set; }

    public TextWriter HelpWriter { get; set; }
    
    [CanBeNull]
    public ICommand Execute()
    {
      Assert.ArgumentNotNull(Args, nameof(Args));

      var parser = new Parser(with =>
      {
        Assert.ArgumentNotNull(with, nameof(with));

        if (HelpWriter != null)
        {
          with.HelpWriter = HelpWriter;
        }
      });

      Assert.IsNotNull(parser, nameof(parser));
      
      if (Autocomplete == true)
      {
        var ensureAutocomplete = new EnsureAutocompleteCommand();

        ensureAutocomplete.Execute();
      }
      
      var result = Execute(parser);

      ICommand selectedCommand = null;
      result.WithParsed(x => selectedCommand = (ICommand)x);

      return selectedCommand;
    }

    private ParserResult<object> Execute(Parser parser)
    {
      var result = parser.ParseArguments<
        BrowseCommandFacade,
        ConfigCommandFacade,
        DeleteCommandFacade,
        InstallCommandFacade,
        InstallModuleCommandFacade,
        ListCommandFacade,
        LoginCommandFacade,
        ProfileCommandFacade,
        RepositoryCommandFacade,
        StartCommandFacade,
        StateCommandFacade,
        StopCommandFacade
      >(Args);

      return result;
    }
  }
}