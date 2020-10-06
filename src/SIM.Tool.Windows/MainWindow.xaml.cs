using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using SIM.Instances;

namespace SIM.Tool.Windows
{
  using System;
  using System.ComponentModel;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Threading;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Windows.MainWindowComponents;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public partial class MainWindow
  {
    #region Fields

    [NotNull]
    public static MainWindow Instance { get; private set; }

    private Timer Timer { get; }
    private IMainWindowButton _DoubleClickHandler;

    #endregion

    #region Constructors

    private object _lock = new object();
    public MainWindow()
    {
      InitializeComponent();
      this.InstanceList.ItemsSource = InstanceManager.Default.InstancesObservableCollection;
      BindingOperations.EnableCollectionSynchronization(InstanceManager.Default.InstancesObservableCollection, _lock);
      InstanceList.Items.GroupDescriptions.Add(new PropertyGroupDescription("SitecoreEnvironment.Name"));
      InstanceList.Items.SortDescriptions.Add(new SortDescription("SitecoreEnvironment.Name", ListSortDirection.Ascending));
      InstanceList.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

      using (new ProfileSection("Main window ctor"))
      {
        Instance = this;
        if (WindowsSettings.AppUiMainWindowWidth.Value <= 0)
        {
          MaxWidth = MinWidth;
        }

        Title = string.Format(Title, ApplicationManager.IsQa ? "QA" : (ApplicationManager.IsDev ? "DEV" : ""), ApplicationManager.AppShortVersion, ApplicationManager.AppVersion, ApplicationManager.AppLabel);

        Timer =
          new Timer(
            obj => Dispatcher.Invoke(() => Search(null, null), DispatcherPriority.Render));
      }
    }

    #endregion

    #region Methods

    #region Protected properties

    protected IMainWindowButton DoubleClickHandler
    {
      get
      {
        return _DoubleClickHandler ?? (_DoubleClickHandler = (IMainWindowButton)WindowsSettings.AppUiMainWindowDoubleClick.Value.With(x => Plugin.CreateInstance(x)));
      }
    }

    #endregion

    #region Private methods

    private bool CheckSqlServer()
    {
      // disabled since not fixed yet
      // return true;
      return EnvironmentHelper.CheckSqlServer();
    }

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
        WindowHelper.HandleError("Failed to handle instance selected", true, ex);
      }
    }

    private void ItemsTreeViewKeyPressed([CanBeNull] object sender, [NotNull] KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, nameof(e));

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
              // MainWindowHelper.Rename();
            }

            return;
          }

          case Key.Escape:
          {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
              // this.WindowState = WindowState.Minimized;
            }

            SearchTextBox.Text = string.Empty;
            if (CheckSqlServer())
            {
              MainWindowHelper.Search();
            }

            return;
          }

          case Key.F3:
          {
            InstanceList.ContextMenu.IsOpen = true;
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
        WindowHelper.HandleError("Failed to handle tree view key pressed", true, ex);
      }
    }

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
          WindowHelper.HandleError("Failed to handle tree view double click", true, ex);
        }
      }
    }

    private void ItemsTreeViewMouseRightClick([CanBeNull] object sender, [NotNull] MouseButtonEventArgs e)
    {
      using (new ProfileSection("Main window tree view right click", this))
      {
        try
        {
          Assert.ArgumentNotNull(e, nameof(e));

          WindowHelper.FocusClickedNode(e);
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Failed to handle tree view right click", true, ex);
        }
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
        WindowHelper.HandleError("Failed to refresh instances", true, ex);
      }
    }

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
        WindowHelper.HandleError("Failed to search", true, ex);
      }
    }

    private void SearchTextBoxKeyPressed([CanBeNull] object sender, [NotNull] KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, nameof(e));

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
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
              // this.WindowState = WindowState.Minimized;
            }

            SearchTextBox.Text = string.Empty;
            if (CheckSqlServer())
            {
              MainWindowHelper.Search();
            }

            return;
          }

          case Key.Enter:
          {
            if (CheckSqlServer())
            {
              MainWindowHelper.Search();
            }

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
              Timer.Change(TimeSpan.FromMilliseconds(WindowsSettings.AppInstanceSearchTimeout.Value), TimeSpan.FromMilliseconds(-1));
            }

            e.Handled = false;
            return;
          }
        }
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Failed to search", true, ex);
      }
    }

    private void WindowLoaded(object sender, EventArgs eventArgs)
    {
      try
      {
        if (CheckSqlServer())
        {
          MainWindowHelper.Initialize();
        }
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Failed to handle window loaded", true, ex);
      }
    }

    #endregion

    #endregion

    #region Public methods

    public void Initialize()
    {
      using (new ProfileSection("Initializing main window", this))
      {
        MainWindowHelper.InitializeRibbon(MainWindowData.Tabs);
        MainWindowHelper.InitializeContextMenu(MainWindowData.MenuItems);
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

    // Prevent context menu opening on toggle button
    private void ToggleButton_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
      e.Handled = true;
    }

    #endregion
  }
}
