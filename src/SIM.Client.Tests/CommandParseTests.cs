namespace SIM.Client.Tests
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Client.Commands;
  using SIM.Commands;

  [TestClass]
  public class CommandParseTests
  {
    [TestMethod]
    public void ProjectListCommand()
    {
      var parser = new ProgramOptionsParser();
      var options = new MainCommandGroup();
      var result = parser.Parse(options, "list");
      Assert.IsTrue(result);

      var commandObject = options.SelectedCommand;
      Assert.IsNotNull(commandObject);

      var command = commandObject as ListInstancesCommand;
      Assert.IsNotNull(command);

      var filter = command.Filter;
      Assert.IsNull(filter);
    }

    [TestMethod]
    public void ProjectListFilterCommand()
    {
      var parser = new ProgramOptionsParser();
      var options = new MainCommandGroup();
      var result = parser.Parse(options, "list", "--filter", "abc");
      Assert.IsTrue(result);

      var commandObject = options.SelectedCommand;
      Assert.IsNotNull(commandObject);

      var command = commandObject as ListInstancesCommand;
      Assert.IsNotNull(command);

      var filter = command.Filter;
      Assert.AreEqual("abc", filter);
    }
  }
}
