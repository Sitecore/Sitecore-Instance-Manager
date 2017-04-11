namespace SIM.Tool.Base.Windows.Dialogs
{
  using System;
  using System.ComponentModel;
  using System.Runtime.InteropServices;
  using System.Threading;
  using Sitecore.Diagnostics.Logging;

  public partial class ProgressDialog
  {
    #region Fields

    private Thread Bw { get; }
    private bool _IsClosing = false;

    #endregion

    #region Constructors

    public ProgressDialog(Action act)
    {
      ThreadStart th = delegate
      {
        try
        {
          act();
        }
        catch (ThreadInterruptedException)
        {
          Log.Info("The thread was interrupted");
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("The long running operation caused an exception", true, ex);
        }
        finally
        {
          this._IsClosing = true;
          this.Dispatcher.Invoke(new Action(this.Close));
        }
      };
      this.Bw = new Thread(th);
      this.InitializeComponent();
    }

    #endregion

    #region Protected methods

    protected override void OnSourceInitialized(EventArgs e)
    {
      IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
      SetWindowLong(hwnd, -16, 
        GetWindowLong(hwnd, -16) & (0xFFFFFFFF ^ 0x80000));

      base.OnSourceInitialized(e);
    }

    #endregion

    #region Private methods

    [DllImport("user32.dll")]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    private void Start(object sender, EventArgs e)
    {
      this.Bw.Start();
    }

    private void Terminate(object sender, CancelEventArgs cancelEventArgs)
    {
      if (this._IsClosing)
      {
        return;
      }

      this.Bw.Interrupt();
      this._IsClosing = true;
      cancelEventArgs.Cancel = true;
    }

    private void Terminate(object sender, EventArgs cancelEventArgs)
    {
      if (this._IsClosing)
      {
        return;
      }

      this._IsClosing = true;
      this.Bw.Interrupt();
    }

    #endregion
  }
}