#region Usings

using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using SIM.Base;
using SIM.Tool.Base;
using SIM.Tool.Base.Profiles;

#endregion

namespace SIM.Tool.Windows.Dialogs
{
  #region

  

  #endregion

  /// <summary>
  ///   Interaction logic for SettingsDialog.xaml
  /// </summary>
  public partial class SettingsDialog
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SettingsDialog" /> class.
    /// </summary>
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

    /// <summary>
    ///   Gets or sets the profile.
    /// </summary>
    /// <value> The profile. </value>
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
    /// Picks the connection string.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void PickConnectionString([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      WindowHelper.ShowDialog<ConnectionStringDialog>(this.ConnectionString.Text, this);
      BindingExpression binding = this.ConnectionString.GetBindingExpression(TextBox.TextProperty);
      Assert.IsNotNull(binding, "binding");
      binding.UpdateSource();
    }

    /// <summary>
    /// Picks the installs folder.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void PickInstallsFolder([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      WindowHelper.PickFolder("Choose folder with Sitecore zip installation packages", this.localRepository, this.DoneButton);
    }

    /// <summary>
    /// Picks the instances folder.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void PickInstancesFolder([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      WindowHelper.PickFolder("Choose folder where Sitecore instances must be installed", this.MainRootFolder, this.DoneButton);
    }

    /// <summary>
    /// Picks the license file.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void PickLicenseFile([NotNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      WindowHelper.PickFile("Choose license file", this.LicenseFile, this.DoneButton, "XML Files|*.xml");
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
      ProfileManager.SaveChanges(this.Profile);
      this.DialogResult = true;
      this.Close();
    }

    /// <summary>
    /// Shows the advanced.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void ShowAdvanced([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.ShowDialog<AdvancedSettingsDialog>(this.Profile, this);
    }

    /// <summary>
    /// Textbox the key up.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data. 
    /// </param>
    private void TextboxKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        var t = this.MainRootFolder.Text;
        if (!string.IsNullOrEmpty(t) && FileSystem.Local.Directory.Exists(t))
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
        if (e.Handled) return;
        e.Handled = true;
        this.Close();
      }
    }

    #endregion

    private void ShowPlugins(object sender, RoutedEventArgs e)
    {
      WindowHelper.ShowDialog(new PluginsDialog(), this);
    }

    private void ShowAbout(object sender, RoutedEventArgs e)
    {
      WindowHelper.ShowDialog(new AboutDialog(), this);
    }
  }
}