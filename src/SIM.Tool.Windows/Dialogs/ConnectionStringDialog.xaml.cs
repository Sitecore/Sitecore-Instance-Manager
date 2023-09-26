namespace SIM.Tool.Windows.Dialogs
{
  using System;
  using System.Data.SqlClient;
  using System.Windows;
  using System.Windows.Input;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;
  using System.Text.RegularExpressions;

  #region

  #endregion

  public partial class ConnectionStringDialog
  {
    private const string PasswordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@$#%])[A-Za-z\d!@$#%]{8,}$";

    #region Constructors

    public ConnectionStringDialog()
    {
      InitializeComponent();
    }

    #endregion

    #region Methods

    private void CancelChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      Close();
    }

    private void SaveChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      if (!IsPasswordValid(password.Text))
      {
        if (WindowHelper.ShowMessage("The entered password may not meet the following operating system policy requirements:\n" +
          "- minimum length of 8 characters\n" +
          "- at least one uppercase letter\n" +
          "- at least one lowercase letter\n" +
          "- at least one digit\n" +
          "- at least one special character\n\n" +
          "Do you want to proceed?",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
          return;
        }
      }

      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
      {
        DataSource = dataSource.Text
      };

      if (useSqlServerLogin.IsChecked == true)
      {
        builder.UserID = userId.Text;
        builder.Password = password.Text;
      }
      else
      {
        builder.IntegratedSecurity = true;
      }

      var value = builder.ToString();

      /* hack for settings dialog */
      SettingsDialog owner = Owner as SettingsDialog;
      if (owner != null)
      {
        WindowHelper.SetTextboxTextValue(owner.ConnectionString, value, owner.DoneButton);
      }

      DataContext = value;
      DialogResult = true;
      Close();
    }

    private void WindowContentRendered([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DataContext as string);
      dataSource.Text = builder.DataSource.EmptyToNull() ?? ".\\SQLEXPRESS";
      userId.Text = builder.UserID.EmptyToNull() ?? "sa";
      password.Text = builder.Password.EmptyToNull() ?? "12345";
      useSqlServerLogin.IsChecked = !builder.IntegratedSecurity;
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      if (e.Key == Key.Escape)
      {
        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        Close();
      }
    }

    private bool IsPasswordValid(string password)
    {
      return Regex.IsMatch(password, PasswordPattern);
    }

    #endregion
  }
}