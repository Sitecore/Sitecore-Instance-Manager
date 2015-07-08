#region Usings

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using SIM.Base;

#endregion

namespace SIM.Tool.Base.Windows.Dialogs
{
  /// <summary>
  /// Interaction logic for ProgressDialog.xaml
  /// </summary>
  public partial class ProgressDialog
  {
    private readonly Thread bw;
    private bool isClosing = false;
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
                             Log.Info("The thread was interrupted", this);
                           }
                           catch (Exception ex)
                           {
                             WindowHelper.HandleError("The long running operation caused an exception", true, ex, this);
                           }
                           finally
                           {
                             isClosing = true;
                             this.Dispatcher.Invoke(new Action(Close));
                           }
                         };
      bw = new Thread(th);
      InitializeComponent();
    }

    private void Terminate(object sender, CancelEventArgs cancelEventArgs)
    {
      if(isClosing) return;
      bw.Interrupt();
      isClosing = true;
      cancelEventArgs.Cancel = true;
    }

    private void Terminate(object sender, EventArgs cancelEventArgs)
    {
      if (isClosing) return;
      isClosing = true;
      bw.Interrupt();
    }

    private void Start(object sender, EventArgs e)
    {
      bw.Start();
    }

    [DllImport("user32.dll")]
    static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    
    protected override void OnSourceInitialized(EventArgs e)
    {
      IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
      SetWindowLong(hwnd, -16,
          GetWindowLong(hwnd, -16) & (0xFFFFFFFF ^ 0x80000));

      base.OnSourceInitialized(e);
    }
  }
}
