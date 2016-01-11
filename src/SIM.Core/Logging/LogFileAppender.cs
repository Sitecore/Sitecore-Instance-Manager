namespace SIM.Core.Logging
{
  using System;
  using System.IO;
  using log4net.Appender;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class LogFileAppender : FileAppender
  {
    #region Fields

    protected string originalFileName;

    #endregion

    #region Public properties

    [CanBeNull]
    public override string File
    {
      get
      {
        return base.File;
      }

      set
      {
        if (this.originalFileName == null)
        {
          this.originalFileName = Path.Combine(ApplicationManager.LogsFolder, value.Equals("$(debugPath)", StringComparison.OrdinalIgnoreCase) ? GetLogFileName(string.Empty, "_DEBUG") : GetLogFileName());
        }

        base.File = this.originalFileName;
      }
    }

    #endregion

    [NotNull]
    private static string GetLogFileName([CanBeNull] string prefix = null, [CanBeNull] string suffix = null)
    {
      return "{0}{1}{2}.txt".FormatWith(prefix, DateTime.Now.ToString("yyyy-MM-dd"), suffix);
    }
  }
}