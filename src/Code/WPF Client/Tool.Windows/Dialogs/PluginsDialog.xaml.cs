#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using SIM.Base;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Runtime;

#endregion

namespace SIM.Tool.Windows.Dialogs
{
  #region

  

  #endregion

  public partial class PluginsDialog
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SettingsDialog" /> class.
    /// </summary>
    public PluginsDialog()
    {
      this.InitializeComponent();
      this.Profile = (Profile)ProfileManager.Profile.Clone();
      this.plugins.DataContext = Plugins;
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

    public IEnumerable Plugins
    {
      get
      {
        return PluginManager.Plugins.Select(plugin => new Item(plugin, this.Profile));
      }
    }

    public class Item 
    {
      private readonly Profile profile;
      public event PropertyChangedEventHandler PropertyChanged;

      public Item(Plugin path, Profile profile)
      {
        this.profile = profile;
        this.Plugin = path;
      }

      public Plugin Plugin { get; set; }

      public bool IsChecked 
      {
        get
        {
          return PluginManager.GetEnabledPlugins(profile).Contains(this.Plugin);
        }
        set
        {
          // refactor this later
          if (!IsChecked)
          {
            this.profile.Plugins = (this.profile.Plugins + "|" + this.Plugin).TrimStart('|');
          }
          else
          {
            this.profile.Plugins = this.profile.Plugins.Replace(this.Plugin.PluginFolder, string.Empty).Replace("||", "|").Trim('|');
          }
        }
      }

      public override string ToString()
      {
        var path = this.Plugin.ToString();
        const string appData = "%AppData%";
        var realAppData = Environment.ExpandEnvironmentVariables(appData);
        path = path.Replace(realAppData, appData);
        return path;
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
      var result = WindowHelper.ShowMessage("In order to apply changes it is required to restart the application. Would you like to restart it now?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
      if(result == MessageBoxResult.Yes)
      {
        LifeManager.RestartApplication();
      }
      else
      {
        this.Close();
      }
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

    private void OpenFolder(object sender, RoutedEventArgs e)
    {
      string path = Path.Combine(ApplicationManager.PluginsFolder, "Readme.html");
      //if (!FileSystem.Instance.FileExists(path))
      //{
      string text = @"<h1>Plugins</h1><p>Plugins folder is designed for your <b>custom SIM Plugins</b>. Please find the examples in the stock plugins folder located here: <b>{0}</b></p>".FormatWith(Path.Combine(Environment.CurrentDirectory, "Plugins"));
      FileSystem.Local.File.WriteAllText(path, text);
      WindowHelper.OpenFolder(ApplicationManager.PluginsFolder);
    }

    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      this.plugins.SelectedIndex = -1;
    }
  }
}