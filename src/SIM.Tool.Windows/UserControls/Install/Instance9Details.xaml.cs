namespace SIM.Tool.Windows.UserControls.Install
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.IO.Compression;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Xml;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.IO.Real;
  using SIM.Sitecore9Installer;
  using SIM.Tool.Windows.Dialogs;
  using SIM.Tool.Windows.UserControls.Helpers;

  #region

  #endregion

  [UsedImplicitly]
  public partial class Instance9Details : IWizardStep, IFlowControl
  {
    #region Fields

    [NotNull]
    private readonly ICollection<string> _AllFrameworkVersions = Environment.Is64BitOperatingSystem ? new[]
    {
      "v2.0", "v2.0 32bit", "v4.0", "v4.0 32bit"
    } : new[]
    {
      "v2.0", "v4.0"
    };

    private Window owner;
    private InstallWizardArgs _InstallParameters = null;
    private IEnumerable<Product> _StandaloneProducts;

    // According to the following document the maximum length for a path in Windows systems is defined as 260 characters:
    // https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file
    // To prevent possibility of additional characters in the full path while combining, the value is set to 250 characters.
    private const int MaxFileSystemPathLength = 250;

    #endregion

    #region Constructors

    public Instance9Details()
    {
      InitializeComponent();

    }

    #endregion

    #region Public properties

    public static bool InstallEverywhere
    {
      get
      {
        return WindowsSettings.AppInstallEverywhere.Value;
      }
    }

    #endregion

    #region Public Methods

    public bool OnMovingBack(WizardArgs wizardArg)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var productRevision = ProductRevision;
      Assert.IsNotNull(productRevision, nameof(productRevision));

      Product product = productRevision.SelectedValue as Product;
      Assert.IsNotNull(product, nameof(product));
           

      var name = GetValidWebsiteName();
      if (name == null)
        return false;     

      var connectionString = ProfileManager.GetConnectionString();
      SqlServerManager.Instance.ValidateConnectionString(connectionString);

      var licensePath = ProfileManager.Profile.License;
      Assert.IsNotNull(licensePath, @"The license file isn't set in the Settings window");
      FileSystem.FileSystem.Local.File.AssertExists(licensePath, "The {0} file is missing".FormatWith(licensePath));

      
      var args = (Install9WizardArgs)wizardArgs;      
      args.InstanceName = name;
      args.InstanceProduct = product;
      args.InstanceConnectionString = connectionString;
      args.LicenseFileInfo = new FileInfo(licensePath);
      args.Product = product;

      SolrDefinition solr = this.Solrs.SelectedItem as SolrDefinition;
      if (solr == null)
      {
        WindowHelper.ShowMessage("Please provide solr.");
        return false;
      }

      args.SolrUrl = solr.Url; //this.solrUrl.Text;
      args.SorlRoot = solr.Root; //this.solrRoot.Text;
      args.ScriptRoot = System.IO.Path.Combine(Directory.GetParent(args.Product.PackagePath).FullName, System.IO.Path.GetFileNameWithoutExtension(args.Product.PackagePath));

      string packageFullPath = args.ScriptRoot + ".zip";
      if (File.Exists(packageFullPath))
      {

        int maxFileFullPathLength = MaxFileSystemPathLength - args.ScriptRoot.Length;
        using (ZipArchive zipArchive = ZipFile.OpenRead(packageFullPath))
        {
          foreach (ZipArchiveEntry entry in zipArchive.Entries)
          {
            if (maxFileFullPathLength + entry.FullName.Length > MaxFileSystemPathLength)
            {
              WindowHelper.ShowMessage("The full path length of some files in the package after unzipping is too long! Please change the path of the Local Repository folder in Settings, so it contains less path length.");
              return false;
            }
          }
        }
      }
      else
      {
        WindowHelper.ShowMessage(string.Format("Please make sure that the \"{0}\" package exists.", packageFullPath));
        return false;
      }

      if (!Directory.Exists(args.ScriptRoot))
      {
        Directory.CreateDirectory(args.ScriptRoot);
        WindowHelper.LongRunningTask(() => this.UnpackInstallationFiles(args), "Unpacking unstallation files.", wizardArgs.WizardWindow);
        WindowHelper.LongRunningTask(() => InstallTasksHelper.CopyCustomSifConfig(args), "Copying custom SIF configuration files to the install folder.", wizardArgs.WizardWindow);
        WindowHelper.LongRunningTask(() => InstallTasksHelper.AddUninstallTasks(args), "Add Uninstall tasks to the OOB config files.", wizardArgs.WizardWindow);
      }
      else
      {
        if (MessageBox.Show(string.Format("Path '{0}' already exists. Do you want to overwrite it?", args.ScriptRoot), "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          Directory.Delete(args.ScriptRoot, true);
          Directory.CreateDirectory(args.ScriptRoot);
          WindowHelper.LongRunningTask(()=>this.UnpackInstallationFiles(args), "Unpacking installation files.", wizardArgs.WizardWindow);
          WindowHelper.LongRunningTask(() => InstallTasksHelper.CopyCustomSifConfig(args), "Copying custom SIF configuration files to the install folder.", wizardArgs.WizardWindow);
          WindowHelper.LongRunningTask(() => InstallTasksHelper.AddUninstallTasks(args), "Add Uninstall tasks to the OOB config files.", wizardArgs.WizardWindow);
        }
       
      }

      string rootPath = this.LocationText.Text; 
      if (!args.ScriptsOnly)
      {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
          WindowHelper.ShowMessage("Please specify location.");
          return false;
        }

        if (SIM.FileSystem.FileSystem.Local.Directory.Exists(rootPath))
        {
          if (Directory.EnumerateFileSystemEntries(rootPath).Any())
          {
            if (WindowHelper.ShowMessage("The folder with the same name already exists and is not empty. Would you like to delete it?", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) == MessageBoxResult.OK)
            {
              SIM.FileSystem.FileSystem.Local.Directory.DeleteIfExists(rootPath, null);
              SIM.FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath);
            }
          }
        }
        else
        {
          SIM.FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath);
        }
      }

      Tasker tasker = new Tasker(args.ScriptRoot, Path.GetFileNameWithoutExtension(args.Product.PackagePath),rootPath);    
      InstallParam sqlServer = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlServer");
      if (sqlServer != null)
      {
        sqlServer.Value = args.InstanceConnectionString.DataSource;
      }

      InstallParam sqlAdminUser= tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlAdminUser");
      if (sqlAdminUser != null)
      {
        sqlAdminUser.Value = args.InstanceConnectionString.UserID;
      }

      InstallParam sqlAdminPass = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlAdminPassword");
      if (sqlAdminPass != null)
      {
        sqlAdminPass.Value = args.InstanceConnectionString.Password;
      }

      InstallParam sqlDbPrefix = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlDbPrefix");
      if (sqlDbPrefix != null)
      {
        sqlDbPrefix.Value = args.InstanceName;
      }

      InstallParam licenseFile = tasker.GlobalParams.FirstOrDefault(p => p.Name == "LicenseFile");
      if (licenseFile != null)
      {
        licenseFile.Value = args.LicenseFileInfo.FullName;
      }     

      InstallParam solrRoot = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SolrRoot");
      if (solrRoot != null)
      {
        solrRoot.Value = args.SorlRoot;
      }

      InstallParam solrService = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SolrService");
      if (solrService != null&&!string.IsNullOrEmpty(solr.Service))
      {
        solrService.Value = solr.Service; // this.SolrService.Text;
      }

      InstallParam solrUrl = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SolrUrl");
      if (solrUrl != null)
      {
        solrUrl.Value = args.SolrUrl;
      }      

      args.ScriptsOnly = this.scriptsOnly.IsChecked ?? false;
      args.Tasker = tasker;
      
      VersionToSolr vts = ProfileManager.Profile.VersionToSolrMap.FirstOrDefault(s => s.Vesrion == product.ShortVersion);
      if (vts == null)
      {
        vts = new VersionToSolr();
        vts.Vesrion = product.ShortVersion;
        ProfileManager.Profile.VersionToSolrMap.Add(vts);
      }
      
      vts.Solr = solr.Name;
      ProfileManager.SaveChanges(ProfileManager.Profile);

      return true;
    }

    public void UnpackInstallationFiles(Install9WizardArgs args)
    {
      RealZipFile zip = new RealZipFile(new RealFile(new RealFileSystem(), args.Product.PackagePath));
      zip.ExtractTo(new RealFolder(new RealFileSystem(), args.ScriptRoot));
      string configFilesZipPath = Directory.GetFiles(args.ScriptRoot, "*Configuration files*.zip").First();
      RealZipFile configFilesZip = new RealZipFile(new RealFile(new RealFileSystem(), configFilesZipPath));
      configFilesZip.ExtractTo(new RealFolder(new RealFileSystem(), args.ScriptRoot));
    }


    private static string GetWebRootPath(string rootPath)
    {
      var webRootPath = Path.Combine(rootPath, "Website");
      return webRootPath;
    }

   

    

    [CanBeNull]
    private string GetValidWebsiteName()
    {
      var instanceName = InstanceName;
      Assert.IsNotNull(instanceName, nameof(instanceName));

      var name = instanceName.Text.EmptyToNull();
      Assert.IsNotNull(name, @"Instance name isn't set");

      var websiteExists = WebServerManager.WebsiteExists(name);
      if (websiteExists)
      {
        using (var context = WebServerManager.CreateContext())
        {
          var site = context.Sites.Single(s => s != null && s.Name.EqualsIgnoreCase(name));
          var path = WebServerManager.GetWebRootPath(site);
          if (FileSystem.FileSystem.Local.Directory.Exists(path))
          {
            Alert("The website with the same name already exists, please choose another instance name.");
            return null;
          }

            if (
                WindowHelper.ShowMessage(
                  $"A website with the name {name} already exists. Would you like to remove it?",
                    MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) != MessageBoxResult.OK)
            {
                return null;
            }

            site.Delete();
          context.CommitChanges();
        }
      }

      websiteExists = WebServerManager.WebsiteExists(name);
      Assert.IsTrue(!websiteExists, "The website with the same name already exists, please choose another instance name.");
      return name;
    }       

    #endregion

    #region Methods

    #region Protected methods

    protected void Alert([NotNull] string message, [NotNull] params object[] args)
    {
      Assert.ArgumentNotNull(message, nameof(message));
      Assert.ArgumentNotNull(args, nameof(args));

      WindowHelper.ShowMessage(message.FormatWith(args), "Conflict is found", MessageBoxButton.OK, MessageBoxImage.Stop);
    }

    #endregion

    #region Private methods

    private void Init()
    {
      using (new ProfileSection("Initializing InstanceDetails", this))
      {
        DataContext = new Model();
        _StandaloneProducts = ProductManager.StandaloneProducts.Where(p=>int.Parse(p.ShortVersion)>=90&&!p.PackagePath.EndsWith(".scwdp.zip"));
        this.Solrs.DataContext = ProfileManager.Profile.Solrs;
      }
    }        

    private void ProductNameChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      var productName = ProductName;
      Assert.IsNotNull(productName, nameof(productName));

      var grouping = productName.SelectedValue as IGrouping<string, Product>;
      if (grouping == null)
      {
        return;
      }

      var productVersion = ProductVersion;
      Assert.IsNotNull(productVersion, nameof(productVersion));

      productVersion.DataContext = grouping.Where(x => x != null).GroupBy(p => p.ShortVersion).Where(x => x != null).OrderBy(p => p.Key);
      SelectFirst(productVersion);
    }

    private void ProductRevisionChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      using (new ProfileSection("Product revision changed", this))
      {
        var product = ProductRevision.SelectedValue as Product;
        if (product == null)
        {
          return;
        }

        var name = product.DefaultInstanceName;
        InstanceName.Text = name;

        var frameworkVersions = new ObservableCollection<string>(_AllFrameworkVersions);

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
      }
    }

    private void ProductVersionChanged([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      var productVersion = ProductVersion;
      Assert.IsNotNull(productVersion, nameof(productVersion));

      var grouping = productVersion.SelectedValue as IGrouping<string, Product>;
      if (grouping == null)
      {
        return;
      }

      ProductRevision.DataContext = grouping.OrderBy(p => p.Revision);
      SelectLast(ProductRevision);

      var solrName = ProfileManager.Profile.VersionToSolrMap.FirstOrDefault(s => s.Vesrion == grouping.Key)?.Solr;

      this.Solrs.SelectedItem = ((List<SolrDefinition>)Solrs.DataContext).FirstOrDefault(s => s.Name== solrName);
    }

    private void Select([NotNull] Selector element, [NotNull] string value)
    {
      Assert.ArgumentNotNull(element, nameof(element));
      Assert.ArgumentNotNull(value, nameof(value));

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
          var key = item1.Key;
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
            var key = item2.Revision;
            if (key.EqualsIgnoreCase(value))
            {
              element.SelectedIndex = i;
              break;
            }
          }
        }
      }
    }

    private void SelectByValue([NotNull] Selector element, string value)
    {
      Assert.ArgumentNotNull(element, nameof(element));

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
            if (item is ContentControl)
            {
              if ((item as ContentControl).Content.ToString().EqualsIgnoreCase(value, true))
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

    private void SelectFirst([NotNull] Selector element)
    {
      Assert.ArgumentNotNull(element, nameof(element));

      if (element.Items.Count > 0)
      {
        element.SelectedIndex = 0;
      }
    }

    private void SelectLast([NotNull] Selector element)
    {
      Assert.ArgumentNotNull(element, nameof(element));

      if (element.Items.Count > 0)
      {
        element.SelectedIndex = element.Items.Count - 1;
      }
    }

    private void SelectProductByValue([CanBeNull] Selector element, [NotNull] string value)
    {
      Assert.ArgumentNotNull(value, nameof(value));

      if (element == null)
      {
        return;
      }

      if (string.IsNullOrEmpty(value))
      {
        SelectLast(element);
        return;
      }

      var items = element.Items;
      Assert.IsNotNull(items, nameof(items));
      if (items.Count > 0)
      {
        foreach (IGrouping<string, Product> item in items)
        {
          if (item.First().Name.EqualsIgnoreCase(value, true))
          {
            element.SelectedItem = item;
            break;
          }

          if (item.First().TwoVersion.EqualsIgnoreCase(value, true))
          {
            element.SelectedItem = item;
            break;
          }
        }
      }
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      using (new ProfileSection("Window loaded", this))
      {
        var args = _InstallParameters;
        Assert.IsNotNull(args, nameof(args));

        var product = args.Product;
        if (product != null)
        {
          return;
        }

        SelectProductByValue(ProductName, WindowsSettings.AppInstallationDefaultProduct.Value);
        SelectProductByValue(ProductVersion, WindowsSettings.AppInstallationDefaultProductVersion.Value);
        SelectByValue(ProductRevision, WindowsSettings.AppInstallationDefaultProductRevision.Value);       
      }
    }

    #endregion

    #endregion

    #region Nested type: Model

    public class Model
    {
      #region Fields

      [CanBeNull]
      [UsedImplicitly]
      public readonly Product[] _Products = ProductManager.StandaloneProducts.ToArray();

      [NotNull]
      private string _Name;

      #endregion

      #region Properties

      [NotNull]
      [UsedImplicitly]
      public string Name
      {
        get
        {
          return _Name;
        }

        set
        {
          Assert.IsNotNull(value.EmptyToNull(), "Name must not be empty");
          _Name = value;
        }
      }

      [UsedImplicitly]
      public IGrouping<string, Product> SelectedProductGroup1 { get; set; }

      #endregion
    }

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      Init();
      this.owner = wizardArgs.WizardWindow;
      ProductName.DataContext = _StandaloneProducts.GroupBy(p => p.Name);

      var args = (InstallWizardArgs)wizardArgs;
      _InstallParameters = args;

      Product product = args.Product;
      if (product != null)
      {
        Select(ProductName, product.Name);
        Select(ProductVersion, product.ShortVersion);
        Select(ProductRevision, product.Revision);
      }
      else
      {
        SelectFirst(ProductName);
      }      

      var name = args.InstanceName;
      if (!string.IsNullOrEmpty(name))
      {
        InstanceName.Text = name;
      }

      //this.LocationText.Text =Path.Combine(ProfileManager.Profile.InstancesFolder,args.InstanceName);
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion

    private void LocationBtn_Click(object sender, RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose location folder", this.LocationText, null);
    }

    private void AddSolr_Click(object sender, RoutedEventArgs e)
    {
      SolrDefinition solr = WindowHelper.ShowDialog<AddSolrDialog>(ProfileManager.Profile.Solrs, this.owner) as SolrDefinition;
      if (solr != null)
      {
        ProfileManager.Profile.Solrs.Add(solr);
        ProfileManager.SaveChanges(ProfileManager.Profile);
        this.Solrs.SelectedItem = solr;
      }
    }
  }
}
