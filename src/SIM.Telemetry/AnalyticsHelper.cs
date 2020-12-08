namespace SIM.Telemetry
{
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Globalization;
  using System.IO;
  using System.Windows.Forms;

  public static class AnalyticsHelper
  {
    public static Guid GetDeviceId(string temporaryFolder, [CanBeNull] ILogger logger)
    {
      if (logger == null) throw new ArgumentNullException(nameof(logger));

      var fileName = "deviceid.txt";

      var tempFolder = temporaryFolder;
      var path = Path.Combine(tempFolder, fileName);
      if (Directory.Exists(tempFolder))
      {
        if (File.Exists(path))
        {
          var deviceId = File.ReadAllText(path);

          if (!string.IsNullOrEmpty(deviceId)
            && Guid.TryParse(deviceId, out Guid deviceIdGuid))
          {
            return deviceIdGuid;
          }

          try
          {
            File.Delete(path);
          }
          catch (Exception ex)
          {
            logger.LogDebug(ex, $"Cannot delete '{fileName}' file");
          }
        }
      }
      else
      {
        Directory.CreateDirectory(tempFolder);
      }

      var newDeviceId = Guid.NewGuid();

      try
      {
        File.WriteAllText(path, newDeviceId.ToString());
      }
      catch (Exception ex)
      {
        logger.LogDebug(ex, $"Cannot write device ID to '{fileName}' file");
      }

      return newDeviceId;
    }

    [NotNull]
    public static string GetCurrentUICulture([CanBeNull] ILogger logger)
    {
      if (logger == null) throw new ArgumentNullException(nameof(logger));

      var defaultUICulture = "en";

      CultureInfo currentUICulture;

      try
      {
        currentUICulture = CultureInfo.CurrentUICulture;

        return currentUICulture.ToString();
      }
      catch (Exception ex)
      {
        logger.LogDebug(ex, $"CurrentUICulture cannot be retrieved. The {defaultUICulture} culture will be used instead.");

        return defaultUICulture;
      }
    }

    public static int GetScreenWidth()
    {
      var screenWidth = Screen.PrimaryScreen.Bounds.Width;

      return screenWidth;
    }

    public static int GetScreenHeight()
    {
      var screenHeight = Screen.PrimaryScreen.Bounds.Height;

      return screenHeight;
    }
  }
}