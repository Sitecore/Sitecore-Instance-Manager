#region Usings

using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using SIM.Base;
using SIM.Tool.Base;

#endregion

namespace SIM.Tool.Windows.Dialogs
{
  #region

  

  #endregion

  /// <summary>
  ///   The connection string dialog.
  /// </summary>
  public partial class ConnectionStringDialog
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConnectionStringDialog" /> class.
    /// </summary>
    public ConnectionStringDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Cancels the changes.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void CancelChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Saves the changes.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void SaveChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder {DataSource = this.dataSource.Text};

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

    /// <summary>
    /// Windows the content rendered.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data. 
    /// </param>
    private void WindowContentRendered([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this.DataContext as string);
      this.dataSource.Text = builder.DataSource.EmptyToNull() ?? ".\\SQLEXPRESS";
      this.userId.Text = builder.UserID.EmptyToNull() ?? "sa";
      this.password.Text = builder.Password.EmptyToNull() ?? "12345";
      this.useSqlServerLogin.IsChecked = !builder.IntegratedSecurity;
    }

    /// <summary>
    /// Windows the key up.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data. 
    /// </param>
    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      if (e.Key == Key.Escape)
      {
        if (e.Handled) return;
        e.Handled = true;
        this.Close();
      }
    }

    #endregion
  }
}