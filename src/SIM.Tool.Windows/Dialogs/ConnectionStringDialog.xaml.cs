namespace SIM.Tool.Windows.Dialogs
{
  using System;
  using System.Data.SqlClient;
  using System.Windows;
  using System.Windows.Input;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public partial class ConnectionStringDialog
  {
    #region Constructors

    public ConnectionStringDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    private void CancelChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.Close();
    }

    private void SaveChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
      {
        DataSource = this.dataSource.Text
      };

      if (this.useSqlServerLogin.IsChecked == true)
      {
        builder.UserID = this.userId.Text;
        builder.Password = this.password.Text;
      }
      else
      {
        builder.IntegratedSecurity = true;
      }

      var value = builder.ToString();

      /* hack for settings dialog */
      SettingsDialog owner = this.Owner as SettingsDialog;
      if (owner != null)
      {
        WindowHelper.SetTextboxTextValue(owner.ConnectionString, value, owner.DoneButton);
      }

      this.DataContext = value;
      this.DialogResult = true;
      this.Close();
    }

    private void WindowContentRendered([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this.DataContext as string);
      this.dataSource.Text = builder.DataSource.EmptyToNull() ?? ".\\SQLEXPRESS";
      this.userId.Text = builder.UserID.EmptyToNull() ?? "sa";
      this.password.Text = builder.Password.EmptyToNull() ?? "12345";
      this.useSqlServerLogin.IsChecked = !builder.IntegratedSecurity;
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      if (e.Key == Key.Escape)
      {
        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        this.Close();
      }
    }

    #endregion
  }
}