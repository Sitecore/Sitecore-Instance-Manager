namespace SIM.Tool.Windows.Dialogs
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  #region

  #endregion

  public partial class AdvancedSettingsDialog
  {
    #region Constructors

    public AdvancedSettingsDialog()
    {
      // workaround for #179
      var types = new[]
      {
        typeof(SIM.Core.CoreAppSettings), 
        typeof(SIM.Tool.Base.WinAppSettings), 
        typeof(SIM.Tool.Windows.Properties.Settings), 
        typeof(SIM.Tool.Windows.WindowsSettings), 
        typeof(SIM.Pipelines.Install.Settings), 
        typeof(SIM.Adapters.SqlServer.SqlServerManager.Settings), 
        typeof(WebRequestHelper.Settings)
      };

      foreach (Type type in types)
      {
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
      }

      this.InitializeComponent();
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

    private void ContentLoaded(object sender, EventArgs e)
    {
      this.DataGrid.DataContext = this.GetAdvancedProperties();
    }

    private IEnumerable<AdvancedPropertyBase> GetAdvancedProperties()
    {
      string pluginPrefix = "App/Plugins/";

      var nonPluginsSettings = new List<AdvancedPropertyBase>();
      var pluginsSettings = new List<AdvancedPropertyBase>();

      foreach (KeyValuePair<string, AdvancedPropertyBase> keyValuePair in AdvancedSettingsManager.RegisteredSettings)
      {
        if (keyValuePair.Key.StartsWith(pluginPrefix))
        {
          pluginsSettings.Add(keyValuePair.Value);
        }
        else
        {
          nonPluginsSettings.Add(keyValuePair.Value);
        }
      }

      List<AdvancedPropertyBase> result = nonPluginsSettings.OrderBy(prop => prop.Name).ToList();
      result.AddRange(pluginsSettings.OrderBy(prop => prop.Name));
      return result;
    }

    private void OpenDocumentation([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      CoreApp.OpenInBrowser("https://github.com/sitecore/sitecore-instance-manager/wiki/Advanced", true);
    }

    private void OpenFile(object sender, RoutedEventArgs e)
    {
      CoreApp.OpenFolder(ApplicationManager.DataFolder);
    }

    private void SaveSettings()
    {
      ProfileManager.SaveChanges(this.Profile);
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
  }
}