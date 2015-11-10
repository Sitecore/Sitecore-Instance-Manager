namespace SIM.Tool.Windows.Dialogs
{
  using System;
  using System.Collections;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Runtime;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public partial class PluginsDialog
  {
    #region Constructors

    public PluginsDialog()
    {
      this.InitializeComponent();
      this.Profile = (Profile)ProfileManager.Profile.Clone();
      this.plugins.DataContext = this.Plugins;
    }

    #endregion

    #region Properties

    #region Public properties

    public IEnumerable Plugins
    {
      get
      {
        return PluginManager.Plugins.Select(plugin => new Item(plugin, this.Profile));
      }
    }

    #endregion

    #region Private properties

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

    public class Item
    {
      #region Delegates

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      #region Fields

      private readonly Profile profile;

      #endregion

      #region Constructors

      public Item(Plugin path, Profile profile)
      {
        this.profile = profile;
        this.Plugin = path;
      }

      #endregion

      #region Public properties

      public bool IsChecked
      {
        get
        {
          return PluginManager.GetEnabledPlugins(this.profile).Contains(this.Plugin);
        }

        set
        {
          // refactor this later
          if (!this.IsChecked)
          {
            this.profile.Plugins = (this.profile.Plugins + "|" + this.Plugin).TrimStart('|');
          }
          else
          {
            this.profile.Plugins = this.profile.Plugins.Replace(this.Plugin.PluginFolder, string.Empty).Replace("||", "|").Trim('|');
          }
        }
      }

      public Plugin Plugin { get; set; }

      #endregion

      #region Public methods

      public override string ToString()
      {
        var path = this.Plugin.ToString();
        const string appData = "%AppData%";
        var realAppData = Environment.ExpandEnvironmentVariables(appData);
        path = path.Replace(realAppData, appData);
        return path;
      }

      #endregion
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
      ProfileManager.SaveChanges(this.Profile);
      var result = WindowHelper.ShowMessage("In order to apply changes it is required to restart the application. Would you like to restart it now?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
      if (result == MessageBoxResult.Yes)
      {
        LifeManager.RestartApplication();
      }
      else
      {
        this.Close();
      }
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

    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      this.plugins.SelectedIndex = -1;
    }

    private void OpenFolder(object sender, RoutedEventArgs e)
    {
      string path = Path.Combine(ApplicationManager.PluginsFolder, "Readme.html");

      // if (!FileSystem.Instance.FileExists(path))
      // {
      string text = @"<h1>Plugins</h1><p>Plugins folder is designed for your <b>custom SIM Plugins</b>. Please find the examples in the stock plugins folder located here: <b>{0}</b></p>".FormatWith(Path.Combine(Environment.CurrentDirectory, "Plugins"));
      FileSystem.FileSystem.Local.File.WriteAllText(path, text);
      WindowHelper.OpenFolder(ApplicationManager.PluginsFolder);
    }

    #endregion
  }
}