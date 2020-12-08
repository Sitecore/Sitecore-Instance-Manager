using System.Text.RegularExpressions;
using SIM.Products;
using Xunit;

namespace SIM.Tests.Products
{
  public class ProductTest
  {
    [Theory]
    // Modules
    [InlineData("Data Exchange Framework 1.3.0 rev. 170210.zip", "Data Exchange Framework", "1.3.0", "170210", ".zip")]
    [InlineData("Email Experience Manager 3.3.0 rev. 160527 (not sc package).zip", "Email Experience Manager", "3.3.0", "160527 (not sc package)", ".zip")]
    [InlineData("Sitecore JavaScript Services Server for Sitecore 9.2 XP 12.0.0 rev. 190522.zip", "Sitecore JavaScript Services Server for Sitecore 9.2 XP", "12.0.0", "190522", ".zip")]
    // Platform
    [InlineData("Sitecore 8.0 rev. 150621.zip", "Sitecore", "8.0", "150621", ".zip")]
    [InlineData("Sitecore 9.1.0 rev. 001564 (WDP XP0 packages).zip", "Sitecore", "9.1.0", "001564 (WDP XP0 packages)", ".zip")]
    [InlineData("Sitecore 10.0.0 rev. 004346 (WDP XM1 packages).zip", "Sitecore", "10.0.0", "004346 (WDP XM1 packages)", ".zip")]
    [InlineData("Sitecore 10.0.0 rev. 004346 (WDP XP1 packages).zip", "Sitecore", "10.0.0", "004346 (WDP XP1 packages)", ".zip")]
    [InlineData("Sitecore 10.0.0 rev. 004346 (Setup XP0 Developer Workstation rev. 1.2.0-r64).zip", "Sitecore", "10.0.0", "004346 (Setup XP0 Developer Workstation rev. 1.2.0-r64)", ".zip")]
    public void Product_ShouldBeParsed_FromPackagePath(
      string packageName,
      string expectedProductName,
      string expectedProductVersion, 
      string expectedProductRevision,
      string expectedFileExtention
      )
    {
      //Arrange
      string fakePackagePath = $"C:\\SC Installs\\sim_\\{packageName}";
      Regex productRegex = Product.ProductRegex;

      //Act
      Match match = productRegex.Match(fakePackagePath);
      string actualName = match.Groups[1].Value;
      string actualVersion = match.Groups[2].Value;
      string actualRevision = match.Groups[5].Value;
      string actualExtention = match.Groups[7].Value;

      //Assert
      Assert.Equal(expectedProductName, actualName);
      Assert.Equal(expectedProductVersion, actualVersion);
      Assert.Equal(expectedProductRevision, actualRevision);
      Assert.Equal(expectedFileExtention, actualExtention);
    }
  }
}