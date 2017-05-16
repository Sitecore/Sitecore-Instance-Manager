namespace SIM.Client.UnitTests.Commands
{
  using SIM.Client.Commands;
  using Xunit;

  public class StopCommandFacadeTests
  {
    [Fact]
    public void NoName()
    {
      const string CommandLine = "stop";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StopCommandFacade)parseArguments.Execute();
     
      Assert.Null(command);
    }

    [Fact]
    public void HasName_Short()
    {
      const string CommandLine = "stop -n name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StopCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }

    [Fact]
    public void HasName_Long()
    {
      const string CommandLine = "stop --name name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StopCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
    }

    [Fact]
    public void HasForce_Short()
    {
      const string CommandLine = "stop -f";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StopCommandFacade)parseArguments.Execute();

      Assert.Null(command);
    }

    [Fact]
    public void HasForce_Long()
    {
      const string CommandLine = "stop --force";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StopCommandFacade)parseArguments.Execute();

      Assert.Null(command);
    }

    [Fact]
    public void HasNameAndForce_Short()
    {
      const string CommandLine = "stop -n name123 -f";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StopCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
      Assert.Equal(true, command.Force);
    }

    [Fact]
    public void HasNameAndForce_Short_Mixed()
    {
      const string CommandLine = "stop -fn name123";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StopCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
      Assert.Equal(true, command.Force);
    }

    [Fact]
    public void HasNameAndForce_Long()
    {
      const string CommandLine = "stop --name name123 --force";

      var args = CommandLine.Split();
      var parseArguments = new ParseArgumentsCommand
      {
        Args = args
      };

      var command = (StopCommandFacade)parseArguments.Execute();

      Assert.NotNull(command);
      Assert.Equal("name123", command.Name);
      Assert.Equal(true, command.Force);
    }
  }
}
