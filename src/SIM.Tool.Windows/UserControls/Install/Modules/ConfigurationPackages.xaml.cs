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
  using JetBrains.Annotations;
  using SIM.Extensions;

  public partial class ConfigurationPackages : IWizardStep, ICustomButton, IFlowControl
  {
    #region Fields

    private ObservableCollection<ProductInCheckbox> _CheckBoxItems = new ObservableCollection<ProductInCheckbox>();
    private ObservableCollection<ProductInCheckbox> _UnfilteredCheckBoxItems = new ObservableCollection<ProductInCheckbox>();

    #endregion

    #region Constructors

    public ConfigurationPackages()
    {
      InitializeComponent();
      filePackages.ItemsSource = _CheckBoxItems;
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

          FileSystem.FileSystem.Local.File.Copy(path, Path.Combine(ApplicationManager.ConfigurationPackagesFolder, fileName));

          var products = _CheckBoxItems.Where(item => !item.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)).ToList();
          products.Add(new ProductInCheckbox(Product.GetFilePackageProduct(Path.Combine(ApplicationManager.ConfigurationPackagesFolder, fileName))));
          _CheckBoxItems = new ObservableCollection<ProductInCheckbox>(products);
          _UnfilteredCheckBoxItems = new ObservableCollection<ProductInCheckbox>(products);
          filePackages.ItemsSource = _CheckBoxItems;

          SelectAddedPackage(fileName);
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
      Assert.IsNotNull(product, nameof(product));
      IEnumerable<Product> selected = _UnfilteredCheckBoxItems.Where(mm => mm.IsChecked).Select(s => s.Value);

      foreach (var boxItem in _UnfilteredCheckBoxItems)
      {
        var moduleIndex = args._Modules.FindIndex(m => string.Equals(m.Name, boxItem.Value.Name, StringComparison.OrdinalIgnoreCase));
        while (moduleIndex >= 0)
        {
          args._Modules.RemoveAt(moduleIndex);
          moduleIndex = args._Modules.FindIndex(m => string.Equals(m.Name, boxItem.Value.Name, StringComparison.OrdinalIgnoreCase));
        }
      }

      args._Modules.AddRange(selected);

      return true;
    }

    #endregion

    #endregion

    #endregion

    #region Methods

    #region Public methods

    public void InitializeStep(WizardArgs wizardArgs)
    {
      WizardArgs = wizardArgs;
      var args = (InstallModulesWizardArgs)wizardArgs;
      _CheckBoxItems.Clear();
      foreach (var folder in EnvironmentHelper.ConfigurationPackageFolders)
      {
        Append(folder, args.Product);
      }

      _UnfilteredCheckBoxItems = new ObservableCollection<ProductInCheckbox>(_CheckBoxItems);

      foreach (var module in args._Modules)
      {
        Product alreadySelectedModule = module;
        ProductInCheckbox checkBoxItem = _CheckBoxItems.SingleOrDefault(cbi => cbi.Value.PackagePath.Equals(alreadySelectedModule.PackagePath, StringComparison.OrdinalIgnoreCase));
        if (checkBoxItem != null)
        {
          checkBoxItem.IsChecked = true;
        }
      }

      if (args is InstallWizardArgs)
      {
        foreach (var cbi in _CheckBoxItems.NotNull())
        {
          if ((WindowsSettings.AppInstallDefaultCustomPackages.Value ?? string.Empty).Split('|').Any(s => cbi.Name.EqualsIgnoreCase(s)))
          {
            cbi.IsChecked = true;
          }
        }
      }

      DoSearch(SearchTextBox.Text = string.Empty);
    }

    #endregion

    #region Private methods

    private void Append(string folder, Product product)
    {
      var ver = product.TwoVersion;
      IEnumerable<string> files = FileSystem.FileSystem.Local.Directory.GetFiles(folder, "*.zip", SearchOption.AllDirectories)
        .Where(f => false 
          || !f.ContainsIgnoreCase("- Sitecore") 
          || f.ContainsIgnoreCase($"- Sitecore {ver}") 
          || f.ContainsIgnoreCase($"- Sitecore {ver.Substring(0, 3)} ") 
          || f.ContainsIgnoreCase($"- Sitecore {ver[0]} ")
          );

      var productsToAdd = files.Select(f => new ProductInCheckbox(Product.GetFilePackageProduct(f))).ToList();
      foreach (var productInCheckbox in productsToAdd)
      {
        _CheckBoxItems.Add(productInCheckbox);
      }
    }

    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      filePackages.SelectedIndex = -1;
    }

    private void SelectAddedPackage(string packageName)
    {
      var package = _CheckBoxItems.FirstOrDefault(item => item.Name.EqualsIgnoreCase(packageName));
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
      _CheckBoxItems = new ObservableCollection<ProductInCheckbox>(_UnfilteredCheckBoxItems.Where(product => product.Name.ContainsIgnoreCase(filter) || product.Value.SearchToken.ContainsIgnoreCase(filter)));
      filePackages.ItemsSource = _CheckBoxItems;
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
        WindowHelper.HandleError("Failed to search", true, ex);
      }
    }

    #endregion

    #endregion
  }
}