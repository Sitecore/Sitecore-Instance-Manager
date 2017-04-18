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
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using JetBrains.Annotations;
  using SIM.Extensions;

  public partial class ModulesDetails : IWizardStep, ICustomButton
  {
    #region Fields

    private bool _IsProductFamiliesChanged;
    private ObservableCollection<ProductInCheckbox> _ProductFamilies = new ObservableCollection<ProductInCheckbox>();
    private ObservableCollection<ProductInCheckbox> _UnfilteredProductFamilies = new ObservableCollection<ProductInCheckbox>();

    #endregion

    #region Constructors

    public ModulesDetails()
    {
      InitializeComponent();
      sitecoreModules.ItemsSource = _ProductFamilies;
    }

    #endregion

    #region Methods

    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      sitecoreModules.SelectedIndex = -1;
    }

    #region Search Implementation

    private void DoSearch(string filter)
    {
      _ProductFamilies = new ObservableCollection<ProductInCheckbox>(_UnfilteredProductFamilies.Where(product => product.Name.ContainsIgnoreCase(filter) || product.Value.SearchToken.ContainsIgnoreCase(filter)));
      sitecoreModules.ItemsSource = _ProductFamilies;
    }

    private void Search(object sender, RoutedEventArgs e)
    {
      DoSearch(SearchTextBox.Text);
    }

    private void SearchTextBoxKeyPressed(object sender, KeyEventArgs e)
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

        if (key == Key.Escape)
        {
          SearchTextBox.Text = string.Empty;
        }

        DoSearch(SearchTextBox.Text);
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError(ex.Message, true, ex);
      }
    }

    #endregion

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (InstallModulesWizardArgs)wizardArgs;

      // create new checkbox list
      Product instanceProduct = args.Product;
      if (instanceProduct == null)
      {
        return;
      }

      var modules = ProductManager.Modules.Where(m => m.IsMatchRequirements(instanceProduct)).OrderByDescending(m => m.SortOrder);
      IEnumerable<string> moduleNames = modules.GroupBy(module => module.Name).Select(moduleGroup => moduleGroup.Key);
      ProductInCheckbox[] productCheckboxes = moduleNames.Select(moduleName => new ProductInCheckbox(moduleName, modules.Where(module => module.Name.EqualsIgnoreCase(moduleName)).OrderByDescending(m => m.VersionAndRevision).ToArray())).ToArray();

      _ProductFamilies = new ObservableCollection<ProductInCheckbox>(productCheckboxes);
      _UnfilteredProductFamilies = new ObservableCollection<ProductInCheckbox>(productCheckboxes);

      foreach (var module in args._Modules)
      {
        Product alreadySelectedModule = module;
        ProductInCheckbox checkBoxItem = productCheckboxes.SingleOrDefault(cbi => cbi.Name.Equals(alreadySelectedModule.Name, StringComparison.OrdinalIgnoreCase));
        if (checkBoxItem != null)
        {
          checkBoxItem.IsChecked = checkBoxItem.Name == module.Name || module.Name.EqualsIgnoreCase("Sitecore Analytics");
          checkBoxItem.Value = module;
        }
      }

      DoSearch(SearchTextBox.Text = string.Empty);
      sitecoreModules.ItemsSource = _ProductFamilies;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (InstallModulesWizardArgs)wizardArgs;
      Product product = args.Product;
      Assert.IsNotNull(product, nameof(product));
      Product[] selectedModules = _UnfilteredProductFamilies.Where(mm => mm.IsChecked).Select(mm => mm.Value).ToArray();
      args._Modules.AddRange(selectedModules.Where(module => !args._Modules.Any(p => p.Name.Equals(module.Name, StringComparison.OrdinalIgnoreCase))));

      return true;
    }

    #endregion

    #region Public properties

    public string CustomButtonText
    {
      get
      {
        return "Add Module";
      }
    }

    #endregion

    #region Public methods

    public void CustomButtonClick()
    {
      var openDialog = new OpenFileDialog
      {
        CheckFileExists = true, 
        Filter = "Sitecore Package or ZIP Archive (*.zip)|*.zip", 
        Title = "Choose Package", 
        Multiselect = true
      };

      if (openDialog.ShowDialog(Window.GetWindow(this)) != true)
      {
        return;
      }

      foreach (var path in openDialog.FileNames)
      {
        var fileName = Path.GetFileName(path);

        if (string.IsNullOrEmpty(fileName))
        {
          continue;
        }

        List<ProductInCheckbox> products = null;
        Product addedProduct;

        Product.TryParse(path, out addedProduct);

        if (string.IsNullOrEmpty(addedProduct?.Name) || string.IsNullOrEmpty(addedProduct.Version) || string.IsNullOrEmpty(addedProduct.Revision))
        {
          var errorMessage =
            $"There was a problem installing a Sitecore module package. The requested package had the following properties: addedProduct is null: [{addedProduct == null}] | addedProduct name: [{addedProduct?.Name}] | addedProduct version: [{addedProduct?.Version}] | addedProduct revision: [{addedProduct?.Revision}]";
          WindowHelper.ShowMessage($"Selected file is not a Sitecore module package");
          Log.Error(errorMessage);
          return;
        }

        var family = GetProductFamily(addedProduct.OriginalName);

        if (family == null)
        {
          FileSystem.FileSystem.Local.File.Copy(path, Path.Combine(ProfileManager.Profile.LocalRepository, fileName));

          Product.TryParse(Path.Combine(ProfileManager.Profile.LocalRepository, fileName), out addedProduct);
          products = _ProductFamilies.ToList();
          products.Add(family = new ProductInCheckbox(addedProduct.Name, new[]
          {
            addedProduct
          }));

          _IsProductFamiliesChanged = true;
        }
        else
        {
          var product = GetProduct(family, addedProduct);

          if (product == null)
          {
            var location = FileSystem.FileSystem.Local.Directory.GetParent(family.Scope.FirstOrDefault().IsNotNull().PackagePath).FullName;
            FileSystem.FileSystem.Local.File.Copy(path, Path.Combine(location, fileName));
            Product.TryParse(Path.Combine(location, fileName), out addedProduct);

            products = _ProductFamilies.Where(item => !item.Name.Equals(family.Name, StringComparison.InvariantCultureIgnoreCase)).ToList();
            products.Insert(_ProductFamilies.IndexOf(family), family = new ProductInCheckbox(family.Name, family.Scope.Add(addedProduct).ToArray()));

            _IsProductFamiliesChanged = true;
          }
          else
          {
            var location = FileSystem.FileSystem.Local.Directory.GetParent(product.PackagePath).FullName;
            FileSystem.FileSystem.Local.File.DeleteIfExists(product.PackagePath);
            FileSystem.FileSystem.Local.File.Copy(path, Path.Combine(location, fileName));
            addedProduct = product;
          }
        }

        ProductManager.Initialize(ProfileManager.Profile.LocalRepository);
        SelectAddedPackage(family, addedProduct);

        if (_IsProductFamiliesChanged)
        {
          RefreshDataSource(products);
        }
      }
    }

    #endregion

    #region Private methods

    private static Product GetProduct(ProductInCheckbox module, Product product)
    {
      return module.Scope.FirstOrDefault(version => version.Version.Equals(product.Version, StringComparison.InvariantCultureIgnoreCase) && version.Revision.Equals(product.Revision, StringComparison.InvariantCultureIgnoreCase));
    }

    private ProductInCheckbox GetProductFamily(string originalName)
    {
      return _ProductFamilies.FirstOrDefault(productInCheckbox => productInCheckbox.Scope.FirstOrDefault().IsNotNull().OriginalName.Equals(originalName, StringComparison.InvariantCultureIgnoreCase));
    }

    private void RefreshDataSource(List<ProductInCheckbox> products)
    {
      _ProductFamilies = new ObservableCollection<ProductInCheckbox>(products);
      _UnfilteredProductFamilies = new ObservableCollection<ProductInCheckbox>(products);
      sitecoreModules.ItemsSource = _ProductFamilies;

      _IsProductFamiliesChanged = false;
    }

    private void SelectAddedPackage(ProductInCheckbox family, Product product)
    {
      family.IsChecked = true;
      family.Value = product;
    }

    #endregion
  }
}