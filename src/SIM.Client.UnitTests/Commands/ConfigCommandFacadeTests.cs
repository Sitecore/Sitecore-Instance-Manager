namespace SIM.Client.UnitTests.Commands
{
  using SIM.Client.Commands;
  using Xunit;

  public class ConfigCommandFacadeTests
  {
    [Fact]
    public void NoParams()
    {
      const string CommandLine = "config";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (ConfigCommandFacade)parseArguments.Execute();
     
      Assert.Null(command);
    }

    [Fact]
    public void HasName_Short()
    {
      const string CommandLine = "config -n name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (ConfigCommandFacade)parseArguments.Execute();

      Assert.Null(command);
    }

    [Fact]
    public void HasName_Long()
    {
      const string CommandLine = "config --name name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (ConfigCommandFacade)parseArguments.Execute();

      Assert.Null(command);
    }

    [Fact]
    public void HasDatabase_Short()
    {
      const string CommandLine = "config -d database123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (ConfigCommandFacade)parseArguments.Execute();

      Assert.Null(command);
    }

    [Fact]
    public void HasDatabase_Long()
    {
      const string CommandLine = "config --database database123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (ConfigCommandFacade)parseArguments.Execute();

      Assert.Null(command);
    }

    [Fact]
    public void HasNameAndDatabase()
    {
      const string CommandLine = "config --name name123 --database database123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (ConfigCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
      Assert.Equal("database123", command.Database);
    }
  }
}
