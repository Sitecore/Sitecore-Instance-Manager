namespace SIM.Client
{
  using System;
  using SIM.Client.Commands;
  using SIM.Client.Options;

  public class Program
  {
    public static void Main(string[] args)
    {
      var parser = new ProgramOptionsParser();
      var options = new MainCommandGroup();
      if (!parser.Parse(args, options))
      {
        Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
      }

      options.SelectedCommand.Execute();
    }
  }
}
