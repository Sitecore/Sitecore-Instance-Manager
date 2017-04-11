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

    private DataTable _QueryResults = new DataTable();

    #endregion

    #region Constructors

    public DatabaseQueryExecutionDialog()
    {
      InitializeComponent();
    }

    public DatabaseQueryExecutionDialog(string defaultQueryValue)
    {
      InitializeComponent();
      databaseQueryInput.Text = defaultQueryValue;
    }

    #endregion

    #region Private methods

    private void ExecuteQueryClick(object sender, RoutedEventArgs e)
    {
      try
      {
        _QueryResults = SqlServerManager.Instance.GetResultOfQueryExecution(ProfileManager.GetConnectionString(), databaseQueryInput.Text);
        dataGrid1.ItemsSource = _QueryResults.DefaultView;
      }
      catch (Exception ex)
      {
        WindowHelper.ShowMessage(ex.Message);
      }
    }

    private void DatabaseQueryInputKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.F5)
      {
        try
        {
          _QueryResults = SqlServerManager.Instance.GetResultOfQueryExecution(ProfileManager.GetConnectionString(), databaseQueryInput.Text);
          dataGrid1.ItemsSource = _QueryResults.DefaultView;
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