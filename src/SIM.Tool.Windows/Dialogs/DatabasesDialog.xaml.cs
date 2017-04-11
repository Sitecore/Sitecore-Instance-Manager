namespace SIM.Tool.Windows.Dialogs
{
  using System;
  using System.Collections;
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Microsoft.Win32;
  using SIM.Adapters.SqlServer;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Windows.Dialogs;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;

  public partial class DatabasesDialog
  {
    #region Fields

    public static string _LastPathToAttach = string.Empty;
    public static string _LastPathToRestore = string.Empty;

    #endregion

    #region Constructors

    public DatabasesDialog()
    {
      InitializeComponent();
    }

    #endregion

    #region Methods

    private void AttachButtonClick(object sender, RoutedEventArgs e)
    {
      var connectionString = ProfileManager.GetConnectionString();
      var dialog = new OpenFileDialog
      {
        AddExtension = true, 
        CheckFileExists = true, 
        CheckPathExists = true, 
        Filter = "*.mdf|*.mdf", 
        Multiselect = false, 
        Title = "Select the database"
      };
      bool? showDialog = dialog.ShowDialog();
      if (showDialog.Value != true)
      {
        return;
      }

      var path = dialog.FileName;
      _LastPathToAttach = path;

      var dbName = SqlServerManager.Instance.GetDatabaseNameFromFile(ProfileManager.GetConnectionString(), path);

      if (dbName == string.Empty)
      {
        dbName = Path.GetFileNameWithoutExtension(path);
      }

      var name = WindowHelper.ShowDialog<InputDialog>(new InputDialogArgs
      {
        Title = "Select the database name", 
        DefaultValue = dbName
      }, this) as string;
      if (!string.IsNullOrEmpty(name))
      {
        if (SqlServerManager.Instance.DatabaseExists(name, connectionString))
        {
          WindowHelper.HandleError("The '{0}' database with the same name is already attached".FormatWith(name), false);
          return;
        }

        SqlServerManager.Instance.AttachDatabase(name, path, connectionString, false);
        Refresh();
      }
    }

    private void CancelChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      Close();
    }

    private void DatabasesMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    private void Delete(object sender, RoutedEventArgs routedEventArgs)
    {
      var connectionString = ProfileManager.GetConnectionString();
      var names = Databases.SelectedItems.Cast<string>().ToArray();

      var result = names.Take(6).Aggregate((curr, name) => curr + "\n  • " + name) + (names.Length > 6 ? "\n    ..." : string.Empty);

      if (MessageBox.Show("Are you sure that want to delete the following databases?\n {0}".FormatWith(result), "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
      {
        return;
      }

      foreach (string name in names)
      {
        if (!string.IsNullOrEmpty(name))
        {
          if (!SqlServerManager.Instance.DatabaseExists(name, connectionString))
          {
            return;
          }

          try
          {
            SqlServerManager.Instance.DeleteDatabase(name, connectionString); // debug

            // WindowHelper.ShowMessage("Database {0} has been deleted".FormatWith(name), "Info");
          }
          catch (Exception ex)
          {
            WindowHelper.HandleError("Deleting the '{0}' database  caused an exception".FormatWith(name), false, ex, 
              GetType());
          }
        }
      }

      Refresh();
    }

    private void Detach(object sender, RoutedEventArgs e)
    {
      var names = Databases.SelectedItems;
      Func<IList, string> getNamesAsString = (IList x) =>
      {
        var result = string.Empty;
        var i = 0;
        foreach (string name in x)
        {
          i++;
          if (i > 10)
          {
            result += "\n...";
            break;
          }

          result += "\n• " + name;
        }

        return result;
      };

      if (
        WindowHelper.ShowMessage("Are you sure that want to detach the following databases? {0}".FormatWith(getNamesAsString(names)), MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
      {
        return;
      }

      WindowHelper.LongRunningTask(() => Detach(names), "Deataching databases", GetWindow(this));
      Refresh();
    }

    private void Detach(IList names)
    {
      var connectionString = ProfileManager.GetConnectionString();
      foreach (string name in names)
      {
        if (!string.IsNullOrEmpty(name))
        {
          bool databaseExists = SqlServerManager.Instance.DatabaseExists(name, connectionString);
          if (databaseExists)
          {
            try
            {
              SqlServerManager.Instance.DetachDatabase(name, connectionString); // debug

              // MessageBox.Show("Database {0} has been detached".FormatWith(name), "Info");
            }
            catch (Exception ex)
            {
              Dispatcher.Invoke(new Action(() => WindowHelper.HandleError(
                "The '{0}' database doesn't exist or isn't attached to the '{1}' SQL Server instance.".FormatWith(name, 
                  connectionString
                    .DataSource), 
                false, ex, this)));
              
            }
          }
          else
          {
            Dispatcher.Invoke(new Action(() => WindowHelper.HandleError(
              "The '{0}' database doesn't exist or isn't attached to the '{1}' SQL Server instance".FormatWith(name, 
                connectionString
                  .DataSource), 
              false)));
          }
        }
      }

      Refresh();
    }

    private void Detach(string dbName)
    {
      var connectionString = ProfileManager.GetConnectionString();

      if (!string.IsNullOrEmpty(dbName))
      {
        bool databaseExists = SqlServerManager.Instance.DatabaseExists(dbName, connectionString);
        if (databaseExists)
        {
          try
          {
            SqlServerManager.Instance.DetachDatabase(dbName, connectionString); // debug

            // MessageBox.Show("Database {0} has been detached".FormatWith(name), "Info");
          }
          catch (Exception ex)
          {
            Dispatcher.Invoke(new Action(() => WindowHelper.HandleError(
              "The '{0}' database doesn't exist or isn't attached to the '{1}' SQL Server instance.".FormatWith(dbName, 
                connectionString
                  .DataSource), 
              false, ex, this)));
            
          }
        }
        else
        {
          Dispatcher.Invoke(new Action(() => WindowHelper.HandleError(
            "The '{0}' database doesn't exist or isn't attached to the '{1}' SQL Server instance".FormatWith(dbName, 
              connectionString
                .DataSource), 
            false)));
        }
      }
    }

    private void Refresh(object sender = null, EventArgs eventArgs = null)
    {
      var connectionString = ProfileManager.GetConnectionString();

      var textBox = SearchBox;
      var arr = textBox.Text.IsNullOrEmpty() ? SqlServerManager.Instance.GetDatabasesNames(connectionString) : SqlServerManager.Instance.GetDatabasesNames(connectionString, textBox.Text.Replace(" ", string.Empty).Replace(@"'", string.Empty));

      Databases.DataContext = arr;
    }

    private void Rename(object sender, RoutedEventArgs e)
    {
      var connectionString = ProfileManager.GetConnectionString();
      var name = Databases.SelectedItem as string;
      if (!string.IsNullOrEmpty(name))
      {
        if (!SqlServerManager.Instance.DatabaseExists(name, connectionString))
        {
          WindowHelper.HandleError("The '{0}' database doesn't exist or isn't attached to the '{1}' SQL Server instance".FormatWith(name, connectionString.DataSource), false);
          return;
        }

        var newname = WindowHelper.ShowDialog<InputDialog>(new InputDialogArgs
        {
          Title = "Select the new name", 
          DefaultValue = name
        }, this) as string;
        if (newname == null)
        {
          return;
        }

        try
        {
          var file = SqlServerManager.Instance.GetDatabaseFileName(name, connectionString);
          SqlServerManager.Instance.DetachDatabase(name, connectionString);
          SqlServerManager.Instance.AttachDatabase(newname, file, connectionString);
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Renaming the '{0}' database caused an exception".FormatWith(name), false, ex, this);
        }

        Refresh();
      }
    }

    private void SaveChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      SaveSettings();
    }

    private void SaveSettings()
    {
      DialogResult = true;
      Close();
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      if (e.Handled)
      {
        return;
      }

      e.Handled = true;

      if (e.Key == Key.Escape)
      {
        Close();
        return;
      }

      if (e.Key == Key.F2 && sender is ListBox)
      {
        Rename(sender, e);
        return;
      }

      if (e.Key == Key.Delete && sender is ListBox)
      {
        Delete(sender, e);
        return;
      }

      e.Handled = false;
    }

    private void WindowLoaded(object sender, EventArgs eventArgs)
    {
      Refresh();
    }

    private void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
    {
      Refresh();
    }

    #endregion

    #region Private methods

    private void Backup(object sender, RoutedEventArgs e)
    {
      var connectionString = ProfileManager.GetConnectionString();
      var databaseName = Databases.SelectedItems[0].ToString();
      if (Databases.SelectedItems.Count == 1)
      {
        var dialog = new SaveFileDialog
        {
          AddExtension = true, 
          CheckPathExists = true, 
          Filter = "*.bak | *.bak", 
          InitialDirectory = Path.GetDirectoryName(SqlServerManager.Instance.GetDatabaseFileName(databaseName, ProfileManager.GetConnectionString())), 
          Title = "Specify folder to backup"
        };
        bool? showDialog = dialog.ShowDialog();
        if (showDialog.Value != true)
        {
          return;
        }

        var path = dialog.FileName;

        var backupPath = Path.Combine(System.IO.Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".bak");
        WindowHelper.LongRunningTask(() => Backup(databaseName, connectionString, backupPath), "Back up database", this, "The backup is being created", null, true);
      }
    }

    private void Backup(string databaseName, SqlConnectionStringBuilder connectionString, string backupPath)
    {
      SqlServerManager.Instance.BackupDatabase(connectionString, databaseName, backupPath);
    }

    private void DatabasesMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (Databases.SelectedItems.Count == 1)
      {
        WindowHelper.ShowDialog(new DatabaseQueryExecutionDialog("USE [" + Databases.SelectedItem.ToString() + "]"), this);
      }
    }

    private void MoveDatabase(object sender, RoutedEventArgs e)
    {
      // var fileDialog = new OpenFileDialog
      // {
      // Title = "Select zip file of exported solution",
      // Multiselect = false,
      // DefaultExt = ".zip"
      // };
      if (Databases.SelectedItems.Count == 1)
      {
        var originalFolder = Path.GetDirectoryName(SqlServerManager.Instance.GetDatabaseFileName(Databases.SelectedItem.ToString(), ProfileManager.GetConnectionString()));
        var fileDialog = new System.Windows.Forms.FolderBrowserDialog()
        {
          // Title = "Select folder for moving database"
          SelectedPath = originalFolder
        };

        fileDialog.ShowDialog();
        var folderForRelocation = fileDialog.SelectedPath;

        if (folderForRelocation != originalFolder && !folderForRelocation.IsNullOrEmpty())
        {
          var dbFileName = Path.GetFileName(SqlServerManager.Instance.GetDatabaseFileName(Databases.SelectedItem.ToString(), ProfileManager.GetConnectionString()));
          var dbName = Databases.SelectedItem.ToString();
          var connString = ProfileManager.GetConnectionString();
          WindowHelper.LongRunningTask(() => MoveDatabase(folderForRelocation, originalFolder, dbName, dbFileName, connString), "Move database", this, "The database is being moved", null, true);
        }
        else
        {
          WindowHelper.ShowMessage("Choose another folder.");
        }
      }
      else if (Databases.SelectedItems.Count > 1)
      {
        WindowHelper.ShowMessage("Please select only one database.");
      }
      else
      {
        WindowHelper.ShowMessage("Please select database.");
      }

      Refresh();
    }

    private void MoveDatabase(string folderForRelocation, string originalFolder, string databaseName, string databaseFileName, SqlConnectionStringBuilder connString)
    {
      Detach(databaseName);
      FileSystem.FileSystem.Local.File.Move(originalFolder.PathCombine(databaseFileName), folderForRelocation.PathCombine(databaseFileName));
      SqlServerManager.Instance.AttachDatabase(databaseName, folderForRelocation.PathCombine(databaseFileName), connString, false);
    }

    private void Restore(object sender, RoutedEventArgs e)
    {
      if (FileSystem.FileSystem.Local.Directory.Exists(Clipboard.GetText()))
      {
        WindowHelper.ShowDialog(new DatabasesOperationsDialog(Clipboard.GetText()), this);
      }
      else if (FileSystem.FileSystem.Local.Directory.Exists(_LastPathToRestore))
      {
        WindowHelper.ShowDialog(new DatabasesOperationsDialog(_LastPathToRestore), this);
      }
      else if (Databases.SelectedItem != null)
      {
        WindowHelper.ShowDialog(new DatabasesOperationsDialog(Path.GetDirectoryName(SqlServerManager.Instance.GetDatabaseFileName(
          Databases.SelectedItems[0].ToString(), ProfileManager.GetConnectionString()))
          ), this);
      }
      else
      {
        WindowHelper.ShowDialog(new DatabasesOperationsDialog(), this);
      }
    }

    private void ShowQueryExecutionDialog(object sender, RoutedEventArgs e)
    {
      WindowHelper.ShowDialog(new DatabaseQueryExecutionDialog("USE [" + Databases.SelectedItem.ToString() + "]"), this);
    }

    private void ShowSqlShell(object sender, RoutedEventArgs e)
    {
      if (Databases.SelectedItems.Count == 1)
      {
        WindowHelper.ShowDialog(new DatabaseQueryExecutionDialog("USE [" + Databases.SelectedItem.ToString() + "]"), this);
      }
      else
      {
        WindowHelper.ShowDialog(new DatabaseQueryExecutionDialog(string.Empty), this);

        // ShowQueryExecutionDialog
      }
    }

    #endregion
  }
}