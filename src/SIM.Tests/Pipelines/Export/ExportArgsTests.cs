namespace SIM.Tests.Pipelines.Export
{
  using System.IO;
  using SIM.Pipelines.Export;
  using Xunit;

  public class ExportArgsTests
  {
    [Fact]
    public void GetTempFolderTest()
    {
      var result = ExportArgs.GetTempFolder(null, @"D:\inetpub\wwwroot");

      // to eliminate random part
      var actual = Path.GetDirectoryName(result);

      Assert.Equal(@"D:\", actual);
    }

    [Fact]
    public void GetTempFolderTest_Custom()
    {
      var result = ExportArgs.GetTempFolder(@"E:\Sitecore\Temp", @"D:\inetpub\wwwroot");

      // to eliminate random part
      var actual = Path.GetDirectoryName(result);

      Assert.Equal(@"E:\Sitecore\Temp", actual);
    }
  }
}
