#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
  public partial class AdvancedSettingsDialog
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SettingsDialog" /> class.
    /// </summary>
    public AdvancedSettingsDialog()
    {
      // workaround for #179
      var types = new[]
        {
          typeof(SIM.Tool.Base.AppSettings),
          typeof(SIM.Tool.Windows.Properties.Settings),
          typeof(SIM.Tool.Windows.WindowsSettings),
          typeof(SIM.Instances.InstanceManager.Settings),
          typeof(SIM.Pipelines.Install.Settings),
          typeof(SIM.Adapters.SqlServer.SqlServerManager.Settings),
          typeof(SIM.Base.WebRequestHelper.Settings)
        };

      foreach (Type type in types)
      {
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
      }

      this.InitializeComponent();
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
      get { return (Profile) this.DataContext; }

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

    private void ContentLoaded(object sender, EventArgs e)
    {
      this.DataGrid.DataContext = GetAdvancedProperties();
    }

    private IEnumerable<AdvancedPropertyBase> GetAdvancedProperties()
    {
      string pluginPrefix = "App/Plugins/";

      var nonPluginsSettings = new List<AdvancedPropertyBase>();
      var pluginsSettings = new List<AdvancedPropertyBase>();

      foreach (KeyValuePair<string, AdvancedPropertyBase> keyValuePair in AdvancedSettingsManager.RegisteredSettings)
      {
        if (keyValuePair.Key.StartsWith(pluginPrefix))
          pluginsSettings.Add(keyValuePair.Value);
        else
          nonPluginsSettings.Add(keyValuePair.Value);
      }

      List<AdvancedPropertyBase> result = nonPluginsSettings.OrderBy(prop => prop.Name).ToList();
      result.AddRange(pluginsSettings.OrderBy(prop => prop.Name));
      return result;
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
    private void OpenDocumentation([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.OpenInBrowser("https://bitbucket.org/alienlab/sitecore-instance-manager/wiki/Home", true);
    }

    private void OpenFile(object sender, RoutedEventArgs e)
    {
      WindowHelper.OpenFolder(ApplicationManager.DataFolder);
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
  }
}