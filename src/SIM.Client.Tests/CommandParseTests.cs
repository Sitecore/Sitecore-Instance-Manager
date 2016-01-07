namespace SIM.Client.Tests
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Client.Commands;
  using SIM.Client.Options;
  using SIM.Commands;

  [TestClass]
  public class CommandParseTests
  {
    [TestMethod]
    public void ProjectListCommand()
    {
      var parser = new ProgramOptionsParser();
      var options = new MainCommandGroup();
      var result = parser.Parse("project list".Split(), options);
      Assert.IsTrue(result);

      var commandObject = options.SelectedCommand;
      Assert.IsNotNull(commandObject);

      var command = commandObject as ListProjectsCommand;
      Assert.IsNotNull(command);

      var filter = command.Filter;
      Assert.IsNull(filter);
    }

    [TestMethod]
    public void ProjectListFilterEmptyCommand()
    {
      var parser = new ProgramOptionsParser();
      var options = new MainCommandGroup();
      var result = parser.Parse("project list --filter".Split(), options);
      Assert.IsFalse(result);
    }

    [TestMethod]
    public void ProjectListFilterCommand()
    {
      var parser = new ProgramOptionsParser();
      var options = new MainCommandGroup();
      var result = parser.Parse("project list --filter abc".Split(), options);
      Assert.IsTrue(result);

      var commandObject = options.SelectedCommand;
      Assert.IsNotNull(commandObject);

      var command = commandObject as ListProjectsCommand;
      Assert.IsNotNull(command);

      var filter = command.Filter;
      Assert.AreEqual("abc", filter);
    }
  }
}
