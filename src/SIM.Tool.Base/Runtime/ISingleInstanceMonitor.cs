namespace SIM.Tool.Base.Runtime
{
  using System;
  using System.Threading;

  public interface ISingleInstanceMonitor
  {
    #region Public Events

    event EventHandler AttemptFromAnotherProcess;

    #endregion

    #region Public Methods and Operators

    ManualResetEventSlim EnqueueMonitorDisabling();

    bool TryAcquireOwnershipNotifyLockHolder();

    #endregion
  }
}