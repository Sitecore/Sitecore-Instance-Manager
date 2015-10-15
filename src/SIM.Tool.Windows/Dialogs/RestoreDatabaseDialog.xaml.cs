using System;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Win32;
using SIM.Adapters.SqlServer;
using SIM.Tool.Base;
using SIM.Tool.Base.Profiles;

namespace SIM.Tool.Windows.Dialogs
{
  using Sitecore.Diagnostics.Logging;

  public partial class RestoreDatabaseDialog : Window
  {
    #region Fields

    public string initialPath = ProfileManager.Profile.InstancesFolder;
    private SqlServerManager.BackupInfo bakInfo = new SqlServerManager.BackupInfo();

    #endregion

    #region Constructors

    public RestoreDatabaseDialog()
    {
      this.InitializeComponent();
    }

    public RestoreDatabaseDialog(string initialDirectory)
    {
      this.InitializeComponent();
      this.initialPath = initialDirectory;
    }

    public RestoreDatabaseDialog(string name, string pathFrom, string pathTo)
    {
      this.InitializeComponent();
      this.dbName.Text = name;
      this.dbPathFrom.Text = pathFrom;
      this.dbPathTo.Text = pathTo;
    }

    #endregion

    #region Methods

    private void RestoreDatabase(string dbFileName, string databaseName, string pathFrom, string pathTo, SqlConnectionStringBuilder connectionString)
    {
      try
      {
        if (dbFileName.IsNullOrEmpty())
        {
          SqlServerManager.Instance.RestoreDatabase(databaseName, connectionString, pathFrom, pathTo, this.bakInfo);
        }
        else
        {
          SqlServerManager.Instance.RestoreDatabase(databaseName, dbFileName, connectionString, pathFrom, pathTo, this.bakInfo);
        }
      }
      catch (Exception exception)
      {
        WindowHelper.ShowMessage(exception.Message);
      }
    }

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
      var connectionString = ProfileManager.GetConnectionString();
      string databaseName = this.dbName.Text;
      string dbFileName = this.fileName.Text;
      string pathFrom = this.dbPathFrom.Text;
      string pathTo = this.dbPathTo.Text;


      if (!this.dbName.Text.IsNullOrEmpty() && !this.dbPathFrom.Text.IsNullOrEmpty() && !this.dbPathTo.Text.IsNullOrEmpty())
      {
        try
        {
          WindowHelper.LongRunningTask(() => this.RestoreDatabase(dbFileName, databaseName, pathFrom, pathTo, connectionString), "Restore database", this, "The database is being restored", null, true);
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

    private void selectPathFrom_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new OpenFileDialog
      {
        AddExtension = true, 
        CheckPathExists = true, 
        Filter = "Backup (*.bak) | *.bak; | All Files (*.*) | *.*", 
        InitialDirectory = this.initialPath, 
        Title = "Specify backup file"
      };
      bool? showDialog = dialog.ShowDialog();
      if (showDialog.Value != true)
      {
        return;
      }

      this.dbPathFrom.Text = dialog.FileName;
      try
      {
        this.bakInfo = SqlServerManager.Instance.GetDatabasesNameFromBackup(ProfileManager.GetConnectionString(), dialog.FileName);
        this.dbPathTo.Text = System.IO.Path.GetDirectoryName(this.bakInfo.physicalNameMdf);
        this.dbName.Text = this.bakInfo.dbOriginalName;
        this.fileName.Text = this.bakInfo.GetDatabaseName() + ".mdf";
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "Cannot get information from '{0}' backup", dialog.FileName);
        WindowHelper.ShowMessage("Cannot get information from backup!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
      }
    }

    private void selectPathTo_Click(object sender, RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose folder for restoring", this.dbPathTo, this.selectPathTo, this.initialPath);
    }

    #endregion
  }
}