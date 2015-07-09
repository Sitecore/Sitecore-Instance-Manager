using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using SIM.Adapters.SqlServer;
using SIM.Tool.Base;
using SIM.Tool.Base.Profiles;

namespace SIM.Tool.Windows.Dialogs
{
  public partial class DatabaseQueryExecutionDialog : Window
  {
    #region Fields

    private DataTable queryResults = new DataTable();

    #endregion

    #region Constructors

    public DatabaseQueryExecutionDialog()
    {
      this.InitializeComponent();
    }

    public DatabaseQueryExecutionDialog(string defaultQueryValue)
    {
      this.InitializeComponent();
      this.databaseQueryInput.Text = defaultQueryValue;
    }

    #endregion

    #region Private methods

    private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        this.queryResults = SqlServerManager.Instance.GetResultOfQueryExecution(ProfileManager.GetConnectionString(), this.databaseQueryInput.Text);
        this.dataGrid1.ItemsSource = this.queryResults.DefaultView;
      }
      catch (Exception ex)
      {
        WindowHelper.ShowMessage(ex.Message);
      }
    }

    private void databaseQueryInput_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.F5)
      {
        try
        {
          this.queryResults = SqlServerManager.Instance.GetResultOfQueryExecution(ProfileManager.GetConnectionString(), this.databaseQueryInput.Text);
          this.dataGrid1.ItemsSource = this.queryResults.DefaultView;
        }
        catch (Exception ex)
        {
          WindowHelper.ShowMessage(ex.Message);
        }
      }
    }

    #endregion
  }
}