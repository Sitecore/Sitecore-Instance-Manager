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

namespace SIM.Tool.Plugins.DatabaseManager.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DatabasesOperationsDialog.xaml
    /// </summary>
    public partial class DatabasesOperationsDialog : Window
    {
        public DatabasesOperationsDialog()
        {
            InitializeComponent();
        }

        public DatabasesOperationsDialog(string name)
        {             
            InitializeComponent();
            dbName.Text = name;
        }

        public DatabasesOperationsDialog(string name, string path)
        {            
            InitializeComponent();
            dbName.Text = name;
            dbPath.Text = path;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog
        }
    }
}
