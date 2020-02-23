using SIM.Telemetry.Models;
using System;
using Xunit;
using Sitecore.Diagnostics.Base.Exceptions;

namespace SIM.Telemetry.Tests.Models
{
  public class TelemetryEventContextTests
  {
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Ctor_Should_ThrowArgumentNull(string version)
    {
      // Arrange
      var app = Guid.NewGuid();
      var device = Guid.NewGuid();

      // Act
      Action action = () => new TelemetryEventContext(app, device, version);

      // Assert
      Assert.Throws<ArgumentNullOrEmptyException>(action);
    }

    [Theory]
    [InlineData("0.0.0.0")]
    public void Ctor_ShouldNotThrow_ArgumentNullOrEmpty(string version)
    {
      // Arrange
      var app = Guid.NewGuid();
      var device = Guid.NewGuid();

      // Act
      var context = new TelemetryEventContext(app, device, version);

      // Assert
      Assert.NotNull(context);
    }
  }
}