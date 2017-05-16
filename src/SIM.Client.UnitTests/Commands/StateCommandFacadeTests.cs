namespace SIM.Client.UnitTests.Commands
{
  using SIM.Client.Commands;
  using Xunit;

  public class StateCommandFacadeTests
  {
    [Fact]
    public void NoName()
    {
      const string CommandLine = "state";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StateCommandFacade)parseArguments.Execute();
     
      Assert.Null(command);
    }

    [Fact]
    public void HasName_Short()
    {
      const string CommandLine = "state -n name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StateCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }

    [Fact]
    public void HasName_Long()
    {
      const string CommandLine = "state --name name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StateCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }
  }
}
