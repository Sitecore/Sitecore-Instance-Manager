namespace SIM.Tool.Windows
{
  using System;
  using System.ComponentModel;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Threading;
  using SIM.Core;
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

        this.Title = string.Format(this.Title, ApplicationManager.AppShortVersion, ApplicationManager.AppVersion, ApplicationManager.AppLabel);

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
        var appDocument = XmlDocumentEx.LoadFileSafe("App.xml") ?? XmlDocumentEx.LoadFile(ApplicationManager.GetEmbeddedFile("SIM.Tool.Windows", "App.xml"));
        appDocument.Save("App.xml");
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
