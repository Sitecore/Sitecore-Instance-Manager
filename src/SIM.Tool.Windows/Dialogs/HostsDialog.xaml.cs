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
  using Sitecore.Diagnostics.Base.Annotations;

  public partial class HostsDialog
  {
    #region Fields

    private List<Hosts.IHostRecord> records;

    #endregion

    #region Constructors

    public HostsDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    private void CancelChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.Close();
    }

    private void SaveChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.SaveSettings();
    }

    private void SaveSettings()
    {
      Hosts.Save(this.records);
      this.DialogResult = true;
      this.Close();
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      if (e.Key == Key.Escape)
      {
        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        this.Close();
      }
    }

    #endregion

    #region Private methods

    private void Add(object sender, RoutedEventArgs e)
    {
      var newRecord = new Hosts.IpHostRecord("127.0.0.1");
      this.records.Add(newRecord);
      this.HostsList.DataContext = null;
      this.HostsList.DataContext = this.records;
      this.HostsList.ScrollIntoView(newRecord);
    }

    private void Delete(object sender, RoutedEventArgs e)
    {
      var button = (Button)sender;
      var id = button.Tag;
      var record = this.records.OfType<Hosts.IpHostRecord>().Single(r => r.ID.Equals(id));
      this.records.Remove(record);
      this.HostsList.DataContext = null;
      this.HostsList.DataContext = this.records;
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      try
      {
        this.records = Hosts.GetRecords().ToList();
        this.HostsList.DataContext = this.records;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("The exception occurred during window initialization - it will be closed then.", true, ex);
        this.Close();
      }
    }

    #endregion
  }
}