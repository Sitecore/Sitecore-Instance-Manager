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
  using System.Xml;
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
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Tool.Base.Wizards;
  using Core;
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

        MainWindowHelper.RefreshInstances();
        MainWindowHelper.RefreshInstaller();
      }
    }

    public static void InitializeContextMenu(XmlDocumentEx appDocument)
    {
      using (new ProfileSection("Initialize context menu"))
      {
        ProfileSection.Argument("appDocument", appDocument);

        MainWindow window = MainWindow.Instance;
        var menuItems = appDocument.SelectElements("/app/mainWindow/contextMenu/*");

        foreach (var item in menuItems)
        {
          using (new ProfileSection("Fill in context menu"))
          {
            ProfileSection.Argument("item", item);

            if (item.Name == "item")
            {
              InitializeContextMenuItem(item, window.ContextMenu.Items, window, uri => Plugin.GetImage(uri, "App.xml"));
            }
            else if (item.Name == "separator")
            {
              window.ContextMenu.Items.Add(new Separator());
            }
            else if (item.Name == "plugins")
            {
              Log.Error("Plugins no longer supported");
            }
          }
        }
      }
    }

    public static string InitializeInstallerUnsafe(Window window)
    {
      using (new ProfileSection("Initialize Installer (Unsafe)"))
      {
        string message = null;
        string localRepository = ProfileManager.Profile.LocalRepository;

        try
        {
          ProductManager.Initialize(localRepository);
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Installer failed to init. {0}", ex.Message);
          message = ex.Message;
        }

        return message;
      }
    }

    public static void InitializeRibbon(XmlDocument appDocument)
    {
      using (new ProfileSection("Initialize main window ribbon"))
      {
        MainWindow window = MainWindow.Instance;
        using (new ProfileSection("Loading tabs from App.xml"))
        {
          var tabs = appDocument.SelectElements("/app/mainWindow/ribbon/tab");
          foreach (var tabElement in tabs)
          {
            // Get Ribbon Tab to insert button to
            InitializeRibbonTab(tabElement, window, uri => Plugin.GetImage(uri, "App.xml"));
          }
        }
        
        // minimize ribbon
        using (new ProfileSection("Normalizing ribbon"))
        {
          foreach (var tab in window.MainRibbon.Tabs)
          {
            int hiddenGroups = 0;
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
          Log.Info("Kill the w3wp.exe worker process ({0}) of the {1} instance", id, instance.Name);
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

    public static void OpenManifestsFolder()
    {
      OpenFolder("Manifests");
    }

    public static void OpenProgramLogs()
    {
      CoreApp.OpenFolder(ApplicationManager.LogsFolder);
    }

    public static void Publish(Instance instance, Window owner, PublishMode mode)
    {
      WindowHelper.LongRunningTask(
        () => PublishAsync(instance), "Publish", 
        owner, "Publish", "Publish 'en' language from 'master' to 'web' with mode " + mode);
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
        MainWindowHelper.RefreshInstaller();
        MainWindowHelper.RefreshInstances();
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
        string instancesFolder = !CoreAppSettings.CoreInstancesDetectEverywhere.Value ? ProfileManager.Profile.InstancesFolder : null;
        InstanceManager.Initialize(instancesFolder);
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
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(license, "license");
      Assert.ArgumentNotNull(connectionString, "connectionString");

      if (instance.IsSitecore)
      {
        Product product = instance.Product;
        if (string.IsNullOrEmpty(product.PackagePath))
        {
          if (WindowHelper.ShowMessage("The {0} product isn't presented in your local repository. Would you like to choose the zip installation package?".FormatWith(instance.ProductFullName), MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
          {
            string patt = instance.ProductFullName + ".zip";
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

        ReinstallArgs args;
        try
        {
          args = new ReinstallArgs(instance, connectionString, license, SIM.Pipelines.Install.Settings.CoreInstallWebServerIdentity.Value, Settings.CoreInstallNotFoundTransfer.Value);
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError(ex.Message, false, ex);
          return;
        }

        var name = instance.Name;
        WizardPipelineManager.Start("reinstall", owner, args, null, () => MainWindowHelper.MakeInstanceSelected(name));
      }
    }

    public static void SoftlyRefreshInstances()
    {
      using (new ProfileSection("Refresh instances (softly)"))
      {
        var instancesFolder = !CoreAppSettings.CoreInstancesDetectEverywhere.Value ?  ProfileManager.Profile.InstancesFolder : null;
        InstanceManager.InitializeWithSoftListRefresh(instancesFolder);
        Search();
      }
    }

    public static void UpdateInstallButtons(string message, MainWindow mainWindow)
    {
      mainWindow.HomeTabInstallGroup.IsEnabled = message == null;
      EnableRefreshButton(mainWindow);
      if (message != null)
      {
        WindowHelper.HandleError("Refresh failed ... " + message, false);
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
            MainWindowHelper.RefreshInstances();
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError(ex.Message, true);
        }
      });

      return clickHandler;
    }

    private static FrameworkElement GetRibbonButton(MainWindow window, Func<string, ImageSource> getImage, XmlElement button, RibbonGroupBox ribbonGroup, IMainWindowButton mainWindowButton)
    {
      var header = button.GetNonEmptyAttribute("label");

      var clickHandler = GetClickHandler(mainWindowButton);

      if (button.ChildNodes.Count == 0)
      {
        // create Ribbon Button
        var imageSource = getImage(button.GetNonEmptyAttribute("largeImage"));
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
        var imageSource = getImage(button.GetNonEmptyAttribute("largeImage"));
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
      Assert.IsNotNull(items, "items");

      foreach (var menuItem in button.ChildNodes.OfType<XmlElement>())
      {
        if (menuItem == null)
        {
          continue;
        }

        try
        {
          var name = menuItem.Name;
          if (name.EqualsIgnoreCase("separator"))
          {
            items.Add(new Separator());
            continue;
          }

          if (!name.EqualsIgnoreCase("button"))
          {
            Log.Error("This element is not supported as SplitButton element: {0}", menuItem.OuterXml);
            continue;
          }

          var menuHeader = menuItem.GetAttribute("label");
          var largeImage = menuItem.GetAttribute("largeImage");
          var menuIcon = string.IsNullOrEmpty(largeImage) ? null : getImage(largeImage);
          var menuHandler = (IMainWindowButton)Plugin.CreateInstance(menuItem);
          Assert.IsNotNull(menuHandler, "model");

          var childrenButtons = splitButton.Tag as ICollection<KeyValuePair<string, IMainWindowButton>>;
          if (childrenButtons != null)
          {
            childrenButtons.Add(new KeyValuePair<string, IMainWindowButton>(menuHeader, menuHandler));
          }

          var menuButton = new Fluent.MenuItem()
          {
            Header = menuHeader, 
            IsEnabled = menuHandler.IsEnabled(window, SelectedInstance)
          };

          if (menuIcon != null)
          {
            menuButton.Icon = menuIcon;
          }

          // bind IsEnabled event
          SetIsEnabledProperty(menuButton, menuHandler);
          
          menuButton.Click += delegate
          {
            try
            {
              if (menuHandler.IsEnabled(MainWindow.Instance, SelectedInstance))
              {
                menuHandler.OnClick(MainWindow.Instance, SelectedInstance);
                MainWindowHelper.RefreshInstances();
              }
            }
            catch (Exception ex)
            {
              WindowHelper.HandleError("Error during handling menu button click: " + menuHandler.GetType().FullName, true, ex);
            }
          };

          items.Add(menuButton);
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Error during initializing ribbon button: " + menuItem.OuterXml, true, ex);
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

    private static void InitializeContextMenuItem(XmlElement menuItemElement, ItemCollection itemCollection, MainWindow window, Func<string, ImageSource> getImage)
    {
      try
      {
        if (menuItemElement.Name.EqualsIgnoreCase("separator"))
        {
          itemCollection.Add(new Separator());
          return;
        }

        if (!menuItemElement.Name.EqualsIgnoreCase("item"))
        {
          Assert.IsTrue(false, "The element is not supported: {0}".FormatWith(menuItemElement.OuterXml));
        }

        // create handler
        var mainWindowButton = (IMainWindowButton)Plugin.CreateInstance(menuItemElement);

        // create Context Menu Item
        var menuItem = new System.Windows.Controls.MenuItem
        {
          Header = menuItemElement.GetNonEmptyAttribute("header"), 
          Icon = new Image
          {
            Source = getImage(menuItemElement.GetNonEmptyAttribute("image")), 
            Width = 16, 
            Height = 16
          }, 
          IsEnabled = mainWindowButton == null || mainWindowButton.IsEnabled(window, SelectedInstance), 
          Tag = mainWindowButton
        };

        if (mainWindowButton != null)
        {
          menuItem.Click += (obj, e) =>
          {
            try
            {
              if (mainWindowButton.IsEnabled(MainWindow.Instance, SelectedInstance))
              {
                mainWindowButton.OnClick(MainWindow.Instance, SelectedInstance);
                MainWindowHelper.RefreshInstances();
              }
            }
            catch (Exception ex)
            {
              WindowHelper.HandleError(ex.Message, true);
            }
          };

          SetIsEnabledProperty(menuItem, mainWindowButton);
        }

        foreach (var childElement in menuItemElement.ChildNodes.OfType<XmlElement>())
        {
          InitializeContextMenuItem(childElement, menuItem.Items, window, getImage);
        }

        itemCollection.Add(menuItem);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Plugin Menu Item caused an exception");
      }
    }

    private static void InitializeRibbonButton(MainWindow window, Func<string, ImageSource> getImage, XmlElement button, RibbonGroupBox ribbonGroup)
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
          var mainWindowButton = (IMainWindowButton)Plugin.CreateInstance(button);


          FrameworkElement ribbonButton;
          ribbonButton = GetRibbonButton(window, getImage, button, ribbonGroup, mainWindowButton);

          Assert.IsNotNull(ribbonButton, "ribbonButton");

          var width = button.GetAttribute("width");
          double d;
          if (!string.IsNullOrEmpty(width) && double.TryParse(width, out d))
          {
            ribbonButton.Width = d;
          }

          // bind IsEnabled event
          if (mainWindowButton != null)
          {
            ribbonButton.Tag = mainWindowButton;
            ribbonButton.IsEnabled = mainWindowButton.IsEnabled(window, SelectedInstance);
            SetIsEnabledProperty(ribbonButton, mainWindowButton);
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Plugin Button caused an exception: " + button.OuterXml, true, ex);
        }
      }
    }

    private static void InitializeRibbonGroup(string name, string tabName, XmlElement groupElement, RibbonTabItem ribbonTab, MainWindow window, Func<string, ImageSource> getImage)
    {
      using (new ProfileSection("Initialize ribbon group"))
      {
        ProfileSection.Argument("name", name);
        ProfileSection.Argument("tabName", tabName);
        ProfileSection.Argument("groupElement", groupElement);
        ProfileSection.Argument("ribbonTab", ribbonTab);
        ProfileSection.Argument("window", window);
        ProfileSection.Argument("getImage", getImage);

        // Get Ribbon Group to insert button to
        name = groupElement.GetNonEmptyAttribute("name");
        var groupName = tabName + name + "Group";
        var ribbonGroup = GetRibbonGroup(name, tabName, groupName, ribbonTab, window);

        Assert.IsNotNull(ribbonGroup, "Cannot find RibbonGroup with {0} name".FormatWith(groupName));

        var buttons = SelectNonEmptyCollection(groupElement, "button");
        foreach (var button in buttons)
        {
          InitializeRibbonButton(window, getImage, button, ribbonGroup);
        }
      }
    }

    private static void InitializeRibbonTab(XmlElement tabElement, MainWindow window, Func<string, ImageSource> getImage)
    {
      var name = tabElement.GetNonEmptyAttribute("name");
      if (string.IsNullOrEmpty(name))
      {
        Log.Error("Ribbon tab doesn't have name: {0}",  tabElement.OuterXml);
        return;
      }

      using (new ProfileSection("Initialize ribbon tab"))
      {
        ProfileSection.Argument("name", name);

        var tabName = name + "Tab";
        var ribbonTab = window.FindName(tabName) as RibbonTabItem ?? CreateTab(window, name);
        Assert.IsNotNull(ribbonTab, "Cannot find RibbonTab with {0} name".FormatWith(tabName));

        var groups = SelectNonEmptyCollection(tabElement, "group");
        foreach (XmlElement groupElement in groups)
        {
          InitializeRibbonGroup(name, tabName, groupElement, ribbonTab, window, getImage);
        }
      }
    }

    private static IEnumerable<XmlElement> SelectNonEmptyCollection(XmlElement xmlElement, string name)
    {
      var collection = xmlElement.SelectElements(name).ToArray();
      Assert.IsTrue(collection.Length > 0, "<{0}> doesn't contain any <{1}> element".FormatWith(xmlElement.Name, name));
      return collection;
    }

    private static void SetIsEnabledProperty(FrameworkElement ribbonButton, IMainWindowButton mainWindowButton)
    {
      ribbonButton.SetBinding(UIElement.IsEnabledProperty, new System.Windows.Data.Binding("SelectedItem")
      {
        Converter = new CustomConverter(mainWindowButton), 
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

    public static void ChangeAppPoolMode(System.Windows.Controls.MenuItem menuItem)
    {
      var selectedInstance = SelectedInstance;
      WindowHelper.LongRunningTask(() => MainWindow.Instance.Dispatcher.Invoke(
        new Action(delegate
        {
          string header = menuItem.Header.ToString();
          selectedInstance.SetAppPoolMode(header.Contains("4.0"), header.Contains("32bit"));
          OnInstanceSelected();
        })), "Changing application pool", MainWindow.Instance, null, "The IIS metabase is being updated");
    }

    public static void CloseMainWindow()
    {
      MainWindow.Instance.Dispatcher.InvokeShutdown();
      MainWindow.Instance.Close();
    }

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
      window.Dispatcher.Invoke(new Action(() => { result = func(window); }));
      return result;
    }

    public static void Invoke(Action<MainWindow> func)
    {
      var window = MainWindow.Instance;
      window.Dispatcher.Invoke(new Action(() => func(window)));
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
        if (SelectedInstance != null && MainWindow.Instance.HomeTab.IsSelected)
        {
          MainWindow.Instance.OpenTab.IsSelected = true;
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
      Assert.ArgumentNotNull(path, "path");

      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        CoreApp.OpenFolder(path);
      }
    }

    public static void Search()
    {
      using (new ProfileSection("Main window search handler"))
      {
        string searchPhrase = Invoke(w => w.SearchTextBox.Text.Trim());
        IEnumerable<Instance> source = InstanceManager.PartiallyCachedInstances;
        if (source == null)
        {
          return;
        }

        // source = source.Select(inst => new CachedInstance((int)inst.ID));
        if (!string.IsNullOrEmpty(searchPhrase))
        {
          source = source.Where(instance => IsInstanceMatch(instance, searchPhrase));
        }

        source = source.OrderBy(instance => instance.Name);
        MainWindow.Instance.InstanceList.DataContext = source;
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
        WindowHelper.HandleError("An error occurred while publishing" + Environment.NewLine + ex.Message, true, ex);
      }
      finally
      {
        AgentHelper.DeleteAgentFiles(instance);
      }
    }

    private static void RefreshInstallerTask()
    {
      string message = InitializeInstallerUnsafe(MainWindow.Instance);
      MainWindowHelper.Invoke((mainWindow) => MainWindowHelper.UpdateInstallButtons(message, mainWindow));
      if (message != null)
      {
        WindowHelper.HandleError("Cannot find any installation package. " + message, false, null);
      }
    }

    #endregion
  }
}