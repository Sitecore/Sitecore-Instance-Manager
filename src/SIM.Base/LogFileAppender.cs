using System;
using log4net.Appender;

namespace SIM
{
  public class LogFileAppender : FileAppender
  {
    #region Fields

    protected string originalFileName;

    #endregion

    #region Public properties

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
          this.originalFileName = value.Equals("$(debugPath)", StringComparison.OrdinalIgnoreCase) ? Log.DebugLogFilePath : Log.LogFilePath;
        }

        base.File = this.originalFileName;
      }
    }

    #endregion
  }
}