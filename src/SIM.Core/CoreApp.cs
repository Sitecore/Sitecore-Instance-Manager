namespace SIM.Core
{
  using System;
  using System.Deployment.Application;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using log4net.Config;
  using log4net.Core;
  using log4net.Layout;
  using log4net.Util;
  using SIM.Core.Logging;
  using SIM.FileSystem;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public static class CoreApp
  {
    public static bool IsFirstRun
    {
      get
      {
        try
        {
          var deployment = ApplicationDeployment.CurrentDeployment;
          if (deployment != null && deployment.IsFirstRun)
          {
            return true;
          }
        }
        catch
        {
          // an error here indicates that it is not click-once deployment
        }

        var fileName = "first-run.txt";
        if (File.Exists(fileName))
        {
          File.Delete(fileName);

          return true;
        }

        return false;
      }
    }

    public static void LogMainInfo()
    {
      try
      {
        var nativeArgs = Environment.GetCommandLineArgs();
        var commandLineArgs = nativeArgs.Skip(1).ToArray();
        var argsToLog = commandLineArgs.Length > 0 ? String.Join("|", commandLineArgs) : "<NO ARGUMENTS>";

        Log.Info("**********************************************************************");
        Log.Info("**********************************************************************");
        Log.Info("Sitecore Instance Manager started");
        Log.Info("Version: {0}", ApplicationManager.AppVersion);
        Log.Info("Revision: {0}", ApplicationManager.AppRevision);
        Log.Info("Label: {0}", ApplicationManager.AppLabel);
        Log.Info("Executable: {0}", nativeArgs.FirstOrDefault() ?? String.Empty);
        Log.Info("Arguments: {0}", argsToLog);
        Log.Info("Directory: {0}", Environment.CurrentDirectory);
        Log.Info("**********************************************************************");
        Log.Info("**********************************************************************");
      }
      catch
      {
        Debug.WriteLine("Error during log main info");
      }
    }

    public static void InitializeLogging()
    {
      Log.Initialize(new Log4NetLogProvider());

      var logConfig = new FileInfo("Log.config");
      if (logConfig.Exists)
      {
        XmlConfigurator.Configure(logConfig);
      }
      else
      {
        var infoLogger = new LogFileAppender
        {
          AppendToFile = true,
          File = "hard-coded",
          Layout = new PatternLayout("%4t %d{ABSOLUTE} %-5p %m%n"),
          SecurityContext = new WindowsSecurityContext(),
          Threshold = Level.Info
        };

        var debugLogger = new LogFileAppender
        {
          AppendToFile = true,
          File = "$(debugPath)",
          Layout = new PatternLayout("%4t %d{ABSOLUTE} %-5p %m%n"),
          SecurityContext = new WindowsSecurityContext(),
          Threshold = Level.Debug
        };

        BasicConfigurator.Configure(infoLogger, debugLogger);
      }
    }

    public static void DeleteTempFolders()
    {
      try
      {
        FileSystem.Local.Directory.DeleteTempFolders();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Deleting temp folders caused an exception");
      }
    }

    public static bool DoNotTrack()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "donottrack.txt");

      return FileSystem.Local.File.Exists(path);
    }

    public static string GetId()
    {
      string cookie = GetCookie();

      return String.Format("public-{0}", cookie);
    }

    [NotNull]
    public static string GetCookie()
    {
      var tempFolder = ApplicationManager.TempFolder;
      var path = Path.Combine(tempFolder, "cookie.txt");
      if (Directory.Exists(tempFolder))
      {
        if (FileSystem.Local.File.Exists(path))
        {
          var cookie = FileSystem.Local.File.ReadAllText(path);
          if (!String.IsNullOrEmpty(cookie))
          {
            return cookie;
          }

          try
          {
            FileSystem.Local.File.Delete(path);
          }
          catch (Exception ex)
          {
            Log.Error(ex, "Cannot delete cookie file");
          }
        }
      }
      else
      {
        Directory.CreateDirectory(tempFolder);
      }

      var newCookie = Guid.NewGuid().ToString().Replace("{", String.Empty).Replace("}", String.Empty).Replace("-", String.Empty);
      try
      {
        FileSystem.Local.File.WriteAllText(path, newCookie);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Cannot write cookie");
      }

      return newCookie;
    }
  }
}
