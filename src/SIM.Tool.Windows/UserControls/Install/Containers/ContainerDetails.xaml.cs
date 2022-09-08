using JetBrains.Annotations;
using SIM.Extensions;
using SIM.IO.Real;
using SIM.Products;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.Helpers;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SIM.Tool.Windows.UserControls.Install.Containers
{
  [UsedImplicitly]
  public partial class ContainerDetails : IWizardStep, IFlowControl
  {
    #region Fields


    private Window owner;
    private InstallWizardArgs _InstallParameters = null;
    private IEnumerable<Product> _Products;

    // According to the following document the maximum length for a path in Windows systems is defined as 260 characters:
    // https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file
    // To prevent possibility of additional characters in the full path while combining, the value is set to 250 characters.
    private const int MaxFileSystemPathLength = 250;

    #endregion

    #region Constructors

    public ContainerDetails()
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

      var licensePath = ProfileManager.Profile.License;
      Assert.IsNotNull(licensePath, @"The license file isn't set in the Settings window");
      FileSystem.FileSystem.Local.File.AssertExists(licensePath, "The {0} file is missing".FormatWith(licensePath));

      var args = (InstallContainerWizardArgs)wizardArgs;
      args.InstanceName = name;
      args.InstanceProduct = product;
      args.LicenseFileInfo = new FileInfo(licensePath);
      args.Product = product;


      args.FilesRoot = System.IO.Path.Combine(Directory.GetParent(args.Product.PackagePath).FullName, System.IO.Path.GetFileNameWithoutExtension(args.Product.PackagePath));

      if (!this.IsFilePathLengthValidInPackage(args.Product.PackagePath, args.FilesRoot))
      {
        return false;
      }

      if (!Directory.Exists(args.FilesRoot))
      {
        Directory.CreateDirectory(args.FilesRoot);
        WindowHelper.LongRunningTask(() => this.UnpackInstallationFiles(args), "Unpacking installation files.", wizardArgs.WizardWindow);
      }
      else
      {
        if (MessageBox.Show(string.Format("Path '{0}' already exists. Do you want to overwrite it?", args.FilesRoot), "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          Directory.Delete(args.FilesRoot, true);
          Directory.CreateDirectory(args.FilesRoot);
          WindowHelper.LongRunningTask(() => this.UnpackInstallationFiles(args), "Unpacking installation files.", wizardArgs.WizardWindow);
        }

      }

      string rootPath = this.LocationText.Text;
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

      args.DestinationFolder = rootPath;
      return true;
    }

    public void UnpackInstallationFiles(InstallContainerWizardArgs args)
    {
      RealZipFile zip = new RealZipFile(new RealFile(new RealFileSystem(), args.Product.PackagePath));
      zip.ExtractTo(new RealFolder(new RealFileSystem(), args.FilesRoot));
    }

    [CanBeNull]
    private string GetValidWebsiteName()
    {
      var instanceName = InstanceName;
      Assert.IsNotNull(instanceName, nameof(instanceName));

      var name = instanceName.Text.EmptyToNull();
      Assert.IsNotNull(name, @"Instance name isn't set");

      return name;
    }

    #endregion

    #region Methods    

    #region Private methods

    private void Init()
    {
      using (new ProfileSection("Initializing InstanceDetails", this))
      {
        DataContext = new Model();
        _Products = ProductManager.ContainerProducts;
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

      productVersion.DataContext = grouping.Where(x => x != null).GroupBy(p => p.ShortVersion).Where(x => x != null).OrderBy(p => Int32.Parse(p.Key));
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

        var name = product.ShortName.Replace(" ", "");
        InstanceName.Text = name;

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

    private bool IsFilePathLengthValidInPackage(string packagePath, string scriptRoot)
    {
      if (File.Exists(packagePath))
      {
        int maxAllowedFilePathLength = MaxFileSystemPathLength - scriptRoot.Length;
        using (ZipArchive zipArchive = ZipFile.OpenRead(packagePath))
        {
          foreach (ZipArchiveEntry entry in zipArchive.Entries)
          {
            if (!(maxAllowedFilePathLength > entry.FullName.Length))
            {
              WindowHelper.ShowMessage("The full path length of some files in the package after unzipping is too long! Please change the path of the Local Repository folder in Settings, so it has less path length.");
              return false;
            }
          }
        }
      }
      else
      {
        WindowHelper.ShowMessage(string.Format("Please make sure that the \"{0}\" package exists.", packagePath));
        return false;
      }

      return true;
    }

    #endregion

    #endregion

    #region Nested type: Model

    public class Model
    {
      #region Fields

      [CanBeNull]
      [UsedImplicitly]
      public readonly Product[] _Products = ProductManager.ContainerProducts.ToArray();

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
      ProductName.DataContext = _Products.GroupBy(p => p.Name);

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

    private void InstanceName_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      if (!NameCharsHelper.IsValidNameChar(e.Text, "project name"))
      {
        e.Handled = true;
      }
    }
  }
}
