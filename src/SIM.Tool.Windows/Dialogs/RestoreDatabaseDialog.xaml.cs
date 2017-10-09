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
      InitializeComponent();
    }

    public RestoreDatabaseDialog(string initialDirectory)
    {
      InitializeComponent();
      _InitialPath = initialDirectory;
    }

    public RestoreDatabaseDialog(string name, string pathFrom, string pathTo)
    {
      InitializeComponent();
      dbName.Text = name;
      dbPathFrom.Text = pathFrom;
      dbPathTo.Text = pathTo;
    }

    #endregion

    #region Methods

    private void RestoreDatabase(string dbFileName, string databaseName, string pathFrom, string pathTo, SqlConnectionStringBuilder connectionString)
    {
      try
      {
        if (dbFileName.IsNullOrEmpty())
        {
          SqlServerManager.Instance.RestoreDatabase(databaseName, connectionString, pathFrom, pathTo, _BakInfo);
        }
        else
        {
          SqlServerManager.Instance.RestoreDatabase(databaseName, dbFileName, connectionString, pathFrom, pathTo, _BakInfo);
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
      var databaseName = dbName.Text;
      var dbFileName = fileName.Text;
      var pathFrom = dbPathFrom.Text;
      var pathTo = dbPathTo.Text;


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

        DatabasesDialog._LastPathToRestore = pathFrom;
        Close();
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
        InitialDirectory = _InitialPath, 
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
        _BakInfo = SqlServerManager.Instance.GetDatabasesNameFromBackup(ProfileManager.GetConnectionString(), dialog.FileName);
        dbPathTo.Text = System.IO.Path.GetDirectoryName(_BakInfo._PhysicalNameMdf);
        dbName.Text = _BakInfo._DbOriginalName;
        fileName.Text = _BakInfo.GetDatabaseName() + ".mdf";
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Cannot get information from '{dialog.FileName}' backup");
        WindowHelper.ShowMessage("Cannot get information from backup!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
      }
    }

    private void SelectPathToClick(object sender, RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose folder for restoring", dbPathTo, selectPathTo, _InitialPath);
    }

    #endregion
  }
}