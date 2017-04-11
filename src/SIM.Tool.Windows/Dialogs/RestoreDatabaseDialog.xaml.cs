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
  using SIM.Extensions;

  public partial class RestoreDatabaseDialog : Window
  {
    #region Fields

    public string _InitialPath = ProfileManager.Profile.InstancesFolder;
    private SqlServerManager.BackupInfo _BakInfo = new SqlServerManager.BackupInfo();

    #endregion

    #region Constructors

    public RestoreDatabaseDialog()
    {
      this.InitializeComponent();
    }

    public RestoreDatabaseDialog(string initialDirectory)
    {
      this.InitializeComponent();
      this._InitialPath = initialDirectory;
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
          SqlServerManager.Instance.RestoreDatabase(databaseName, connectionString, pathFrom, pathTo, this._BakInfo);
        }
        else
        {
          SqlServerManager.Instance.RestoreDatabase(databaseName, dbFileName, connectionString, pathFrom, pathTo, this._BakInfo);
        }
      }
      catch (Exception exception)
      {
        WindowHelper.ShowMessage(exception.Message);
      }
    }

    private void BtnOkClick(object sender, RoutedEventArgs e)
    {
      var connectionString = ProfileManager.GetConnectionString();
      var databaseName = this.dbName.Text;
      var dbFileName = this.fileName.Text;
      var pathFrom = this.dbPathFrom.Text;
      var pathTo = this.dbPathTo.Text;


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

        DatabasesDialog._LastPathToRestore = pathFrom;
        this.Close();
      }
      else
      {
        WindowHelper.ShowMessage("Please fill all fields!");
      }
    }

    private void SelectPathFromClick(object sender, RoutedEventArgs e)
    {
      var dialog = new OpenFileDialog
      {
        AddExtension = true, 
        CheckPathExists = true, 
        Filter = "Backup (*.bak) | *.bak; | All Files (*.*) | *.*", 
        InitialDirectory = this._InitialPath, 
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
        this._BakInfo = SqlServerManager.Instance.GetDatabasesNameFromBackup(ProfileManager.GetConnectionString(), dialog.FileName);
        this.dbPathTo.Text = System.IO.Path.GetDirectoryName(this._BakInfo._PhysicalNameMdf);
        this.dbName.Text = this._BakInfo._DbOriginalName;
        this.fileName.Text = this._BakInfo.GetDatabaseName() + ".mdf";
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Cannot get information from '{dialog.FileName}' backup");
        WindowHelper.ShowMessage("Cannot get information from backup!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
      }
    }

    private void SelectPathToClick(object sender, RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose folder for restoring", this.dbPathTo, this.selectPathTo, this._InitialPath);
    }

    #endregion
  }
}