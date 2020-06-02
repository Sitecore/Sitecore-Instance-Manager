namespace SIM.Tool.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.Diagnostics;
  using System.Linq;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Forms;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Xaml;

  using Fluent;
  using SIM.Instances;
  using SIM.Pipelines.Agent;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Reinstall;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Tool.Base.Wizards;
  using Core;
  using SIM.Extensions;
  using SIM.Tool.Base.Pipelines;

  using System.ComponentModel;
  using System.Windows.Data;

  #region

  #endregion

  public static class MainWindowHelper
  {
    #region Public methods

    public static void AppPoolRecycle()
    {
      if (SelectedInstance != null)
      {
        SelectedInstance.Recycle();
        OnInstanceSelected();
      }
    }

    public static void AppPoolStart()
    {
      if (SelectedInstance != null)
      {
        SelectedInstance.Start();
        OnInstanceSelected();
      }
    }

    public static void AppPoolStop()
    {
      if (SelectedInstance != null)
      {
        SelectedInstance.Stop();
        OnInstanceSelected();
      }
    }

    public static void EnableRefreshButton(MainWindow mainWindow)
    {
      mainWindow.HomeTabRefreshGroup.IsEnabled = true;
    }

    public static void Initialize()
    {
      using (new ProfileSection("Initialize main window"))
      {
        if (WindowsSettings.AppUiMainWindowWidth.Value > 0)
        {
          double d = WindowsSettings.AppUiMainWindowWidth.Value;
          MainWindow.Instance.MaxWidth = Screen.PrimaryScreen.Bounds.Width;
          MainWindow.Instance.Width = d;
        }

        RefreshInstances();
        RefreshInstaller();
      }
    }

    public static void InitializeContextMenu(ButtonDefinition[] menuItems)
    {
      using (new ProfileSection("Initialize context menu"))
      {
        MainWindow window = MainWindow.Instance;
        foreach (var item in menuItems)
        {
          using (new ProfileSection("Fill in context menu"))
          {
            ProfileSection.Argument("item", item);

            var header = item.Label;
            if (string.IsNullOrEmpty(header))
            {
              window.ContextMenu.Items.Add(new Separator());
              continue;
            }

            InitializeContextMenuItem(item, window.ContextMenu.Items, window, uri => Plugin.GetImage(uri));            
          }
        }
      }
    }

    public static string InitializeInstallerUnsafe(Window window)
    {
      using (new ProfileSection("Initialize Installer (Unsafe)"))
      {
        string message = null;
        var localRepository = ProfileManager.Profile.LocalRepository;

        try
        {
          ProductManager.Initialize(localRepository);
        }
        catch (Exception ex)
        {
          Log.Error(ex, $"Installer failed to init. {ex.Message}");
          message = ex.Message;
        }

        return message;
      }
    }

    public static void InitializeRibbon(TabDefinition[] tabs)
    {
      using (new ProfileSection("Initialize main window ribbon"))
      {
        MainWindow window = MainWindow.Instance;
        using (new ProfileSection("Loading tabs from App.xml"))
        {
          foreach (var tab in tabs)
          {
            // Get Ribbon Tab to insert button to
            InitializeRibbonTab(tab, window, uri => Plugin.GetImage(uri));
          }
        }

        // minimize ribbon
        using (new ProfileSection("Normalizing ribbon"))
        {
          foreach (var tab in window.MainRibbon.Tabs)
          {
            var hiddenGroups = 0;
            foreach (var group in tab.Groups)
            {
              if (group.Items.Count == 0)
              {
                group.Visibility = Visibility.Hidden;
                hiddenGroups += 1;
              }
            }

            if (hiddenGroups == tab.Groups.Count)
            {
              tab.Visibility = Visibility.Hidden;
            }
          }
        }
      }
    }

    public static bool IsInstallerReady()
    {
      try
      {
        ProfileManager.Profile.Validate();
        return true;
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during checking if installer ready");

        return false;
      }
    }

    public static void KillProcess(Instance instance = null)
    {
      instance = instance ?? SelectedInstance;

      if (instance != null)
      {
        foreach (var id in instance.ProcessIds)
        {
          Process process = Process.GetProcessById((int)id);
          Log.Info($"Kill the w3wp.exe worker process ({id}) of the {instance.Name} instance");
          process.Kill();
          OnInstanceSelected();
        }
      }
    }

    public static void MakeInstanceSelected(string name)
    {
      var id = GetListItemID(name);
      MakeInstanceSelected(id);
    }

    public static void OpenProgramLogs()
    {
      CoreApp.OpenFolder(ApplicationManager.LogsFolder);
    }

    public static void Publish(Instance instance, Window owner, PublishMode mode)
    {
      WindowHelper.LongRunningTask(
        () => PublishAsync(instance), "Publish",
        owner, "Publish", $"Publish \'en\' language from \'master\' to \'web\' with mode {mode}");
    }

    public static void RefreshCaches()
    {
      using (new ProfileSection("Refresh caching"))
      {
        CacheManager.ClearAll();
      }
    }

    public static void RefreshEverything()
    {
      using (new ProfileSection("Refresh everything"))
      {
        CacheManager.ClearAll();
        RefreshInstaller();
        RefreshInstances();
      }
    }

    public static void RefreshInstaller()
    {
      using (new ProfileSection("Refresh installer"))
      {
        var mainWindow = MainWindow.Instance;
        DisableInstallButtons(mainWindow);
        DisableRefreshButton(mainWindow);
        WindowHelper.LongRunningTask(RefreshInstallerTask, "Initialization", mainWindow, "Scanning local repository to find supported product packages", "The supported product packages are *.zip files they could be Sitecore packages, standalone packages or regular archive files. For supported files it computes manifests with information how the files should be treated.\n\nThe very first time the operation may take quite a long time, or if you clicked Refresh -> Everything", true);
        EnableRefreshButton(mainWindow);
      }
    }

    public static void RefreshInstances()
    {
      using (new ProfileSection("Refresh instances"))
      {
        var mainWindow = MainWindow.Instance;
        var tabIndex = mainWindow.MainRibbon.SelectedTabIndex;
        var instance = SelectedInstance;
        var name = instance != null ? instance.Name : null;
        var instancesFolder = !CoreAppSettings.CoreInstancesDetectEverywhere.Value ? ProfileManager.Profile.InstancesFolder : null;
      
        WindowHelper.LongRunningTask(() =>
        {
            InstanceManager.Default.Initialize(instancesFolder);
        }, "Refresh Sitecore web sites", mainWindow, "Scanning IIS applications to find Sitecore web sites", "", true);

        Search();
        if (string.IsNullOrEmpty(name))
        {
          mainWindow.MainRibbon.SelectedTabIndex = tabIndex;
          return;
        }

        var list = mainWindow.InstanceList;
        for (int i = 0; i < list.Items.Count; ++i)
        {
          var item = list.Items[i] as Instance;
          if (item != null && item.Name.EqualsIgnoreCase(name))
          {
            list.SelectedIndex = i;
            mainWindow.MainRibbon.SelectedTabIndex = tabIndex;
            return;
          }
        }
      }
    }

    public static void ReinstallInstance([NotNull] Instance instance, Window owner, [NotNull] string license, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(license, nameof(license));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      if (instance.IsSitecore || instance.IsSitecoreEnvironmentMember)
      {
        if (int.Parse(instance.Product.ShortVersion) < 90)
        {
          Product product = instance.Product;
          if (string.IsNullOrEmpty(product.PackagePath))
          {
            if (WindowHelper.ShowMessage("The {0} product isn't presented in your local repository. Would you like to choose the zip installation package?".FormatWith(instance.ProductFullName), MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
            {
              var patt = instance.ProductFullName + ".zip";
              OpenFileDialog fileBrowserDialog = new OpenFileDialog
              {
                Title = @"Choose installation package",
                Multiselect = false,
                CheckFileExists = true,
                Filter = patt + '|' + patt
              };

              if (fileBrowserDialog.ShowDialog() == DialogResult.OK)
              {
                product = Product.Parse(fileBrowserDialog.FileName);
                if (string.IsNullOrEmpty(product.PackagePath))
                {
                  WindowHelper.HandleError("SIM can't parse the {0} package".FormatWith(instance.ProductFullName), true, null);
                  return;
                }
              }
            }
          }

          if (string.IsNullOrEmpty(product.PackagePath))
          {
            return;
          }
        }

        var name = instance.Name;
        WizardPipelineManager.Start("reinstall", owner, null, null, ignore => MakeInstanceSelected(name), () => new ReinstallWizardArgs(instance, connectionString, license));
      }
    }

    public static void Reinstall9Instance([NotNull] Instance instance, Window owner, [NotNull] string license, [NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(license, nameof(license));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      if (instance.IsSitecore || instance.IsSitecoreEnvironmentMember)
      {       
        var name = instance.Name;
        WizardPipelineManager.Start("reinstall9", owner, null, null, ignore => MakeInstanceSelected(name), () => new ReinstallWizardArgs(instance, connectionString, license));
      }
    }

    public static void SoftlyRefreshInstances()
    {
      using (new ProfileSection("Refresh instances (softly)"))
      {
        var instancesFolder = !CoreAppSettings.CoreInstancesDetectEverywhere.Value ? ProfileManager.Profile.InstancesFolder : null;
        InstanceManager.Default.InitializeWithSoftListRefresh(instancesFolder);
        Search();
      }
    }

    public static void UpdateInstallButtons(string message, MainWindow mainWindow)
    {
      mainWindow.HomeTabInstallGroup.IsEnabled = message == null;
      EnableRefreshButton(mainWindow);
      if (message != null)
      {
        WindowHelper.HandleError($"Refresh failed ... {message}", false);
      }
    }

    #endregion

    #region Plugins

    private static RibbonGroupBox CreateGroup(RibbonTabItem tab, string name)
    {
      var group = new RibbonGroupBox
      {
        Name = "{0}{1}Group".FormatWith(tab.Name, name.Replace(" ", "_")),
        Header = name
      };

      tab.Groups.Add(group);

      return group;
    }

    private static RibbonTabItem CreateTab(MainWindow window, string name)
    {
      var tab = new RibbonTabItem
      {
        Name = "{0}Tab".FormatWith(name.Replace(" ", "_")),
        Header = name
      };

      window.MainRibbon.Tabs.Add(tab);

      return tab;
    }

    private static RoutedEventHandler GetClickHandler(IMainWindowButton mainWindowButton)
    {
      var clickHandler = new RoutedEventHandler(delegate
      {
        try
        {
          if (mainWindowButton != null && mainWindowButton.IsEnabled(MainWindow.Instance, SelectedInstance))
          {
            mainWindowButton.OnClick(MainWindow.Instance, SelectedInstance);
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Failed to get click handler", true, ex);
        }
      });

      return clickHandler;
    }

    private static FrameworkElement GetRibbonButton(MainWindow window, Func<string, ImageSource> getImage, ButtonDefinition button, RibbonGroupBox ribbonGroup, IMainWindowButton mainWindowButton)
    {
      Assert.ArgumentNotNull(button, nameof(button));
      Assert.ArgumentNotNull(ribbonGroup, nameof(ribbonGroup));

      var header = button.Label;

      var clickHandler = GetClickHandler(mainWindowButton);

      if (button.Buttons == null || button.Buttons.Length == 0 || button.Buttons.All(x => x == null))
      {
        // create Ribbon Button
        var imageSource = getImage(button.Image);
        var fluentButton = new Fluent.Button
        {
          Icon = imageSource,
          LargeIcon = imageSource,
          Header = header
        };
        fluentButton.Click += clickHandler;
        ribbonGroup.Items.Add(fluentButton);
        return fluentButton;
      }

      // create Ribbon Button
      var splitButton = ribbonGroup.Items.OfType<SplitButton>().SingleOrDefault(x => x.Header.ToString().Trim().EqualsIgnoreCase(header.Trim()));
      if (splitButton == null)
      {
        var imageSource = getImage(button.Image);
        splitButton = new Fluent.SplitButton
        {
          Icon = imageSource,
          LargeIcon = imageSource,
          Header = header
        };

        if (mainWindowButton != null)
        {
          splitButton.Click += clickHandler;
        }
        else
        {
          var childrenButtons = new List<KeyValuePair<string, IMainWindowButton>>();
          splitButton.Tag = childrenButtons;
          splitButton.Click += (sender, args) => splitButton.IsDropDownOpen = true;
        }

        ribbonGroup.Items.Add(splitButton);
      }

      var items = splitButton.Items;
      Assert.IsNotNull(items, nameof(items));

      foreach (var menuItem in button.Buttons)
      {
        if (menuItem == null)
        {
          continue;
        }

        try
        {
          var menuHeader = menuItem.Label;
          if (string.IsNullOrEmpty(menuHeader))
          {
            items.Add(new Separator());
            continue;
          }

          var largeImage = menuItem.Image;
          var menuIcon = string.IsNullOrEmpty(largeImage) ? null : getImage(largeImage);
          var menuHandler = menuItem.Handler;

          var childrenButtons = splitButton.Tag as ICollection<KeyValuePair<string, IMainWindowButton>>;
          if (childrenButtons != null)
          {
            childrenButtons.Add(new KeyValuePair<string, IMainWindowButton>(menuHeader, menuHandler));
          }

          var menuButton = new Fluent.MenuItem()
          {
            Header = menuHeader,
            IsEnabled = menuHandler?.IsEnabled(window, SelectedInstance) ?? true,
            Visibility = menuHandler != null && menuHandler.IsEnabled(window, SelectedInstance) ? Visibility.Visible : Visibility.Collapsed
          };

          if (menuIcon != null)
          {
            menuButton.Icon = menuIcon;
          }

          if (menuHandler != null)
          {
            // bind IsEnabled and IsVisible events
            SetIsEnabledProperty(menuButton, menuHandler);
            SetIsVisibleProperty(menuButton, menuHandler);

            menuButton.Click += delegate
          {
            try
            {
              if (menuHandler.IsEnabled(MainWindow.Instance, SelectedInstance) && menuHandler.IsVisible(MainWindow.Instance, SelectedInstance))
              {
                menuHandler.OnClick(MainWindow.Instance, SelectedInstance);
              }
            }
            catch (Exception ex)
            {
              WindowHelper.HandleError($"Error during handling menu button click: {menuHandler.GetType().FullName}", true, ex);
            }
          };
          }

          items.Add(menuButton);
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError($"Error during initializing ribbon button: {menuItem.Label}", true, ex);
        }
      }

      return splitButton;
    }

    private static RibbonGroupBox GetRibbonGroup(string name, string tabName, string groupName, RibbonTabItem ribbonTab, MainWindow window)
    {
      using (new ProfileSection("Get ribbon group"))
      {
        ProfileSection.Argument("name", name);
        ProfileSection.Argument("tabName", tabName);
        ProfileSection.Argument("groupName", groupName);
        ProfileSection.Argument("ribbonTab", ribbonTab);
        ProfileSection.Argument("window", window);

        var ribbonGroup = window.FindName(groupName) as RibbonGroupBox;

        if (ribbonGroup == null)
        {
          var ribbonTabItem = window.FindName(tabName) as RibbonTabItem;

          if (ribbonTabItem != null)
          {
            var ribbonGroupBoxs = ribbonTabItem.Groups;
            foreach (
              var ribbonGroupBox in ribbonGroupBoxs.Where(ribbonGroupBox => ribbonGroupBox.Header.ToString() == name))
            {
              ribbonGroup = ribbonGroupBox;
              break;
            }
          }

          if (ribbonGroup == null)
          {
            ribbonGroup = CreateGroup(ribbonTab, name);
          }
        }

        return ribbonGroup;
      }
    }

    private static void InitializeContextMenuItem([NotNull] ButtonDefinition menuItemElement, ItemCollection itemCollection, MainWindow window, Func<string, ImageSource> getImage)
    {
      try
      {
        var header = menuItemElement.Label;
        if (string.IsNullOrEmpty(header))
        {
          itemCollection.Add(new Separator());
          return;
        }
        
        // create handler
        var mainWindowButton = menuItemElement.Handler;

        // create Context Menu Item
        var menuItem = new System.Windows.Controls.MenuItem
        {
          Header = header,
          Icon = new Image
          {
            Source = getImage(menuItemElement.Image),
            Width = 16,
            Height = 16
          },
          IsEnabled = mainWindowButton == null || mainWindowButton.IsEnabled(window, SelectedInstance),
          Visibility = mainWindowButton != null && mainWindowButton.IsVisible(window, SelectedInstance) ? Visibility.Visible : Visibility.Collapsed,
          Tag = mainWindowButton
        };

        if (mainWindowButton != null)
        {
          menuItem.Click += (obj, e) =>
          {
            try
            {
              if (mainWindowButton.IsEnabled(MainWindow.Instance, SelectedInstance) && mainWindowButton.IsVisible(MainWindow.Instance, SelectedInstance))
              {
                mainWindowButton.OnClick(MainWindow.Instance, SelectedInstance);
              }
            }
            catch (Exception ex)
            {
              WindowHelper.HandleError("Failed to initialize context menu", true, ex);
            }
          };

          SetIsEnabledProperty(menuItem, mainWindowButton);
          SetIsVisibleProperty(menuItem, mainWindowButton);
        }

        foreach (var childElement in menuItemElement.Buttons ?? new ButtonDefinition[0])
        {
          if (childElement == null)
          {
            continue;
          }

          InitializeContextMenuItem(childElement, menuItem.Items, window, getImage);
        }

        itemCollection.Add(menuItem);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Plugin Menu Item caused an exception");
      }
    }

    private static void InitializeRibbonButton(MainWindow window, Func<string, ImageSource> getImage, ButtonDefinition button, RibbonGroupBox ribbonGroup)
    {
      using (new ProfileSection("Initialize ribbon button"))
      {
        ProfileSection.Argument("button", button);
        ProfileSection.Argument("ribbonGroup", ribbonGroup);
        ProfileSection.Argument("window", window);
        ProfileSection.Argument("getImage", getImage);

        try
        {
          // create handler
          var mainWindowButton = button.Handler;

          FrameworkElement ribbonButton;
          ribbonButton = GetRibbonButton(window, getImage, button, ribbonGroup, mainWindowButton);

          Assert.IsNotNull(ribbonButton, nameof(ribbonButton));

          var width = button.Width;
          double d;
          if (!string.IsNullOrEmpty(width) && double.TryParse(width, out d))
          {
            ribbonButton.Width = d;
          }

          // bind IsEnabled and IsVisible events
          if (mainWindowButton != null)
          {
            ribbonButton.Tag = mainWindowButton;
            ribbonButton.IsEnabled = mainWindowButton.IsEnabled(window, SelectedInstance);
            ribbonButton.Visibility = mainWindowButton.IsVisible(window, SelectedInstance) ? Visibility.Visible : Visibility.Collapsed;
            SetIsEnabledProperty(ribbonButton, mainWindowButton);
            SetIsVisibleProperty(ribbonButton, mainWindowButton);
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError($"Plugin Button caused an exception: {button.Label}", true, ex);
        }
      }
    }

    private static void InitializeRibbonTab([NotNull] TabDefinition tab, MainWindow window, Func<string, ImageSource> getImage)
    {
      Assert.ArgumentNotNull(tab, nameof(tab));

      var name = tab.Name;
      Assert.IsNotNull(name, nameof(name));

      using (new ProfileSection("Initialize ribbon tab"))
      {
        ProfileSection.Argument("name", name);

        var tabName = name + "Tab";
        var ribbonTab = window.FindName(tabName) as RibbonTabItem ?? CreateTab(window, name);
        Assert.IsNotNull(ribbonTab, "Cannot find RibbonTab with {0} name".FormatWith(tabName));

        foreach (var group in tab.Groups)
        {
          Assert.IsNotNull(group, nameof(group));

          // Get Ribbon Group to insert button to
          name = group.Name;
          var groupName = tabName + name + "Group";
          var ribbonGroup = GetRibbonGroup(name, tabName, groupName, ribbonTab, window);

          Assert.IsNotNull(ribbonGroup, "Cannot find RibbonGroup with {0} name".FormatWith(groupName));

          foreach (var button in group.Buttons)
          {
            InitializeRibbonButton(window, getImage, button, ribbonGroup);
          }
        }
      }
    }

    private static void SetIsEnabledProperty(FrameworkElement ribbonButton, IMainWindowButton mainWindowButton)
    {
      ribbonButton.SetBinding(UIElement.IsEnabledProperty, new System.Windows.Data.Binding("SelectedItem")
      {
        Converter = new CustomEnabledConverter(mainWindowButton),
        ElementName = "InstanceList"
      });
    }

    private static void SetIsVisibleProperty(FrameworkElement ribbonButton, IMainWindowButton mainWindowButton)
    {
      ribbonButton.SetBinding(UIElement.VisibilityProperty, new System.Windows.Data.Binding("SelectedItem")
      {
        Converter = new CustomVisibilityConverter(mainWindowButton),
        ElementName = "InstanceList"
      });
    }

    #endregion

    #region Properties

    [CanBeNull]
    public static Instance SelectedInstance
    {
      get
      {
        return MainWindow.Instance.InstanceList.SelectedValue as Instance;
      }
    }

    #endregion

    #region Methods

    #region Public methods

    public static int GetListItemID(long value)
    {
      var itemCollection = MainWindow.Instance.InstanceList.Items;

      for (int i = 0; i < itemCollection.Count; ++i)
      {
        if (((Instance)itemCollection[i]).ID == value)
        {
          return i;
        }
      }

      // YBO: Fix for issue #37. If we haven't found the ID of a newly installed instance, we should refresh the list.
      RefreshInstances();

      for (int i = 0; i < itemCollection.Count; ++i)
      {
        if (((Instance)itemCollection[i]).ID == value)
        {
          return i;
        }
      }

      throw new ArgumentOutOfRangeException("There is no instance with {0} ID in the list".FormatWith(value));
    }

    public static T Invoke<T>(Func<MainWindow, T> func) where T : class
    {
      var window = MainWindow.Instance;
      T result = null;
      window.Dispatcher.Invoke(() => { result = func(window); });
      return result;
    }

    public static void Invoke(Action<MainWindow> func)
    {
      var window = MainWindow.Instance;
      window.Dispatcher.Invoke(() => func(window));
    }

    public static void MakeInstanceSelected(int id)
    {
      var count = MainWindow.Instance.InstanceList.Items.Count;
      if (count == 0)
      {
        return;
      }

      if (id >= count)
      {
        MakeInstanceSelected(count - 1);
        return;
      }

      if (id < 0)
      {
        MakeInstanceSelected(0);
        return;
      }

      MainWindow.Instance.InstanceList.SelectedItem = MainWindow.Instance.InstanceList.Items[id];
      FocusManager.SetFocusedElement(MainWindow.Instance.InstanceList, MainWindow.Instance.InstanceList);
    }

    public static void OnInstanceSelected()
    {
      using (new ProfileSection("Main window instance selected handler"))
      {
        if (SelectedInstance != null)
        {
          if (MainWindow.Instance.HomeTab.IsSelected)
          {
            MainWindow.Instance.OpenTab.IsSelected = true;
          }

          ShowContextMenuItems(SelectedInstance);
          HideContextMenuItems(SelectedInstance);
        }
      }
    }

    private static bool IsSitecoreMember(Instance selectedInstance)
    {
      if (selectedInstance.Product == Product.Undefined || selectedInstance.Product.Release == null)
      {
        return true;
      }

      return false;
    }

    private static bool IsSitecore9(Instance selectedInstance)
    {
      if (selectedInstance.Product.Release.Version.MajorMinorInt >= 90)
      {
        return true;
      }

      return false;
    }

    private static bool IsSitecore91(Instance selectedInstance)
    {
      if (selectedInstance.Product.Release.Version.MajorMinorInt >= 91)
      {
        return true;
      }

      return false;
    }

    public static bool IsEnabledOrVisibleButtonForSitecore91AndMember(Instance selectedInstance)
    {
      if (selectedInstance != null && (IsSitecoreMember(selectedInstance) || IsSitecore91(selectedInstance)))
      {
        return false;
      }

      return true;
    }

    public static bool IsEnabledOrVisibleButtonForSitecore9AndMember(Instance selectedInstance)
    {
      if (selectedInstance != null && (IsSitecoreMember(selectedInstance) || IsSitecore9(selectedInstance)))
      {
        return false;
      }

      return true;
    }

    public static bool IsEnabledOrVisibleButtonForSitecoreMember(Instance selectedInstance)
    {
      if (selectedInstance != null && IsSitecoreMember(selectedInstance))
      {
        return false;
      }

      return true;
    }

    private static void ShowContextMenuItems(Instance selectedInstance)
    {
      ShowHideContextMenuItemsForSitecore9(Visibility.Visible);
      ShowHideContextMenuItemsForSitecore9EnvironmentMember(Visibility.Visible);
    }

    private static void HideContextMenuItems(Instance selectedInstance)
    {
      if (IsSitecoreMember(selectedInstance))
      {
        ShowHideContextMenuItemsForSitecore9(Visibility.Collapsed);
        ShowHideContextMenuItemsForSitecore9EnvironmentMember(Visibility.Collapsed);

        return;
      }

      if (IsSitecore9(selectedInstance))
      {
        ShowHideContextMenuItemsForSitecore9(Visibility.Collapsed);
      }
    }

    private static void ShowHideContextMenuItemsForSitecore9(Visibility visibility)
    {
      for (int i = 0; i < MainWindow.Instance.ContextMenu.Items.Count; i++)
      {
        if (MainWindow.Instance.ContextMenu.Items[i] is System.Windows.Controls.MenuItem &&
            ((MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Backup" ||
            (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Restore" ||
            (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Export" ||
            (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Install modules"))
        {
          (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Visibility = visibility;
          if (MainWindow.Instance.ContextMenu.Items[i+1] is System.Windows.Controls.Separator)
          {
            (MainWindow.Instance.ContextMenu.Items[i+1] as System.Windows.Controls.Separator).Visibility = visibility;
          }
        }
      }
    }

    private static void ShowHideContextMenuItemsForSitecore9EnvironmentMember(Visibility visibility)
    {
      for (int i = 0; i < MainWindow.Instance.ContextMenu.Items.Count; i++)
      {
        if (MainWindow.Instance.ContextMenu.Items[i] is System.Windows.Controls.MenuItem &&
            ((MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Browse Sitecore Website" ||
             (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Browse Sitecore Client" ||
             (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Log in admin" ||
             (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Sitecore Admin" ||
             (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Open Visual Studio" ||
             (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Analyze log files" ||
             (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Open log file" ||
             (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Open web.config file" ||
             (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Header == "Publish Site"))
        {
          (MainWindow.Instance.ContextMenu.Items[i] as System.Windows.Controls.MenuItem).Visibility = visibility;
          if (MainWindow.Instance.ContextMenu.Items[i + 1] is System.Windows.Controls.Separator)
          {
            (MainWindow.Instance.ContextMenu.Items[i + 1] as System.Windows.Controls.Separator).Visibility = visibility;
          }
        }
      }
    }
    // private static void SetupInstanceRestoreButton(string webRootPath)
    // {
    // using (new ProfileSection("MainWindowHelper:SetupInstanceRestoreButton()"))
    // {
    // //MainWindow.Instance.rsbRestore.Items.Clear();

    // try
    // {
    // string backupsFolder;
    // using (new ProfileSection("MainWindowHelper:SetupInstanceRestoreButton(), backupsFolder"))
    // {
    // backupsFolder = SelectedInstance.GetBackupsFolder(webRootPath);
    // }
    // bool hasBackups;
    // using (new ProfileSection("MainWindowHelper:SetupInstanceRestoreButton(), hasBackups"))
    // {
    // hasBackups = FileSystem.Instance.DirectoryExists(backupsFolder) &&
    // FileSystem.Instance.GetDirectories(backupsFolder, "*", SearchOption.TopDirectoryOnly).Length > 0;
    // }
    // MainWindow.Instance.rsbRestore.IsEnabled = hasBackups;
    // }
    // catch (InvalidOperationException ex)
    // {
    // Log.Warn(ex.Message);
    // MainWindow.Instance.rsbRestore.IsEnabled = false;
    // }
    // }
    // }
    public static void OpenFolder([NotNull] string path)
    {
      Assert.ArgumentNotNull(path, nameof(path));

      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        CoreApp.OpenFolder(path);
      }
    }

    public static void Search()
    {
      using (new ProfileSection("Main window search handler"))
      {
        var searchPhrase = Invoke(w => w.SearchTextBox.Text.Trim());
        IEnumerable<Instance> source = InstanceManager.Default.PartiallyCachedInstances;
        if (source == null)
        {
          return;
        }

        // source = source.Select(inst => new CachedInstance((int)inst.ID));
        if (!string.IsNullOrEmpty(searchPhrase))
        {
          source = source.Where(instance => IsInstanceMatch(instance, searchPhrase));
        }

        ICollectionView view = CollectionViewSource.GetDefaultView(source);
        if (!view.GroupDescriptions.OfType<PropertyGroupDescription>().Any(x=>x.PropertyName=="SitecoreEnvironment.Name"))
        {
          view.GroupDescriptions.Add(new PropertyGroupDescription("SitecoreEnvironment.Name"));
        }

        if (!view.SortDescriptions.OfType<SortDescription>().Any(x => x.PropertyName == "SitecoreEnvironment.Name"))
        {
          view.SortDescriptions.Add(new SortDescription("SitecoreEnvironment.Name", ListSortDirection.Ascending));
        }

        if (!view.SortDescriptions.OfType<SortDescription>().Any(x => x.PropertyName == "Name"))
        {
          view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        MainWindow.Instance.InstanceList.DataContext = view;
        MainWindow.Instance.SearchTextBox.Focus();
      }
    }

    #endregion

    #region Private methods

    private static bool IsInstanceMatch(Instance instance, string searchPhrase)
    {
      return instance.Name.ContainsIgnoreCase(searchPhrase) || instance.ProductFullName.ContainsIgnoreCase(searchPhrase) || instance.Product.SearchToken.ContainsIgnoreCase(searchPhrase);
    }

    #endregion

    /*public class CachedInstance : Instance
    {
      private string name;
      private string webRootPath;

      public CachedInstance(int id)
        : base(id)
      {
      }

      public override string Name
      {
        get
        {
          return this.name ?? (this.name = base.Name);
        }
      }

      public override string WebRootPath
      {
        get
        {
          return this.webRootPath ?? (this.webRootPath = base.WebRootPath);
        }
      }
    }*/

    #endregion

    #region Private methods

    private static void DisableInstallButtons(MainWindow mainWindow)
    {
      mainWindow.HomeTabInstallGroup.IsEnabled = false;
    }

    private static void DisableRefreshButton(MainWindow mainWindow)
    {
      mainWindow.HomeTabRefreshGroup.IsEnabled = false;
    }

    private static int GetListItemID(string value)
    {
      var itemCollection = MainWindow.Instance.InstanceList.Items;
      for (int i = 0; i < itemCollection.Count; ++i)
      {
        if (((Instance)itemCollection[i]).Name == value)
        {
          return i;
        }
      }

      throw new ArgumentOutOfRangeException("There is no instance with {0} ID in the list".FormatWith(value));
    }

    private static void PublishAsync(Instance instance)
    {
      try
      {
        PublishAgentHelper.CopyAgentFiles(instance);
        PublishAgentHelper.Publish(instance);
      }
      catch (ThreadAbortException)
      {
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError($"An error occurred while publishing{Environment.NewLine}{ex.Message}", true, ex);
      }
      finally
      {
        AgentHelper.DeleteAgentFiles(instance);
      }
    }

    private static void RefreshInstallerTask()
    {
      var message = InitializeInstallerUnsafe(MainWindow.Instance);
      Invoke((mainWindow) => UpdateInstallButtons(message, mainWindow));
      if (message != null)
      {
        WindowHelper.HandleError($"Cannot find any installation package. {message}", false, null);
      }
    }

    #endregion
  }
}