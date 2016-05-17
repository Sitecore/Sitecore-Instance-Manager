namespace SIM.Tool.Windows.UserControls.Install.Modules
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Microsoft.Win32;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  public partial class FilePackages : IWizardStep, ICustomButton, IFlowControl
  {
    #region Fields

    private ObservableCollection<ProductInCheckbox> checkBoxItems = new ObservableCollection<ProductInCheckbox>();
    private ObservableCollection<ProductInCheckbox> unfilteredCheckBoxItems = new ObservableCollection<ProductInCheckbox>();

    #endregion

    #region Constructors

    public FilePackages()
    {
      this.InitializeComponent();
      this.filePackages.ItemsSource = this.checkBoxItems;
    }

    #endregion

    #region Properties

    public string CustomButtonText
    {
      get
      {
        return "Add Package";
      }
    }

    #endregion

    #region Public Methods

    #region ICustomButton Members

    public void CustomButtonClick()
    {
      var openDialog = new OpenFileDialog
      {
        CheckFileExists = true, 
        Filter = "Sitecore Package or ZIP Archive (*.zip)|*.zip", 
        Title = "Choose Package", 
        Multiselect = true
      };
      if (openDialog.ShowDialog(Window.GetWindow(this)) == true)
      {
        foreach (string path in openDialog.FileNames)
        {
          var fileName = Path.GetFileName(path);

          if (string.IsNullOrEmpty(fileName))
          {
            continue;
          }

          FileSystem.FileSystem.Local.File.Copy(path, Path.Combine(ApplicationManager.FilePackagesFolder, fileName));

          var products = this.checkBoxItems.Where(item => !item.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)).ToList();
          products.Add(new ProductInCheckbox(Product.GetFilePackageProduct(Path.Combine(ApplicationManager.FilePackagesFolder, fileName))));
          this.checkBoxItems = new ObservableCollection<ProductInCheckbox>(products);
          this.unfilteredCheckBoxItems = new ObservableCollection<ProductInCheckbox>(products);
          this.filePackages.ItemsSource = this.checkBoxItems;

          this.SelectAddedPackage(fileName);
        }
      }
    }

    #endregion

    #region IFlowControl Members

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion

    #region IStateControl Members

    #region Public properties

    public WizardArgs WizardArgs { get; set; }

    #endregion

    #region Public methods

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (InstallModulesWizardArgs)wizardArgs;
      Product product = args.Product;
      Assert.IsNotNull(product, "product");
      IEnumerable<Product> selected = this.unfilteredCheckBoxItems.Where(mm => mm.IsChecked).Select(s => s.Value);

      foreach (ProductInCheckbox boxItem in this.unfilteredCheckBoxItems)
      {
        int moduleIndex = args.Modules.FindIndex(m => string.Equals(m.Name, boxItem.Value.Name, StringComparison.OrdinalIgnoreCase));
        while (moduleIndex >= 0)
        {
          args.Modules.RemoveAt(moduleIndex);
          moduleIndex = args.Modules.FindIndex(m => string.Equals(m.Name, boxItem.Value.Name, StringComparison.OrdinalIgnoreCase));
        }
      }

      args.Modules.AddRange(selected);

      return true;
    }

    #endregion

    #endregion

    #endregion

    #region Methods

    #region Public methods

    public void InitializeStep(WizardArgs wizardArgs)
    {
      this.WizardArgs = wizardArgs;
      var args = (InstallModulesWizardArgs)wizardArgs;
      this.checkBoxItems.Clear();
      foreach (var folder in EnvironmentHelper.FilePackageFolders)
      {
        this.Append(folder);
      }

      this.unfilteredCheckBoxItems = new ObservableCollection<ProductInCheckbox>(this.checkBoxItems);

      foreach (Product module in args.Modules)
      {
        Product alreadySelectedModule = module;
        ProductInCheckbox checkBoxItem = this.checkBoxItems.SingleOrDefault(cbi => cbi.Value.PackagePath.Equals(alreadySelectedModule.PackagePath, StringComparison.OrdinalIgnoreCase));
        if (checkBoxItem != null)
        {
          checkBoxItem.IsChecked = true;
        }
      }

      if (args is InstallWizardArgs)
      {
        foreach (var cbi in this.checkBoxItems.NotNull())
        {
          if ((WindowsSettings.AppInstallDefaultCustomPackages.Value ?? string.Empty).Split('|').Any(s => cbi.Name.EqualsIgnoreCase(s)))
          {
            cbi.IsChecked = true;
          }
        }
      }

      this.DoSearch(this.SearchTextBox.Text = string.Empty);
    }

    #endregion

    #region Private methods

    private void Append(string folder)
    {
      if (!Directory.Exists(folder))
      {
        return;
      }
      
      var files = FileSystem.FileSystem.Local.Directory.GetFiles(folder, "*.zip", SearchOption.AllDirectories);
      var productsToAdd = files.Select(f => new ProductInCheckbox(Product.GetFilePackageProduct(f))).ToList();
      foreach (ProductInCheckbox productInCheckbox in productsToAdd)
      {
        this.checkBoxItems.Add(productInCheckbox);
      }
    }

    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      this.filePackages.SelectedIndex = -1;
    }

    private void SelectAddedPackage(string packageName)
    {
      var package = this.checkBoxItems.FirstOrDefault(item => item.Name.EqualsIgnoreCase(packageName));
      if (package != null)
      {
        package.IsChecked = true;
      }
    }

    private void UserControlLoaded(object sender, RoutedEventArgs e)
    {
    }

    #endregion

    #region Search Implementation

    private void DoSearch(string filter)
    {
      this.checkBoxItems = new ObservableCollection<ProductInCheckbox>(this.unfilteredCheckBoxItems.Where(product => product.Name.ContainsIgnoreCase(filter) || product.Value.SearchToken.ContainsIgnoreCase(filter)));
      this.filePackages.ItemsSource = this.checkBoxItems;
    }

    private void Search(object sender, RoutedEventArgs e)
    {
      this.DoSearch(this.SearchTextBox.Text);
    }

    private void SearchTextBoxKeyPressed(object sender, KeyEventArgs e)
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

        if (key == Key.Escape)
        {
          this.SearchTextBox.Text = string.Empty;
        }

        this.DoSearch(this.SearchTextBox.Text);
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError(ex.Message, true, ex);
      }
    }

    #endregion

    #endregion
  }
}