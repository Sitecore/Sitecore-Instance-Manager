namespace SIM.Tool.Base.Runtime
{
  using System;
  using System.Diagnostics;
  using System.Globalization;
  using System.Threading;
  using System.Windows;

  public static class LifeManager
  {
    #region Constants

    public const string ParentSIMArgumentName = "ParentSIMPID";

    #endregion

    #region Constructors

    static LifeManager()
    {
      SingleInstanceLock = new SingleInstanceMonitor();
    }

    #endregion

    #region Public Events

    public static event EventHandler OpenAttemptFromAnotherInstance
    {
      add
      {
        SingleInstanceLock.AttemptFromAnotherProcess += value;
      }

      remove
      {
        SingleInstanceLock.AttemptFromAnotherProcess -= value;
      }
    }

    #endregion

    #region Public Properties

    public static bool IsRestarting { get; set; }

    public static ISingleInstanceMonitor SingleInstanceLock { get; set; }
    public static SynchronizationContext UISynchronizationContext { get; set; }

    #endregion

    #region Public Methods and Operators

    public static bool AcquireSingleInstanceLock()
    {
      return SingleInstanceLock.TryAcquireOwnershipNotifyLockHolder();
    }

    public static void ReleaseSingleInstanceLock()
    {
      ManualResetEventSlim waitHandle = SingleInstanceLock.EnqueueMonitorDisabling();
      waitHandle.Wait();
    }

    public static void RestartApplication()
    {
      // Stop single instance monitor
      ReleaseSingleInstanceLock();

      // Set global flag
      IsRestarting = true;

      // Perform actual restart
      WindowHelper.RunApp(Application.ResourceAssembly.Location, ParentSIMArgumentName, Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));
      LifeManager.ShutdownApplication(KnownShutdownCodes.ExitToRestart);
    }

    public static void ShutdownApplication(int exitCode = KnownShutdownCodes.RegularExit)
    {
      Application.Current.Shutdown(exitCode);
    }

    // Is actual only in case of restart. This method checks arguments and ensure that parent instance exited.
    public static void WaitUntilOriginalInstanceExits(string[] commandArgs)
    {
      int parentSIMPID = -1;
      for (int i = 0; i < commandArgs.Length; i++)
      {
        string currentCommandArg = commandArgs[i];
        if (currentCommandArg.Equals(ParentSIMArgumentName, StringComparison.OrdinalIgnoreCase) && (i + 1 < commandArgs.Length))
        {
          var pidArg = commandArgs[i + 1];
          if (!int.TryParse(pidArg, out parentSIMPID))
          {
            parentSIMPID = -1;
          }

          break;
        }
      }

      if (parentSIMPID != -1)
      {
        WaitForSIMInstanceExit(parentSIMPID);
      }
    }

    #endregion

    #region Methods

    private static void WaitForSIMInstanceExit(int pid)
    {
      try
      {
        Process foreignSimProcess = Process.GetProcessById(pid);

        if (!foreignSimProcess.ProcessName.ToUpperInvariant().Contains("SIM"))
        {
          return;
        }

        if (!foreignSimProcess.WaitForExit(60000))
        {
          Log.Warn("Unexpected awaiting time of another SIM exit", typeof(LifeManager));
        }
      }
      catch (ArgumentException ex)
      {
        // Ignore. Process has already exited
      }
      catch (Exception ex)
      {
        Log.Error("Unexpected exception during another SIM exit awaiting", typeof(LifeManager), ex);
      }
    }

    #endregion
  }
}