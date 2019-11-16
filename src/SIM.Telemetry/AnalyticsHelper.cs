namespace SIM.Telemetry
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using System;
  using System.Globalization;
  using System.IO;
  using System.Windows.Forms;

  public static class AnalyticsHelper
  {    
    public static Guid GetDeviceId(string temporaryFolder)
    {
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
            Log.Error(ex, $"Cannot delete '{fileName}' file");
            Log.Debug($"Message:{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
        Log.Error(ex, $"Cannot write device id to '{fileName}' file");
        Log.Debug($"Message:{ex.Message}{Environment.NewLine}{ex.StackTrace}");
      }

      return newDeviceId;
    }

    [NotNull]
    public static string GetCurrentUICulture()
    {
      var defaultUICulture = "en";

      CultureInfo currentUICulture;

      try
      {
        currentUICulture = CultureInfo.CurrentUICulture;

        return currentUICulture.ToString();
      }
      catch (Exception ex)
      {
        Log.Debug($"CurrentUICulture cannot be retrieved. The {defaultUICulture} culture will be used instead.{Environment.NewLine}Message:{ex.Message}{Environment.NewLine}{ex.StackTrace}");

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