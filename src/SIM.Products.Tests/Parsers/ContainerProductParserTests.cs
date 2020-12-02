using Xunit;
using NSubstitute;
using SIM.Products.ProductParsers;

namespace SIM.Products.Tests.Parsers
{
  public class ContainerProductParserTests
  {
    [Theory]
    [InlineData("zzz")]
    public void TryParseProduct_CannotParsePackagePath_ReturnsFalse(string packagePath)
    {
      // Arrange
      var parser = new ContainerProductParser();

      // Act
      var result = parser.TryParseProduct(packagePath, out Product product);

      // Assert
      Assert.False(result);
    }

    [Theory]
    [InlineData("zzz")]
    public void TryParseProduct_CannotParsePackagePath_OutProductIsNull(string packagePath)
    {
      // Arrange
      var parser = new ContainerProductParser();

      // Act
      var result = parser.TryParseProduct(packagePath, out Product product);

      // Assert
      Assert.Null(product);
    }

    [Theory]
    [InlineData("c:\\Install\\sc\\SitecoreContainerDeployment.10.0.0.004346.150.zip")]
    public void TryParseProduct_CanParsePackagePath_ReturnsTrue(string packagePath)
    {
      // Arrange
      var parser = new ContainerProductParser();

      // Act
      var result = parser.TryParseProduct(packagePath, out Product product);

      // Assert
      Assert.True(result);
    }

    [Theory]
    [InlineData("c:\\Install\\sc\\SitecoreContainerDeployment.10.0.0.004346.150.zip")]
    public void TryParseProduct_CanParsePackagePath_OutProductIsNotNull(string packagePath)
    {
      // Arrange
      var parser = new ContainerProductParser();

      // Act
      var result = parser.TryParseProduct(packagePath, out Product product);

      // Assert
      Assert.NotNull(product);
    }

    [Theory]
    [InlineData(
      "c:\\Install\\sc\\SitecoreContainerDeployment.10.0.0.004346.150.zip",
      "SitecoreContainerDeployment",
      "10.0",
      "10.0.0",
      "004346.150")]
    public void TryParseProduct_CanParsePackagePath_GetOrCreateProductIsCalled(
      string packagePath,
      string originalName,
      string twoVersion,
      string triVersion,
      string revision
    )
    {
      // Arrange
      var parser = Substitute.ForPartsOf<ContainerProductParser>();
      parser.WhenForAnyArgs(x => x.GetOrCreateProduct(
        Arg.Any<string>(),
        Arg.Any<string>(),
        Arg.Any<string>(),
        Arg.Any<string>(),
        Arg.Any<string>()
        )).DoNotCallBase();

      // Act
      parser.TryParseProduct(packagePath, out Product product);

      // Assert
      parser.Received().GetOrCreateProduct(originalName, packagePath, twoVersion, triVersion, revision);
      //parser.DidNotReceive().GetOrCreateProduct(originalName, packagePath, twoVersion, triVersion, revision);
    }

    [Theory]
    [InlineData("zzz")]
    public void TryParseProduct_CannotParsePackagePath_GetOrCreateProductIsNotCalled(string packagePath)
    {
      // Arrange
      var parser = Substitute.ForPartsOf<ContainerProductParser>();
      parser.WhenForAnyArgs(x => x.GetOrCreateProduct(
        Arg.Any<string>(),
        Arg.Any<string>(),
        Arg.Any<string>(),
        Arg.Any<string>(),
        Arg.Any<string>()
      )).DoNotCallBase();

      // Act
      parser.TryParseProduct(packagePath, out Product product);

      // Assert
      parser.DidNotReceive().GetOrCreateProduct(
        Arg.Any<string>(), 
        Arg.Any<string>(), 
        Arg.Any<string>(), 
        Arg.Any<string>(), 
        Arg.Any<string>());
    }

    [Theory]
    [InlineData("zzz")]
    public void TryParseName_CannotParsePackagePath_ReturnsFalse(string packagePath)
    {
      // Arrange
      var parser = new ContainerProductParser();

      // Act
      var result = parser.TryParseName(packagePath, out string name);

      // Assert
      Assert.False(result);
    }

    [Theory]
    [InlineData("zzz")]
    public void TryParseName_CannotParsePackagePath_OutNameIsNull(string packagePath)
    {
      // Arrange
      var parser = new ContainerProductParser();

      // Act
      parser.TryParseName(packagePath, out string name);

      // Assert
      Assert.Null(name);
    }

    [Theory]
    [InlineData("c:\\Install\\sc\\SitecoreContainerDeployment.10.0.0.004346.150.zip")]
    public void TryParseName_CanParsePackagePath_ReturnsTrue(string packagePath)
    {
      // Arrange
      var parser = new ContainerProductParser();

      // Act
      var result = parser.TryParseName(packagePath, out string name);

      // Assert
      Assert.True(result);
    }

    [Theory]
    [InlineData("c:\\Install\\sc\\SitecoreContainerDeployment.10.0.0.004346.150.zip", "SitecoreContainerDeployment")]
    public void TryParseName_CanParsePackagePath_OutNameIsProper(string packagePath, string expectedName)
    {
      // Arrange
      var parser = new ContainerProductParser();

      // Act
      parser.TryParseName(packagePath, out string actualName);

      // Assert
      Assert.Equal(expectedName, actualName);
    }
  }
}