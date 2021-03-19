using System.Windows;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.Dialogs
{
  /// <summary>
  /// Interaction logic for CredentialsDialog.xaml
  /// </summary>
  public partial class CredentialsDialog : Window
  {
    public CredentialsDialog()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      CredentialsContext credentialsContext = this.DataContext as CredentialsContext;
      Assert.ArgumentNotNull(credentialsContext, nameof(credentialsContext));

      UserNameText.Text = credentialsContext.UserName;
      PasswordText.Text = credentialsContext.Password;
      if (string.IsNullOrEmpty(credentialsContext.Uri))
      {
        IdentityServerUriRow.Height = new GridLength(0);
        this.Height = 112;
      }
      else
      {
        IdentityServerUriText.Text = credentialsContext.Uri;
      }
    }

    private void OK_OnClick(object sender, RoutedEventArgs e)
    {
      this.DataContext = new CredentialsContext(UserNameText.Text, PasswordText.Text, IdentityServerUriText.Text);
      this.DialogResult = true;
      this.Close();
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}