#region Usings

using System;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using SIM.Base;
using SIM.Instances;
using SIM.Products;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Runtime;
using SIM.Tool.Windows.Dialogs;
using SIM.Tool.Windows.MainWindowComponents;
using TaskDialogInterop;

#endregion

namespace SIM.Tool.Windows
{
  #region



  #endregion

  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    #region Fields

    /// <summary>
    ///   The instance.
    /// </summary>
    [NotNull]
    public static MainWindow Instance;

    private readonly System.Threading.Timer timer;
    private IMainWindowButton doubleClickHandler;

    #endregion

    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public MainWindow()
    {
      this.InitializeComponent();

      using (new ProfileSection("Main window ctor", typeof(MainWindow)))
      {
        Instance = this;
        if (WindowsSettings.AppUiMainWindowWidth.Value <= 0) 
        {
          this.MaxWidth = this.MinWidth;
        }
        
        this.Title = string.Format(this.Title, SIM.Base.ApplicationManager.AppShortVersion, SIM.Base.ApplicationManager.AppLabel);

        this.timer =
          new System.Threading.Timer(
            obj => this.Dispatcher.Invoke(new Action(() => Search(null, null)), DispatcherPriority.Render));
      }
    }

    #endregion

    #region Methods

    private void HandleError(Exception exception)
    {
      WindowHelper.HandleError(exception.Message, true, exception);
    }

    private bool CheckSqlServer()
    {
      // disabled since not fixed yet
      //return true;
      return EnvironmentHelper.CheckSqlServer();
    }

