using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SIM.Adapters.SqlServer;
using SIM.Tool.Base.Profiles;
using System.Data;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DatabaseQueryExecutionDialog.xaml
    /// </summary>
    public partial class DatabaseQueryExecutionDialog : Window
    {
        #region Fields
        DataTable queryResults = new DataTable();
        #endregion
        public DatabaseQueryExecutionDialog()
        {
            InitializeComponent();
        }
        public DatabaseQueryExecutionDialog(string defaultQueryValue)
        {
            InitializeComponent();
            databaseQueryInput.Text = defaultQueryValue;
        }

        
        private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                queryResults = SqlServerManager.Instance.GetResultOfQueryExecution(ProfileManager.GetConnectionString(), databaseQueryInput.Text);
                dataGrid1.ItemsSource = queryResults.DefaultView;
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
                    queryResults = SqlServerManager.Instance.GetResultOfQueryExecution(ProfileManager.GetConnectionString(), databaseQueryInput.Text);
                    dataGrid1.ItemsSource = queryResults.DefaultView;
                }
                catch (Exception ex)
                {
                    WindowHelper.ShowMessage(ex.Message);
                }
            }
        }

    }
}
