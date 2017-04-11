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

  #region

  #endregion

  public partial class ConnectionStringDialog
  {
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

    #endregion
  }
}