    /// <summary>
    /// Instances the selected.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data. 
    /// </param>
    private void InstanceSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      try
      {
        if (CheckSqlServer())
        {
          MainWindowHelper.OnInstanceSelected();
        }
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    /// <summary>
    /// Itemses the tree view key pressed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data. 
    /// </param>
    private void ItemsTreeViewKeyPressed([CanBeNull] object sender, [NotNull] System.Windows.Input.KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, "e");

        if (e.Handled)
          return;
        e.Handled = true;
        Key key = e.Key;
        switch (key)
        {
          case Key.Delete:
            {
              if (CheckSqlServer())
              {
                new DeleteInstanceButton().OnClick(this, MainWindowHelper.SelectedInstance);
              }
              return;
            }
          case Key.F2:
            {
              if (CheckSqlServer())
              {
                //MainWindowHelper.Rename();
              }
              return;
            }

          case Key.Escape:
            {
              if (string.IsNullOrEmpty(this.SearchTextBox.Text))
              {
                //this.WindowState = WindowState.Minimized;
              }

              this.SearchTextBox.Text = string.Empty;
              if (CheckSqlServer())
              {
                MainWindowHelper.Search();
              }
              return;
            }

          case Key.F3:
            {
              this.InstanceList.ContextMenu.IsOpen = true;
              return;
            }

          case Key.F5:
            {
              RefreshInstances();
              return;
            }

          case Key.C:
            {
              if ((Keyboard.IsKeyToggled(Key.LeftCtrl) | Keyboard.IsKeyToggled(Key.RightCtrl)) && MainWindowHelper.SelectedInstance != null)
              {
                System.Windows.Clipboard.SetText(MainWindowHelper.SelectedInstance.Name);
                return;
              }

              break;
            }
        }

        e.Handled = false;
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    /// <summary>
    /// Itemses the tree view mouse double click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data. 
    /// </param>
    private void ItemsTreeViewMouseDoubleClick([CanBeNull] object sender, [CanBeNull] MouseButtonEventArgs e)
    {
      using (new ProfileSection("Main window tree item double click", this))
      {
        try
        {
          if (CheckSqlServer())
          {
            if (DoubleClickHandler.IsEnabled(this, MainWindowHelper.SelectedInstance))
            {
              DoubleClickHandler.OnClick(this, MainWindowHelper.SelectedInstance);
            }
          }
        }
        catch (Exception ex)
        {
          HandleError(ex);
        }
      }
    }

    protected IMainWindowButton DoubleClickHandler
    {
      get
      {
        return this.doubleClickHandler ?? (this.doubleClickHandler = (IMainWindowButton)WindowsSettings.AppUiMainWindowDoubleClick.Value.With(x => Plugin.CreateInstance(x)));
      }
    }

    /// <summary>
    /// Itemses the tree view mouse right click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data. 
    /// </param>
    private void ItemsTreeViewMouseRightClick([CanBeNull] object sender, [NotNull] MouseButtonEventArgs e)
    {
      using (new ProfileSection("Main window tree view right click", this))
      {
        try
        {
          Assert.ArgumentNotNull(e, "e");

          WindowHelper.FocusClickedNode(e);
        }
        catch (Exception ex)
        {
          HandleError(ex);
        }
      }
    }

    /// <summary>
    /// Searches the specified sender.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data. 
    /// </param>
    private void Search([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      try
      {
        if (CheckSqlServer())
        {
          MainWindowHelper.Search();
        }
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    /// <summary>
    /// Searches the text box key pressed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data. 
    /// </param>
    private void SearchTextBoxKeyPressed([CanBeNull] object sender, [NotNull] System.Windows.Input.KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, "e");

        if (e.Handled)
          return;
        e.Handled = true;
        Key key = e.Key;
        switch (key)
        {
          case Key.Escape:
            {
              if (string.IsNullOrEmpty(this.SearchTextBox.Text))
              {
                //this.WindowState = WindowState.Minimized;
              }
              this.SearchTextBox.Text = string.Empty;
              if (CheckSqlServer())
                MainWindowHelper.Search();
              return;
            }

          case Key.Enter:
            {
              if (CheckSqlServer())
                MainWindowHelper.Search();
              return;
            }

          case Key.F5:
            {
              RefreshInstances();
              return;
            }

          default:
            {
              if (WindowsSettings.AppInstanceSearchEnabled.Value)
              {
                this.timer.Change(TimeSpan.FromMilliseconds(WindowsSettings.AppInstanceSearchTimeout.Value), TimeSpan.FromMilliseconds(-1));
              }
              e.Handled = false;
              return;
            }
        }
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    /// <summary>
    /// The window loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The e. 
    /// </param>
    private void WindowLoaded(object sender, EventArgs eventArgs)
    {
      try
      {
        if (CheckSqlServer())
        {
          MainWindowHelper.Initialize();
        }

        new Action(this.AnalyticsTracking).BeginInvoke(null, null);
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    private void AnalyticsTracking()
    {
      if (DoNotTrack())
      {
        return;
      }
      
      var id = GetId();
      var ver = ApplicationManager.AppVersion.EmptyToNull() ?? "dev";

      this.Dispatcher.Invoke(new Action(() =>
      {
        try
        {
          var wb = new System.Windows.Forms.WebBrowser
          {
            ScriptErrorsSuppressed = true
          };

          var url = string.Format("https://bitbucket.org/alienlab/sitecore-instance-manager/wiki/Tracking?version={0}&id={1}", ver, id);

          wb.Navigate(url, null, null, "User-Agent: Sitecore Instance Manager");          
        }
        catch (Exception ex)
        {
          Log.Error("Failed to update statistics internal identifier", this, ex);
        }
      }));
    }

    private bool DoNotTrack()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "donottrack.txt");

      return FileSystem.Local.File.Exists(path);
    }

    private string GetId()
    {
      try
      {
        if (EnvironmentHelper.IsSitecoreMachine)
        {
          return "internal-" + Environment.MachineName + "/" + Environment.UserName;
        }
      }
      catch (Exception ex)
      {
        Log.Warn("Failed to compute internal identifier", this, ex);
      }
      
      string cookie = GetCookie();

      return string.Format("public-{0}", cookie);
    }

    private static string GetCookie()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "cookie.txt");
      if (!FileSystem.Local.File.Exists(path))
      {
        var cookie = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");
        FileSystem.Local.File.WriteAllText(path, cookie);

        return cookie;
      }

      return FileSystem.Local.File.ReadAllText(path);
    }

    private void OpenProgramLogs(object sender, RoutedEventArgs e)
    {
      try
      {
        MainWindowHelper.OpenProgramLogs();
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }


    private void AppPoolRecycleClick(object sender, RoutedEventArgs e)
    {
      try
      {
        if (CheckSqlServer())
        {
          MainWindowHelper.AppPoolRecycle();
        }
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    private void AppPoolStopClick(object sender, RoutedEventArgs e)
    {
      try
      {
        if (CheckSqlServer())
        {
          MainWindowHelper.AppPoolStop();
        }
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    private void AppPoolStartClick(object sender, RoutedEventArgs e)
    {
      try
      {
        if (CheckSqlServer())
        {
          MainWindowHelper.AppPoolStart();
        }
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    private void ChangeAppPoolMode(object sender, RoutedEventArgs e)
    {
      try
      {
        if (CheckSqlServer())
        {
          MainWindowHelper.ChangeAppPoolMode((System.Windows.Controls.MenuItem)sender);
        }
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

    private void RefreshInstances()
    {
      try
      {
        MainWindowHelper.RefreshInstances();
      }
      catch (Exception ex)
      {
        HandleError(ex);
      }
    }

	  #endregion

    public void Initialize()
    {
      using (new ProfileSection("Initializing main window", this))
      {
        var appDocument = XmlDocumentEx.LoadFile("App.xml");
        MainWindowHelper.InitializeRibbon(appDocument);
        MainWindowHelper.InitializeContextMenu(appDocument);
        new Action(ManifestHelper.CheckUpdateNeeded).BeginInvoke(null, null);
      }
    }

    private void WindowClosing(object sender, CancelEventArgs e)
    {
      using (new ProfileSection("Closing main window", this))
      {
        try
        {
          ApplicationManager.RaiseAttemptToClose(e);
        }
        catch (Exception ex)
        {
          Log.Error("Err", this, ex);
        }
        if (!e.Cancel)
        {
          MainWindowHelper.UpdateManifests();
        }
      }
    }
  }
}