namespace SIM.Tool.Windows.Dialogs
{
  using System.Diagnostics;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Navigation;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  public partial class AboutDialog
  {
    #region Constructors

    public AboutDialog()
    {
      this.InitializeComponent();
      this.VersionNumber.Text = ApplicationManager.AppVersion;
      this.RevisionNumber.Text = ApplicationManager.AppRevision;

      var label = ApplicationManager.AppLabel;
      if (!string.IsNullOrEmpty(label))
      {
        this.UpdateNumber.Text = label;
      }
      else
      {
        this.Update.Visibility = Visibility.Hidden;
      }
    }

    #endregion

    #region Private methods

    private void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      CoreApp.RunApp(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      if (e.Key != Key.Escape)
      {
        return;
      }

      if (e.Handled)
      {
        return;
      }

      e.Handled = true;
      this.Close();
    }

    #endregion
  }
}