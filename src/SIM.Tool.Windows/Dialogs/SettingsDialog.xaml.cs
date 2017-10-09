namespace SIM.Tool.Windows.Dialogs
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  public partial class SettingsDialog
  {
    #region Constructors

    public SettingsDialog()
    {
      InitializeComponent();
      Profile = (Profile)ProfileManager.Profile.Clone();
      if (!ProfileManager.FileExists)
      {
        Header.Text = "Initial Configuration";
        HeaderDetails.Text = "We are very sorry, but you have to perform this boring initial configuration in order to use this tool out of the box";
        CancelButton.Visibility = Visibility.Collapsed;
        Profile.InstancesFolder = "C:\\inetpub\\wwwroot";
      }
    }

    #endregion

    #region Properties

    [NotNull]
    private Profile Profile
    {
      get
      {
        return (Profile)DataContext;
      }

      set
      {
        Assert.ArgumentNotNull(value, nameof(value));

        DataContext = value;
      }
    }

    #endregion

    #region Methods

    private void CancelChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      Close();
    }

    private void PickConnectionString([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      WindowHelper.ShowDialog<ConnectionStringDialog>(ConnectionString.Text, this);
      BindingExpression binding = ConnectionString.GetBindingExpression(TextBox.TextProperty);
      Assert.IsNotNull(binding, nameof(binding));
      binding.UpdateSource();
    }

    private void PickInstallsFolder([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      WindowHelper.PickFolder("Choose folder with Sitecore zip installation packages", localRepository, DoneButton);
    }

    private void PickInstancesFolder([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      WindowHelper.PickFolder("Choose folder where Sitecore instances must be installed", MainRootFolder, DoneButton);
    }

    private void PickLicenseFile([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      WindowHelper.PickFile("Choose license file", LicenseFile, DoneButton, "XML Files|*.xml");
    }

    private void SaveChanges([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      SaveSettings();
    }

    private void SaveSettings()
    {
      ProfileManager.SaveChanges(Profile);
      DialogResult = true;
      Close();
    }

    private void ShowAdvanced([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.ShowDialog<AdvancedSettingsDialog>(Profile, this);
    }

    private void TextboxKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        var t = MainRootFolder.Text;
        if (!string.IsNullOrEmpty(t) && FileSystem.FileSystem.Local.Directory.Exists(t))
        {
          BindingExpression binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
          Assert.IsNotNull(binding, nameof(binding));
          binding.UpdateSource();
          SaveSettings();

          return;
        }
      }

      WindowKeyUp(sender, e);
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

    private void ShowAbout(object sender, RoutedEventArgs e)
    {
      WindowHelper.ShowDialog(new AboutDialog(), this);
    }

    #endregion
  }
}