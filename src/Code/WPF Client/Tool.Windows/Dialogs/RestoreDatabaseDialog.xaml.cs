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
using SIM.Tool.Base;
using Microsoft.Win32;
using SIM.Base;
using System.Data.SqlClient;
using SIM.Tool.Windows.Dialogs;

namespace SIM.Tool.Windows.Windows.Dialogs
{
  /// <summary>
  /// Interaction logic for DatabasesOperationsDialog.xaml
  /// </summary>
  public partial class DatabasesOperationsDialog : Window
  {
    #region Fields

    public string initialPath = ProfileManager.Profile.InstancesFolder;
    SqlServerManager.BackupInfo bakInfo = new SqlServerManager.BackupInfo();

    #endregion
    #region Constructors
    public DatabasesOperationsDialog()
    {
      InitializeComponent();
    }

    public DatabasesOperationsDialog(string initialDirectory)
    {
      InitializeComponent();
      initialPath = initialDirectory;
    }

    public DatabasesOperationsDialog(string name, string pathFrom, string pathTo)
    {
      InitializeComponent();
      dbName.Text = name;
      dbPathFrom.Text = pathFrom;
      dbPathTo.Text = pathTo;
    }
    #endregion
    #region Methods

    private void selectPathFrom_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new OpenFileDialog
      {
        AddExtension = true,
        CheckPathExists = true,
        Filter = "Backup (*.bak) | *.bak; | All Files (*.*) | *.*",
        InitialDirectory = initialPath,
        Title = "Specify backup file"
      };
      bool? showDialog = dialog.ShowDialog();
      if (showDialog.Value != true)
      {
        return;
      }
      dbPathFrom.Text = dialog.FileName;
      try
      {
        bakInfo = SqlServerManager.Instance.GetDatabasesNameFromBackup(ProfileManager.GetConnectionString(), dialog.FileName);
        dbPathTo.Text = System.IO.Path.GetDirectoryName( bakInfo.physicalNameMdf );
        dbName.Text =bakInfo.dbOriginalName ;
        fileName.Text = bakInfo.GetDatabaseName() + ".mdf";
      }
      catch (Exception ex)
      {
        Log.Warn(String.Format("Cannot get information from '{0}' backup", dialog.FileName), typeof(int), ex);
        WindowHelper.ShowMessage("Cannot get information from backup!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
      }
    }

    private void selectPathTo_Click(object sender, RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose folder for restoring", this.dbPathTo, this.selectPathTo, initialPath);
    }

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
      var connectionString = ProfileManager.GetConnectionString();
      string databaseName = dbName.Text;
      string dbFileName = fileName.Text;
      string pathFrom = dbPathFrom.Text;
      string pathTo = dbPathTo.Text;


      if (!dbName.Text.IsNullOrEmpty() && !dbPathFrom.Text.IsNullOrEmpty() && !dbPathTo.Text.IsNullOrEmpty())
      {
        try
        {
          WindowHelper.LongRunningTask(() => RestoreDatabase(dbFileName, databaseName, pathFrom, pathTo, connectionString), "Restore database", this, "The database is being restored", null, true);
        }
        catch (SqlException exception)
        {
          WindowHelper.ShowMessage(exception.Message);
        }
        DatabasesDialog.lastPathToRestore = pathFrom;
        this.Close();
      }
      else
      {
        WindowHelper.ShowMessage("Please fill all fields!");
      }
    }

    private void RestoreDatabase(string dbFileName, string databaseName, string pathFrom, string pathTo, SqlConnectionStringBuilder connectionString)
    {
      try
      {
        if (dbFileName.IsNullOrEmpty())
        {
          SqlServerManager.Instance.RestoreDatabase(databaseName, connectionString, pathFrom, pathTo, bakInfo);
        }
        else
        {
          SqlServerManager.Instance.RestoreDatabase(databaseName, dbFileName, connectionString, pathFrom, pathTo, bakInfo);
        }
      }
      catch (Exception exception)
      {
        WindowHelper.ShowMessage(exception.Message);
      }
    }  
    #endregion
  }
}
