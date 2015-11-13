﻿namespace SIM.Tool.Windows
{
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Threading;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Windows.MainWindowComponents;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public partial class MainWindow
  {
    #region Fields

    [NotNull]
    public static MainWindow Instance;

    private readonly Timer timer;
    private IMainWindowButton doubleClickHandler;

    #endregion

    #region Constructors

    public MainWindow()
    {
      this.InitializeComponent();

      using (new ProfileSection("Main window ctor"))
      {
        Instance = this;
        if (WindowsSettings.AppUiMainWindowWidth.Value <= 0)
        {
          this.MaxWidth = this.MinWidth;
        }

        this.Title = string.Format(this.Title, ApplicationManager.AppShortVersion, ApplicationManager.AppLabel)
            + (IsRunningAsAdministrator() ? " (Administrator)" : string.Empty);

        this.timer =
          new System.Threading.Timer(
            obj => this.Dispatcher.Invoke(new Action(() => this.Search(null, null)), DispatcherPriority.Render));
      }
    }

    #endregion

    #region Methods

    #region Protected properties

    protected IMainWindowButton DoubleClickHandler
    {
      get
      {
        return this.doubleClickHandler ?? (this.doubleClickHandler = (IMainWindowButton)WindowsSettings.AppUiMainWindowDoubleClick.Value.With(x => Plugin.CreateInstance(x)));
      }
    }

    #endregion

    #region Private methods

    private static string GetCookie()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "cookie.txt");
      if (!FileSystem.FileSystem.Local.File.Exists(path))
      {
        var cookie = Guid.NewGuid().ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("-", string.Empty);
        FileSystem.FileSystem.Local.File.WriteAllText(path, cookie);

        return cookie;
      }

      return FileSystem.FileSystem.Local.File.ReadAllText(path);
    }

    private void AnalyticsTracking()
    {
      if (this.DoNotTrack())
      {
        return;
      }

      var id = this.GetId();
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
          Log.Error(ex, "Failed to update statistics internal identifier");
        }
      }));
    }

    private void AppPoolRecycleClick(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.CheckSqlServer())
        {
          MainWindowHelper.AppPoolRecycle();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void AppPoolStartClick(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.CheckSqlServer())
        {
          MainWindowHelper.AppPoolStart();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void AppPoolStopClick(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.CheckSqlServer())
        {
          MainWindowHelper.AppPoolStop();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void ChangeAppPoolMode(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.CheckSqlServer())
        {
          MainWindowHelper.ChangeAppPoolMode((System.Windows.Controls.MenuItem)sender);
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private bool CheckSqlServer()
    {
      // disabled since not fixed yet
      // return true;
      return EnvironmentHelper.CheckSqlServer();
    }

    private bool DoNotTrack()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "donottrack.txt");

      return FileSystem.FileSystem.Local.File.Exists(path);
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
        Log.Warn(ex, "Failed to compute internal identifier");
      }

      string cookie = GetCookie();

      return string.Format("public-{0}", cookie);
    }

    private void HandleError(Exception exception)
    {
      WindowHelper.HandleError(exception.Message, true, exception);
    }

    private void InstanceSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      try
      {
        if (this.CheckSqlServer())
        {
          MainWindowHelper.OnInstanceSelected();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private static bool IsRunningAsAdministrator()
    {
      var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
      var principal = new System.Security.Principal.WindowsPrincipal(identity);
      return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }

    private void ItemsTreeViewKeyPressed([CanBeNull] object sender, [NotNull] KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, "e");

        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        Key key = e.Key;
        switch (key)
        {
          case Key.Delete:
          {
            if (this.CheckSqlServer())
            {
              new DeleteInstanceButton().OnClick(this, MainWindowHelper.SelectedInstance);
            }

            return;
          }

          case Key.F2:
          {
            if (this.CheckSqlServer())
            {
              // MainWindowHelper.Rename();
            }

            return;
          }

          case Key.Escape:
          {
            if (string.IsNullOrEmpty(this.SearchTextBox.Text))
            {
              // this.WindowState = WindowState.Minimized;
            }

            this.SearchTextBox.Text = string.Empty;
            if (this.CheckSqlServer())
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
            this.RefreshInstances();
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
        this.HandleError(ex);
      }
    }

    private void ItemsTreeViewMouseDoubleClick([CanBeNull] object sender, [CanBeNull] MouseButtonEventArgs e)
    {
      using (new ProfileSection("Main window tree item double click", this))
      {
        try
        {
          if (this.CheckSqlServer())
          {
            if (this.DoubleClickHandler.IsEnabled(this, MainWindowHelper.SelectedInstance))
            {
              this.DoubleClickHandler.OnClick(this, MainWindowHelper.SelectedInstance);
            }
          }
        }
        catch (Exception ex)
        {
          this.HandleError(ex);
        }
      }
    }

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
          this.HandleError(ex);
        }
      }
    }

    private void OpenProgramLogs(object sender, RoutedEventArgs e)
    {
      try
      {
        MainWindowHelper.OpenProgramLogs();
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
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
        this.HandleError(ex);
      }
    }

    private void Search([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      try
      {
        if (this.CheckSqlServer())
        {
          MainWindowHelper.Search();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void SearchTextBoxKeyPressed([CanBeNull] object sender, [NotNull] KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, "e");

        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        Key key = e.Key;
        switch (key)
        {
          case Key.Escape:
          {
            if (string.IsNullOrEmpty(this.SearchTextBox.Text))
            {
              // this.WindowState = WindowState.Minimized;
            }

            this.SearchTextBox.Text = string.Empty;
            if (this.CheckSqlServer())
            {
              MainWindowHelper.Search();
            }

            return;
          }

          case Key.Enter:
          {
            if (this.CheckSqlServer())
            {
              MainWindowHelper.Search();
            }

            return;
          }

          case Key.F5:
          {
            this.RefreshInstances();
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
        this.HandleError(ex);
      }
    }

    private void WindowLoaded(object sender, EventArgs eventArgs)
    {
      try
      {
        if (this.CheckSqlServer())
        {
          MainWindowHelper.Initialize();
        }

        new Action(this.AnalyticsTracking).BeginInvoke(null, null);
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    #endregion

    #endregion

    #region Public methods

    public void Initialize()
    {
      using (new ProfileSection("Initializing main window", this))
      {
        var appDocument = XmlDocumentEx.LoadFile("App.xml");
        MainWindowHelper.InitializeRibbon(appDocument);
        MainWindowHelper.InitializeContextMenu(appDocument);
      }
    }

    #endregion

    #region Private methods

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
          Log.Error(ex, "Err");
        }
      }
    }

    #endregion
  }
}