#region Usings

using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;

#endregion

namespace SIM.Base
{
  #region



  #endregion

  /// <summary>
  ///   The log.
  /// </summary>
  public static class Log
  {
    #region Fields

    /// <summary>
    ///   The logs folder.
    /// </summary>
    public static readonly string LogsFolder = ApplicationManager.LogsFolder;

    #endregion

    static Log()
    {
      XmlConfigurator.Configure(new FileInfo("Log.config"));
    }

    #region Public Methods

    /// <summary>
    /// The error.
    /// </summary>
    /// <param name="message">
    ///   The message. 
    /// </param>
    /// <param name="ownerType"> </param>
    /// <param name="exception"> </param>
    /// <param name="ex">
    /// The ex. 
    /// </param>
    [UsedImplicitly]
    public static void Error(string message, Type ownerType, Exception exception = null)
    {
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(ownerType, "ownerType");

      if (string.IsNullOrEmpty(message)) message = "Exception with no description";
      if (ownerType == null) ownerType = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
      ILog logger = LogManager.GetLogger(ownerType);
      if (logger != null)
      {
        if (exception != null)
        {
          logger.Error(message, exception);
        }
        else
        {
          logger.Error(message);
        }
      }
    }

    /// <summary>
    /// The info.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    public static void Info(string message, Type ownerType)
    {
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(ownerType, "ownerType");

      ILog logger = LogManager.GetLogger(ownerType);
      if (logger != null)
      {
        logger.Info(message);
      }
    }

    /// <summary> 
    /// The warn.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    public static void Warn(string message, Type ownerType, Exception exception = null)
    {
      if (string.IsNullOrEmpty(message)) message = "Exception with no description";
      if (ownerType == null) ownerType = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;

      ILog logger = LogManager.GetLogger(ownerType);
      if (logger != null)
      {
        if (exception != null)
        {
          logger.Warn(message, exception);
        }
        else
        {
          logger.Warn(message);
        }
      }
    }

    #endregion

    /// <summary>
    ///   Gets LogFilePath.
    /// </summary>
    public static string LogFilePath
    {
      get { return Path.Combine(LogsFolder, GetLogFileName()); }
    }

    public static string DebugLogFilePath
    {
      get { return Path.Combine(LogsFolder, GetLogFileName(string.Empty,"_DEBUG")); }
    }

    /// <summary>
    ///   The get log file name.
    /// </summary>
    /// <returns> The get log file name. </returns>
    [NotNull]
    private static string GetLogFileName(string prefix = "", string suffix = "")
    {
      return "{0}{1}{2}.txt".FormatWith(prefix, DateTime.Now.ToString("yyyy-MM-dd"), suffix);
    }

    public static void Debug(string message, Type type = null, bool skipIndent = false)
    {
      Assert.ArgumentNotNull(message, "message");

      ILog logger = LogManager.GetLogger(type ?? typeof(Log));
      if (logger != null)
      {
        logger.Debug(skipIndent ? message : message = ProfileSection.Start + ProfileSection.ShiftLog() + message);// + Environment.NewLine + GetCutStackTrace());        
      }
    }

    [UsedImplicitly]
    private static string GetCutStackTrace()
    {
      var ss = Environment.StackTrace;
      var arr = ss.Split('\n').Reverse().ToArray();
      var s = string.Empty;
      try
      {
        var i = 0;
        while (!arr[i++].Contains("SIM.Tool.App.Main")) ;
        s += Add(i - 1, arr);
        int q = i;
        try
        {
          while (!arr[q++].Contains("SIM.Tool.Base.WindowHelper.ShowDialog"))
          {
          }

          i = q;
          s += Add(i - 1, arr);
        }
        catch { }
        while (!arr[i++].Contains("SIM.")) ;
        for (int j = arr.Length - 3; j >= i; --j)
        {
          s += Add(j, arr);
        }
      }
      catch (Exception ex)
      {
        Log.Error("Exception", typeof(Log), ex);
      }
      return s.EmptyToNull() ?? ss;
    }

    private static string Add(int j, string[] arr)
    {
      return arr[j] + '\n';
    }

    public static void Info(string message, object owner)
    {
      Info(message, owner.GetType());
    }

    public static void Warn(string formatWith, object ownerType, Exception exception)
    {
      Warn(formatWith, ownerType.GetType(), exception);
    }

    public static void Warn(string message, object ownerType)
    {
      Warn(message, ownerType.GetType());
    }

    public static void Error(string message, object ownerType, Exception exception = null)
    {
      Error(message, ownerType.GetType(), exception);
    }
  }
}