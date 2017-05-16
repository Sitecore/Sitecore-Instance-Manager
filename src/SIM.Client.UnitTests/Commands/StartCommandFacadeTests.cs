namespace SIM.Client.UnitTests.Commands
{
  using SIM.Client.Commands;
  using Xunit;

  public class StartCommandFacadeTests
  {
    [Fact]
    public void NoName()
    {
      const string CommandLine = "start";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StartCommandFacade)parseArguments.Execute();
     
      Assert.Null(command);
    }

    [Fact]
    public void HasName_Short()
    {
      const string CommandLine = "start -n name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StartCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }

    [Fact]
    public void HasName_Long()
    {
      const string CommandLine = "start --name name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StartCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }
  }
}
