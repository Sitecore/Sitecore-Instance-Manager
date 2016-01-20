namespace SIM.Tool.Windows.Dialogs
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public partial class SettingsDialog
  {
    #region Constructors

    public SettingsDialog()
    {
      this.InitializeComponent();
      this.Profile = (Profile)ProfileManager.Profile.Clone();
      if (!ProfileManager.FileExists)
      {
        this.Header.Text = "Initial Configuration";
        this.HeaderDetails.Text = "We are very sorry, but you have to perform this boring initial configuration in order to use this tool out of the box";
        this.CancelButton.Visibility = Visibility.Collapsed;
        this.Profile.InstancesFolder = "C:\\inetpub\\wwwroot";
      }
    }

    #endregion

    #region Properties

    [NotNull]
    private Profile Profile
    {
      get
      {
        return (Profile)this.DataContext;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.DataContext = value;
      }
    }

    #endregion

    #region Methods

    private void CancelChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.Close();
    }

    private void PickConnectionString([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      WindowHelper.ShowDialog<ConnectionStringDialog>(this.ConnectionString.Text, this);
      BindingExpression binding = this.ConnectionString.GetBindingExpression(TextBox.TextProperty);
      Assert.IsNotNull(binding, "binding");
      binding.UpdateSource();
    }

    private void PickInstallsFolder([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      WindowHelper.PickFolder("Choose folder with Sitecore zip installation packages", this.localRepository, this.DoneButton);
    }

    private void PickInstancesFolder([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      WindowHelper.PickFolder("Choose folder where Sitecore instances must be installed", this.MainRootFolder, this.DoneButton);
    }

    private void PickLicenseFile([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      WindowHelper.PickFile("Choose license file", this.LicenseFile, this.DoneButton, "XML Files|*.xml");
    }

    private void SaveChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      this.SaveSettings();
    }

    private void SaveSettings()
    {
      ProfileManager.SaveChanges(this.Profile);
      this.DialogResult = true;
      this.Close();
    }

    private void ShowAdvanced([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.ShowDialog<AdvancedSettingsDialog>(this.Profile, this);
    }

    private void TextboxKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        var t = this.MainRootFolder.Text;
        if (!string.IsNullOrEmpty(t) && FileSystem.FileSystem.Local.Directory.Exists(t))
        {
          BindingExpression binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
          Assert.IsNotNull(binding, "binding");
          binding.UpdateSource();
          this.SaveSettings();

          return;
        }
      }

      this.WindowKeyUp(sender, e);
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

    private void ShowAbout(object sender, RoutedEventArgs e)
    {
      WindowHelper.ShowDialog(new AboutDialog(), this);
    }

    #endregion
  }
}