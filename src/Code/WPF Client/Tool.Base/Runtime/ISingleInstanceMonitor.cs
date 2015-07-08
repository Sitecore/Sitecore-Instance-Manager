using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SIM.Tool.Base.Runtime
{
  public interface ISingleInstanceMonitor
  {
    #region Public Events

    event EventHandler AttemptFromAnotherProcess;

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// This method is used to disable the obtained single instance lock. You should use the returned event to wait until monitor is disabled.
    /// </summary>
    /// <returns></returns>
    ManualResetEventSlim EnqueueMonitorDisabling();

    /// <summary>
    /// This method tries to obtain single instance lock. If it fails, it notifies lock holder about the failed attempt.
    /// </summary>
    /// <returns></returns>
    bool TryAcquireOwnershipNotifyLockHolder();

    #endregion
  }
}