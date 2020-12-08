using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using SIM.Sitecore9Installer.Validation;
using SIM.Sitecore9Installer.Validation.Validators;
using Xunit;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class DiskSpaceValidatorTests
  {
    [Theory]
    [InlineData(@"C:\inetpub\wwwroot", 7368709120, ValidatorState.Success, "Hard disk has enough free space to continue the installation.")]
    [InlineData(@"C:\inetpub\wwwroot\", 5368709119, ValidatorState.Warning, @"Hard disk 'C:\' has a little free space.")]
    [InlineData(@"D:\inetpub\", 3221225470, ValidatorState.Error, @"Hard disk 'D:\' does not have enough free space to continue installation.")]
    [InlineData(@"D:\inetpub\wwwroot", -1, ValidatorState.Error, @"Hard disk 'D:\' has not been found.")]

    public void ReturnsValidValidationResults(string deployRoot, long freeSpace, ValidatorState expectedResult, string resultMessage)
    {
      //Arrange
      Task task = Substitute.For<Task>("", 0, null, null, new Dictionary<string, string>());
      GlobalParameters globals = new GlobalParameters();
      task.LocalParams.Returns(new LocalParameters(new List<InstallParam>(),globals));
      task.LocalParams.AddOrUpdateParam("DeployRoot",deployRoot,InstallParamType.String);

      List <Task> tasks = Substitute.For<List<Task>>();
      tasks.Add(task);
      
      DiskSpaceValidator val = Substitute.ForPartsOf<DiskSpaceValidator>();
      val.GetHardDriveFreeSpace(string.Empty).ReturnsForAnyArgs(freeSpace);

      val.Data["HardDriveWarningLimit"] = "5368709120";
      val.Data["HardDriveErrorLimit"] = "3221225472";
      val.Data["DeployRoot"] = "DeployRoot";
      //Act
      IEnumerable<ValidationResult> results = val.Evaluate(tasks);

      //Assert
      Assert.Contains(results, r => r.State == expectedResult && r.Message == resultMessage);
    }
  }
}
