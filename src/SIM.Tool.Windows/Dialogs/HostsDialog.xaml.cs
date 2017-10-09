namespace SIM.Tool.Windows.Dialogs
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using SIM.Adapters.WebServer;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public partial class HostsDialog
  {
    #region Fields

    private IList<Hosts.IpHostRecord> _Records;

    #endregion

    #region Constructors

    public HostsDialog()
    {
      InitializeComponent();
    }

    #endregion

    #region Methods

    private void CancelChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      Close();
    }

    private void SaveChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      SaveSettings();
    }

    private void SaveSettings()
    {
      Hosts.Save(_Records);
      DialogResult = true;
      Close();
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      if (e.Key == Key.Escape)
      {
        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        Close();
      }
    }

    #endregion

    #region Private methods

    private void Add(object sender, RoutedEventArgs e)
    {
      var newRecord = new Hosts.IpHostRecord("127.0.0.1");
      _Records.Add(newRecord);
      HostsList.DataContext = null;
      HostsList.DataContext = _Records;
      HostsList.ScrollIntoView(newRecord);
    }

    private void Delete(object sender, RoutedEventArgs e)
    {
      var button = (Button)sender;
      var id = button.Tag;
      var record = _Records.OfType<Hosts.IpHostRecord>().Single(r => r.ID.Equals(id));
      _Records.Remove(record);
      HostsList.DataContext = null;
      HostsList.DataContext = _Records;
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      try
      {
        _Records = Hosts.GetRecords().OfType<Hosts.IpHostRecord>().ToList();
        HostsList.DataContext = _Records;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("The exception occurred during window initialization - it will be closed then.", true, ex);
        Close();
      }
    }

    #endregion
  }
}