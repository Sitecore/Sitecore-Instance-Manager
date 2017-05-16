namespace SIM.Client.UnitTests.Commands
{
  using SIM.Client.Commands;
  using Xunit;

  public class LoginCommandFacadeTests
  {
    [Fact]
    public void NoName()
    {
      const string CommandLine = "login";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (LoginCommandFacade)parseArguments.Execute();
     
      Assert.Null(command);
    }

    [Fact]
    public void HasName_Short()
    {
      const string CommandLine = "login -n name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (LoginCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }

    [Fact]
    public void HasName_Long()
    {
      const string CommandLine = "login --name name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (LoginCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }
  }
}
