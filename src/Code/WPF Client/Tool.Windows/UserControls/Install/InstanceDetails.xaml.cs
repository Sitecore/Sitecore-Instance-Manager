#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SIM.Adapters.SqlServer;
using SIM.Adapters.WebServer;
using SIM.Base;
using SIM.Instances;
using SIM.Products;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using System.Collections.ObjectModel;
using System.Xml;

#endregion

namespace SIM.Tool.Windows.UserControls.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   Interaction logic for CollectData.xaml
  /// </summary>
  [UsedImplicitly]
  public partial class InstanceDetails : IWizardStep, IFlowControl
  {
    #region Fields

    /// <summary>
    ///   The standalone products.
    /// </summary>
    private IEnumerable<Product> standaloneProducts;

    [NotNull]
    private readonly ICollection<string> allFrameworkVersions = Environment.Is64BitOperatingSystem ? new[] { "v2.0", "v2.0 32bit", "v4.0", "v4.0 32bit" } : new[] { "v2.0", "v4.0" };

    private InstallWizardArgs installParameters = null;

    #endregion

    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="InstanceDetails" /> class.
    /// </summary>
    public InstanceDetails()
    {
      this.InitializeComponent();

      this.NetFramework.ItemsSource = this.allFrameworkVersions;
    }

    public static bool InstallEverywhere
    {
      get { return WindowsSettings.AppInstallEverywhere.Value; }
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///   Call base.OnMovingNext() in the end of the method if everything is OK
    /// </summary>
    /// <returns> The on moving next. </returns>
    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var productRevision = this.ProductRevision;
      Assert.IsNotNull(productRevision, "productRevision");

      Product product = productRevision.SelectedValue as Product;
      Assert.IsNotNull(product, "product");

      var instanceName = this.InstanceName;
      Assert.IsNotNull(instanceName, "instanceName");

      string name = instanceName.Text.EmptyToNull();
      Assert.IsNotNull(name, @"Instance name isn't set");

      var hostName = this.HostName;
      Assert.IsNotNull(hostName, "hostName");

      string host = hostName.Text.EmptyToNull();
      Assert.IsNotNull(host, "Hostname must not be emoty");

      var rootName = this.RootName;
      Assert.IsNotNull(rootName, "rootName");

      string root = rootName.Text.EmptyToNull();
      Assert.IsNotNull(rootName, "Root folder name must not be emoty");

      string location = this.locationFolder.Text.EmptyToNull();
      Assert.IsNotNull(location, @"The location folder isn't set");

      string rootPath = Path.Combine(location, root);
      bool locationIsPhysical = FileSystem.Local.Directory.HasDriveLetter(rootPath);
      Assert.IsTrue(locationIsPhysical, "The location folder path must be physical i.e. contain a drive letter. Please choose another location folder");

      string webRootPath = Path.Combine(rootPath, "Website");

      bool websiteExists = WebServerManager.WebsiteExists(name);
      if (websiteExists)
      {
        using (var context = WebServerManager.CreateContext("InstanceDetails.OnMovingNext('{0}')".FormatWith(name)))
        {
          var site = context.Sites.Single(s => s != null && s.Name.EqualsIgnoreCase(name));
          var path = WebServerManager.GetWebRootPath(site);
          if (FileSystem.Local.Directory.Exists(path))
          {
            this.Alert("The website with the same name already exists, please choose another instance name.");
            return false;
          }

          if (
            WindowHelper.ShowMessage("There website with the same name already exists, but points to non-existing location. Would you like to delete it?",
              MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) != MessageBoxResult.OK)
          {
            return false;
          }

          site.Delete();
          context.CommitChanges();
        }
      }

      websiteExists = WebServerManager.WebsiteExists(name);
      Assert.IsTrue(!websiteExists, "The website with the same name already exists, please choose another instance name.");

      bool hostExists = WebServerManager.HostBindingExists(host);
      Assert.IsTrue(!hostExists, "Website with the same host name already exists");

      bool rootFolderExists = FileSystem.Local.Directory.Exists(rootPath);
      if (rootFolderExists && InstanceManager.Instances != null)
      {
        if (InstanceManager.Instances.Any(i => i != null && i.WebRootPath.EqualsIgnoreCase(webRootPath)))
        {
          this.Alert("There is another instance with the same root path, please choose another folder");
          return false;
        }

        if (WindowHelper.ShowMessage("The folder with the same name already exists. Would you like to delete it?", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) != MessageBoxResult.OK)
        {
          return false;
        }

        FileSystem.Local.Directory.DeleteIfExists(rootPath);
      }

      var connectionString = ProfileManager.GetConnectionString();
      SqlServerManager.Instance.ValidateConnectionString(connectionString);

      string licensePath = ProfileManager.Profile.License;
      Assert.IsNotNull(licensePath, @"The license file isn't set in the Settings window");
      FileSystem.Local.File.AssertExists(licensePath, "The {0} file is missing".FormatWith(licensePath));

      var netFramework = this.NetFramework;
      Assert.IsNotNull(netFramework, "netFramework");

      var framework = netFramework.SelectedValue.ToString();
      var frameworkArr = framework.Split(' ');
      Assert.IsTrue(frameworkArr.Length > 0, "impossible");

      var force32Bit = frameworkArr.Length == 2;
      var mode = this.Mode;
      Assert.IsNotNull(mode, "mode");

      var modeItem = (ListBoxItem)mode.SelectedValue;
      Assert.IsNotNull(modeItem, "modeItem");

      var isClassic = ((string)modeItem.Content).EqualsIgnoreCase("Classic");

      var args = (InstallWizardArgs)wizardArgs;
      args.InstanceName = name;
      args.InstanceHost = host;
      args.InstanceWebRootPath = webRootPath;
      args.InstanceRootName = root;
      args.InstanceRootPath = rootPath;
      args.InstanceProduct = product;
      args.InstanceConnectionString = connectionString;
      args.LicenseFileInfo = new FileInfo(licensePath);
      args.InstanceAppPoolInfo = new AppPoolInfo {FrameworkVersion = frameworkArr[0].EmptyToNull() ?? "v2.0", Enable32BitAppOnWin64 = force32Bit, ManagedPipelineMode = !isClassic};
      args.Product = product;

      return true;
    }

    public bool OnMovingBack(WizardArgs wizardArg)
    {
      return true;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Alerts the specified message.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <param name="args">
    /// The arguments. 
    /// </param>
    protected void Alert([NotNull] string message, [NotNull] params object[] args)
    {
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(args, "args");

      WindowHelper.ShowMessage(message.FormatWith(args), "Conflict is found", MessageBoxButton.OK, MessageBoxImage.Stop);
    }


    /// <summary>
    ///   Inits this instance.
    /// </summary>
    private void Init()
    {
      using (new ProfileSection("Initializing InstanceDetails", this))
      {
        this.DataContext = new Model();
        this.standaloneProducts = ProductManager.StandaloneProducts;    
      }
    }

    /// <summary>
    /// Instances the name text changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data. 
    /// </param>
    private void InstanceNameTextChanged([CanBeNull] object sender, [CanBeNull] TextChangedEventArgs e)
    {
      using (new ProfileSection("Instance name text changed", this))
      {
        var name = this.InstanceName.Text;
        this.HostName.Text = name;
        this.RootName.Text = name;
        this.sqlPrefix.Text = name; 
      }
    }

    /// <summary>
    /// Picks the location folder.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void PickLocationFolder([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose location folder", this.locationFolder, null);
    }

    /// <summary>
    /// Products the name changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data. 
    /// </param>
    private void ProductNameChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      var productName = this.ProductName;
      Assert.IsNotNull(productName, "productName");

      var grouping = productName.SelectedValue as IGrouping<string, Product>;
      if (grouping == null)
      {
        return;
      }

      var productVersion = this.ProductVersion;
      Assert.IsNotNull(productVersion, "productVersion");

      productVersion.DataContext = grouping.Where(x => x != null).GroupBy(p => p.ShortVersion).Where(x => x != null).OrderBy(p => p.Key);
      this.SelectFirst(productVersion);
    }

    /// <summary>
    /// Products the revision changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data. 
    /// </param>
    private void ProductRevisionChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      using (new ProfileSection("Product revision changed", this))
      {
        var product = this.ProductRevision.SelectedValue as Product;
        if (product == null)
        {
          return;
        }

        var name = product.DefaultInstanceName;
        this.InstanceName.Text = name;
        this.HostName.Text = name;
        this.RootName.Text = product.DefaultFolderName;
        this.sqlPrefix.Text = name;

        var frameworkVersions = new ObservableCollection<string>(this.allFrameworkVersions);

        var m = product.Manifest;
        if (m != null)
        {
          var node = (XmlElement)m.SelectSingleNode("/manifest/*/limitations");
          if (node != null)
          {
            foreach (XmlElement limitation in node.ChildNodes)
            {
              var lname = limitation.Name;
              switch (lname.ToLower())
              {
                case "framework":
                  {
                    var supportedVersions = limitation.SelectElements("supportedVersion");
                    if (supportedVersions != null)
                    {
                      ICollection<string> supportedVersionNames = supportedVersions.Select(supportedVersion => supportedVersion.InnerText).ToArray();
                      for (int i = frameworkVersions.Count - 1; i >= 0; i--)
                      {
                        if (!supportedVersionNames.Contains(frameworkVersions[i]))
                        {
                          frameworkVersions.RemoveAt(i);
                        }
                      }
                    }
                    break;
                  }
              }
            }
          }
        }

        var netFramework = this.NetFramework;
        Assert.IsNotNull(netFramework, "netFramework");

        netFramework.ItemsSource = frameworkVersions;
        netFramework.SelectedIndex = 0; 
      }
    }

    /// <summary>
    /// Products the version changed.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data. 
    /// </param>
    private void ProductVersionChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      var productVersion = this.ProductVersion;
      Assert.IsNotNull(productVersion, "productVersion");

      var grouping = productVersion.SelectedValue as IGrouping<string, Product>;
      if (grouping == null)
      {
        return;
      }

      this.ProductRevision.DataContext = grouping.OrderBy(p => p.Revision);
      this.SelectLast(this.ProductRevision);
    }

    /// <summary>
    /// Selects the specified element.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
    private void Select([NotNull] ComboBox element, [NotNull] string value)
    {
      Assert.ArgumentNotNull(element, "element");
      Assert.ArgumentNotNull(value, "value");

      if (element.Items.Count <= 0)
      {
        return;
      }

      for (int i = 0; i < element.Items.Count; ++i)
      {
        object item0 = element.Items[i];
        IGrouping<string, Product> item1 = item0 as IGrouping<string, Product>;
        if (item1 != null)
        {
          string key = item1.Key;
          if (key.EqualsIgnoreCase(value))
          {
            element.SelectedIndex = i;
            break;
          }
        }
        else
        {
          Product item2 = item0 as Product;
          if (item2 != null)
          {
            string key = item2.Revision;
            if (key.EqualsIgnoreCase(value))
            {
              element.SelectedIndex = i;
              break;
            }
          }
        }
      }
    }

    /// <summary>
    /// Selects the first.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    private void SelectFirst([NotNull] ComboBox element)
    {
      Assert.ArgumentNotNull(element, "element");

      if (element.Items.Count > 0)
      {
        element.SelectedIndex = 0;
      }
    }

    /// <summary>
    /// Selects last.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    private void SelectLast([NotNull] ComboBox element)
    {
        Assert.ArgumentNotNull(element, "element");

        if (element.Items.Count > 0)
        {
            element.SelectedIndex = element.Items.Count-1;
        }
    }

    /// <summary>
    /// Selects by value.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    private void SelectProductByValue([CanBeNull] ComboBox element, [NotNull] string value)
    {
      Assert.ArgumentNotNull(value, "value");

      if (element == null)
      {
        return;
      }

      if (string.IsNullOrEmpty(value))
      {
        this.SelectLast(element);
        return;
      }

      var items = element.Items;
      Assert.IsNotNull(items, "items");
      if (items.Count > 0)
      {
        foreach (IGrouping<string, Product> item in items)
        {
          if (item.First().Name.EqualsIgnoreCase(value, true))
          {
            element.SelectedItem = item;
            break;
          }

          if (item.First().Version.EqualsIgnoreCase(value, true))
          {
            element.SelectedItem = item;
            break;
          }
        }
      }
    }

    private void SelectByValue([NotNull] ComboBox element, string value)
    {
      Assert.ArgumentNotNull(element, "element");

      if (string.IsNullOrEmpty(value))
      {
        SelectLast(element);
        return;
      }

      if (element.Items.Count > 0)
      {
        if (element.Items[0].GetType() == typeof(Product))
        {
          foreach (Product item in element.Items)
          {
            if (item.Name.EqualsIgnoreCase(value, true))
            {
              element.SelectedItem = item;
              break;
            }
            if (item.Revision.EqualsIgnoreCase(value, true))
            {
              element.SelectedItem = item;
              break;
            }
          }
        }
        else
        {
          foreach (var item in element.Items)
          {
            if (item is ComboBoxItem)
            {
              if ((item as ComboBoxItem).Content.ToString().EqualsIgnoreCase(value, true))
              {
                element.SelectedItem = item;
                break;
              }
            }

            if (item is string)
            {
              if ((item as string).EqualsIgnoreCase(value, true))
              {
                element.SelectedItem = item;
                break;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Windows the loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      using (new ProfileSection("Window loaded", this))
      {
        var args = this.installParameters;
        Assert.IsNotNull(args, "args");

        var product = args.Product;
        if (product != null)
        {
          return;
        }

        this.SelectProductByValue(this.ProductName, WindowsSettings.AppInstallationDefaultProduct.Value);
        this.SelectProductByValue(this.ProductVersion, WindowsSettings.AppInstallationDefaultProductVersion.Value);
        this.SelectByValue(this.ProductRevision, WindowsSettings.AppInstallationDefaultProductRevision.Value);

        var netFramework = this.NetFramework;
        Assert.IsNotNull(netFramework, "netFramework");

        if (string.IsNullOrEmpty(WindowsSettings.AppInstallationDefaultFramework.Value))
        {
          netFramework.SelectedIndex = 0;
        }
        else
        {
          this.SelectByValue(netFramework, WindowsSettings.AppInstallationDefaultFramework.Value);
        }

        var mode = this.Mode;
        Assert.IsNotNull(mode, "mode");

        if (string.IsNullOrEmpty(WindowsSettings.AppInstallationDefaultPoolMode.Value))
        {
          mode.SelectedIndex = 0;
        }
        else
        {
          this.SelectByValue(mode, WindowsSettings.AppInstallationDefaultPoolMode.Value);
        } 
      }
    }

    #endregion

    #region Nested type: Model

    /// <summary>
    ///   Defines the model class.
    /// </summary>
    public class Model
    {
      #region Fields

      /// <summary>
      ///   The products.
      /// </summary>
      [CanBeNull]
      [UsedImplicitly]
      public readonly Product[] Products = ProductManager.StandaloneProducts.ToArray();

      /// <summary>
      ///   The name.
      /// </summary>
      [NotNull]
      private string name;

      #endregion

      #region Properties

      /// <summary>
      ///   Gets or sets the name.
      /// </summary>
      /// <value> The name. </value>
      [NotNull]
      [UsedImplicitly]
      public string Name
      {
        get
        {
          return this.name;
        }

        set
        {
          Assert.IsNotNull(value.EmptyToNull(), "Name must not be empty");
          this.name = value;
        }
      }

      /// <summary>
      ///   Gets or sets SelectedProductGroup1.
      /// </summary>
      [UsedImplicitly]
      public IGrouping<string, Product> SelectedProductGroup1 { get; set; }

      #endregion
    }

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      this.Init();

      this.locationFolder.Text = ProfileManager.Profile.InstancesFolder;
      this.ProductName.DataContext = this.standaloneProducts.GroupBy(p => p.Name);

      var args = (InstallWizardArgs)wizardArgs;
      this.installParameters = args;

      Product product = args.Product;
      if (product != null)
      {
        this.Select(this.ProductName, product.Name);
        this.Select(this.ProductVersion, product.ShortVersion);
        this.Select(this.ProductRevision, product.Revision);
      }
      else
      {
        this.SelectFirst(this.ProductName);
      }

      AppPoolInfo info = args.InstanceAppPoolInfo;
      if (info != null)
      {
        var frameworkValue = info.FrameworkVersion + " " + (info.Enable32BitAppOnWin64 ? "32bit" : string.Empty);
        this.SelectByValue(this.NetFramework, frameworkValue);
        this.SelectByValue(this.Mode, info.ManagedPipelineMode ? "Integrated" : "Classic");
      }

      string name = args.InstanceName;
      if (!string.IsNullOrEmpty(name))
      {
        this.InstanceName.Text = name;
      }

      string rootName = args.InstanceRootName;
      if (!string.IsNullOrEmpty(rootName))
      {
        this.RootName.Text = rootName;
      }

      string host = args.InstanceHost;

      if (!string.IsNullOrEmpty(host))
      {
        this.HostName.Text = host;
      }

      if (rootName != null)
      {
        string location = args.InstanceRootPath.TrimEnd(rootName).Trim(new[] { '/', '\\' });
        if (!string.IsNullOrEmpty(location))
        {
          this.locationFolder.Text = location;
        }
      }
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion
  }
}