#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SIM.Adapters.WebServer;
using SIM.Base;
using SIM.Tool.Base;

#endregion

namespace SIM.Tool.Windows.Dialogs
{               
  /// <summary>
  ///   Interaction logic for SettingsDialog.xaml
  /// </summary>
  public partial class HostsDialog
  {
    readonly string hostsFilePath = Environment.ExpandEnvironmentVariables(@"%WINDIR%\System32\drivers\etc\hosts");

    private List<Hosts.HostRecord> records;

    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SettingsDialog" /> class.
    /// </summary>
    public HostsDialog()
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
      this.SaveSettings();
    }

    /// <summary>
    ///   Saves the settings.
    /// </summary>
    private void SaveSettings()
    {
      Hosts.Save(records);
      this.DialogResult = true;
      this.Close();
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
        if (e.Handled)
          return;
        e.Handled = true;
        this.Close();
      }
    }

    #endregion

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      try
      {
        records = Hosts.GetRecords().ToList();
        HostsList.DataContext = records;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("The exception occurred during window initialization - it will be closed then.", true, ex, this);
        this.Close();
      }
    }

    private void Delete(object sender, RoutedEventArgs e)
    {
      var button = (Button)sender;
      var id = button.Tag;
      var record = records.Single(r => r.ID.Equals(id));
      records.Remove(record);
      HostsList.DataContext = null;
      HostsList.DataContext = records;
    }

    private void Add(object sender, RoutedEventArgs e)
    {
      var newRecord = new Hosts.HostRecord("127.0.0.1");
      records.Add(newRecord);
      HostsList.DataContext = null;
      HostsList.DataContext = records;
      HostsList.ScrollIntoView(newRecord);
    }
  }

}