namespace SIM.Core.Logging
{
  using System;
  using System.IO;
  using log4net.Appender;
  using JetBrains.Annotations;
  using SIM.Extensions;

  [UsedImplicitly]
  public class LogFileAppender : FileAppender
  {
    #region Fields

    protected string expandedFilePath;

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
        if (expandedFilePath == null)
        {
          expandedFilePath = value
            .Replace("$(logFolder)", ApplicationManager.LogsFolder)
            .Replace("$(currentFolder)", Environment.CurrentDirectory)
            .PipeTo(t => string.Format(t ?? "", DateTime.Now))            
            .PipeTo(Environment.ExpandEnvironmentVariables);
        }

        base.File = expandedFilePath;
      }
    }

    #endregion     
  }
}