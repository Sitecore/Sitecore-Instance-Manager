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
        var argsToLog = commandLineArgs.Length > 0 ? string.Join("|", commandLineArgs) : "<NO ARGUMENTS>";

        Log.Info("**********************************************************************");
        Log.Info("**********************************************************************");
        Log.Info("Sitecore Instance Manager started");
        Log.Info("Version: {0}", ApplicationManager.AppVersion);
        Log.Info("Revision: {0}", ApplicationManager.AppRevision);
        Log.Info("Label: {0}", ApplicationManager.AppLabel);
        Log.Info("Executable: {0}", nativeArgs.FirstOrDefault() ?? string.Empty);
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
  }
}
