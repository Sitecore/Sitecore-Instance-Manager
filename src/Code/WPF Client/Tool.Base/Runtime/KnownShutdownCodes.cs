using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Tool.Base.Runtime
{
  public static class KnownShutdownCodes
  {
    #region Constants

    /// <summary>
    /// Application exists with this code if another instance is run
    /// </summary>
    public const int ExitAsDuplicate = -9;

    /// <summary>
    /// Application exists with this code if it's going to be restarted
    /// </summary>
    public const int ExitToRestart = -2;

    /// <summary>
    /// Application exists with this code by default.
    /// </summary>
    public const int RegularExit = 0;

    /// <summary>
    /// Application exists with this code if shutdown is initiated by user (i.e. from window close or from tray).
    /// </summary>
    public const int UserExit = -1;

    #endregion
  }
}