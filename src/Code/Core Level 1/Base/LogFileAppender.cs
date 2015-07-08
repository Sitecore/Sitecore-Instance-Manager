#region Usings

using System;
using log4net.Appender;

#endregion

namespace SIM.Base
{
  public class LogFileAppender : FileAppender
  {
    protected string originalFileName;

    public override string File
    {

      get { return base.File; }
      set
      {
        if (originalFileName == null)
        {
          originalFileName = value.Equals("$(debugPath)", StringComparison.OrdinalIgnoreCase) ? Log.DebugLogFilePath : Log.LogFilePath;
        }
        base.File = originalFileName;
      }
    }
  }
}
