namespace SIM.Tests.Pipelines.Export
{
  using System;
  using System.IO;
  using System.Linq;
  using SIM.Extensions;
  using SIM.Pipelines.Export;
  using Xunit;

  public class ExportArgsTests
  {
    [Fact]
    public void GetTempFolderTest()
    {
      var drive = GetDrive();
      var result = ExportArgs.GetTempFolder(null, $"{drive}inetpub\\wwwroot");

      // to eliminate random part
      var actual = Path.GetDirectoryName(result);

      Assert.Equal(drive, actual);
    }

    [Fact]
    public void GetTempFolderTest_Custom()
    {
      var drive = GetDrive();
      var result = ExportArgs.GetTempFolder($"{drive}Sitecore\\Temp", @"D:\inetpub\wwwroot");

      // to eliminate random part
      var actual = Path.GetDirectoryName(result);

      Assert.Equal($"{drive}Sitecore\\Temp", actual);
    }

    private string GetDrive()
    {
      //Try to get disk C as it is quite common. 
      //Get any disk if it doesn't exist.
      var drive = Environment.GetLogicalDrives()
        .FirstOrDefault(d => d.EqualsIgnoreCase("c:\\"))
        ?? Environment.GetLogicalDrives().First();

      return drive;
    }
  }
}
