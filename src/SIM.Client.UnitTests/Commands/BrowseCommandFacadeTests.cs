namespace SIM.Client.UnitTests.Commands
{
  using SIM.Client.Commands;
  using Xunit;

  public class BrowseCommandFacadeTests
  {
    [Fact]
    public void NoName()
    {
      const string CommandLine = "browse";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (BrowseCommandFacade)parseArguments.Execute();
     
      Assert.Null(command);
    }

    [Fact]
    public void HasName_Short()
    {
      const string CommandLine = "browse -n name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (BrowseCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }

    [Fact]
    public void HasName_Long()
    {
      const string CommandLine = "browse --name name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (BrowseCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }
  }
}
