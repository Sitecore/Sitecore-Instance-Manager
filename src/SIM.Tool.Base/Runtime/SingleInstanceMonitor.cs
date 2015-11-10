namespace SIM.Tool.Base.Runtime
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;

  public class SingleInstanceMonitor : ISingleInstanceMonitor
  {
    #region Constants

    protected const int DISPOSED_CODE_DISPOSED = 1;
    protected const int DISPOSED_CODE_DISPOSING = 0;
    protected const int DISPOSED_CODE_NOTDISPOSED = -1;

    #endregion

    #region Fields

    protected static readonly string EventGlobalName = @"Local\SitecoreInstanceManager_SingleInstanceLock";

    protected volatile int disposed;

    protected volatile ManualResetEventSlim disposedWaitHandle;

    protected volatile EventWaitHandle innerHandle;

    #endregion

    #region Constructors

    public SingleInstanceMonitor()
    {
      this.disposed = DISPOSED_CODE_NOTDISPOSED;
      this.disposedWaitHandle = new ManualResetEventSlim(false);
    }

    #endregion

    #region Public Events

    public event EventHandler AttemptFromAnotherProcess;

    #endregion

    #region Public Methods and Operators

    public ManualResetEventSlim EnqueueMonitorDisabling()
    {
      Assert.IsNotNull(this.innerHandle, "Monitor should be initialized before");
      if (this.disposed == DISPOSED_CODE_NOTDISPOSED)
      {
        // We shouuld ensure that we do disposing only once
        if (Interlocked.CompareExchange(ref this.disposed, DISPOSED_CODE_DISPOSING, DISPOSED_CODE_NOTDISPOSED) == DISPOSED_CODE_NOTDISPOSED)
        {
          // this set of lines starts monitor disabling in awaiting loop
          this.innerHandle.Set();
        }
      }

      return this.disposedWaitHandle;
    }

    public virtual bool TryAcquireOwnershipNotifyLockHolder()
    {
      Assert.IsTrue(this.innerHandle == null, "Method TryAcquireOwnership() should be called only once.");
      bool createdNewEvent;
      this.innerHandle = new EventWaitHandle(false, EventResetMode.AutoReset, EventGlobalName, out createdNewEvent);
      if (!createdNewEvent)
      {
        // Notify another process about our attempt
        this.innerHandle.Set();
        this.innerHandle.Close();

        // We set state to "Disposed"
        this.disposed = DISPOSED_CODE_DISPOSED;
        this.disposedWaitHandle.Set();
        return false;
      }

      // Log.Info("SingleInstanceMonitor: ownership acquired");
      this.StartAwaitingLoop();
      return true;
    }

    #endregion

    #region Methods

    protected virtual void ForeignEventAwaitingLoop()
    {
      try
      {
        Assert.IsNotNull(this.innerHandle, "Inner handle cannot be null at this point");
        while (this.disposed != DISPOSED_CODE_DISPOSED)
        {
          this.innerHandle.WaitOne();
          if (this.disposed == DISPOSED_CODE_NOTDISPOSED)
          {
            // Log.Info("SingleInstanceMonitor: Event from foreign process received");
            this.NotifyAboutForeignEvent();
          }
            
            // this.disposed == 0. Disposing in progress.
            // Value cannot be 1, because while() condition cannot allow this.
          else
          {
            // Log.Info("SingleInstanceMonitor: disposing");
            this.innerHandle.Close();
            this.disposed = DISPOSED_CODE_DISPOSED;
            this.disposedWaitHandle.Set();
          }
        }

        Assert.IsTrue(this.innerHandle.SafeWaitHandle.IsClosed, "Handle should be closed at this point");
      }
      catch (ThreadAbortException)
      {
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Unexpected error in SingleInstanceMonitor awaiting loop");
      }
      finally
      {
        if (!this.innerHandle.SafeWaitHandle.IsClosed)
        {
          this.innerHandle.Close();
        }

        this.disposed = DISPOSED_CODE_DISPOSED;
        this.disposedWaitHandle.Set();
      }
    }

    protected virtual void NotifyAboutForeignEvent()
    {
      Task.Factory.StartNew(this.OnAttemptFromAnotherProcess);
    }

    protected virtual void OnAttemptFromAnotherProcess()
    {
      EventHandler handler = this.AttemptFromAnotherProcess;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    protected virtual void StartAwaitingLoop()
    {
      var checkingThread = new Thread(this.ForeignEventAwaitingLoop)
      {
        IsBackground = true
      };
      checkingThread.Start();
    }

    #endregion
  }
}