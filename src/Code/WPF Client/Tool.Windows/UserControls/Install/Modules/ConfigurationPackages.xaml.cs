#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SIM.Base;
using SIM.Products;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using System.Windows.Input;

#endregion

namespace SIM.Tool.Windows.UserControls.Install.Modules
{
  /// <summary>
  ///   Interaction logic for FilePackages.xaml
  /// </summary>
  public partial class ConfigurationPackages : IWizardStep, ICustomButton, IFlowControl
  {
    #region Fields

    /// <summary>
    ///   The check box items.
    /// </summary>
    private ObservableCollection<ProductInCheckbox> checkBoxItems = new ObservableCollection<ProductInCheckbox>();
    private ObservableCollection<ProductInCheckbox> unfilteredCheckBoxItems = new ObservableCollection<ProductInCheckbox>();

    #endregion

    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="FilePackages" /> class.
    /// </summary>
    public ConfigurationPackages()
    {
      this.InitializeComponent();
      this.filePackages.ItemsSource = this.checkBoxItems;
    }

    #endregion

    #region Properties

    #region Public properties

    /// <summary>
    ///   Gets the custom button text.
    /// </summary>
    public string CustomButtonText
    {
      get
      {
        return "Add Package";
      }
    }

    #endregion

    #endregion

    #region Public Methods

    #region ICustomButton Members

    /// <summary>
    /// Customs the button click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="routedEventArgs">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
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

          if (string.IsNullOrEmpty(fileName)) continue;

          FileSystem.Local.File.Copy(path, Path.Combine(ApplicationManager.ConfigurationPackagesFolder, fileName));

          var products = this.checkBoxItems.Where(item => !item.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)).ToList();
          products.Add(new ProductInCheckbox(Product.GetFilePackageProduct(Path.Combine(ApplicationManager.ConfigurationPackagesFolder, fileName))));
          this.checkBoxItems = new ObservableCollection<ProductInCheckbox>(products);
          this.unfilteredCheckBoxItems = new ObservableCollection<ProductInCheckbox>(products);
          this.filePackages.ItemsSource = this.checkBoxItems;

          SelectAddedPackage(fileName);
        }
      }
    }

    #endregion

    #region IFlowControl Members

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion

    #region IStateControl Members

    /// <summary>
    ///   Saves the changes.
    /// </summary>
    /// <returns> The changes. </returns>
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

    public WizardArgs WizardArgs { get; set; }

    #endregion

    #endregion

    #region Methods

    public void InitializeStep(WizardArgs wizardArgs)
    {
      this.WizardArgs = wizardArgs;
      var args = (InstallModulesWizardArgs)wizardArgs;
      this.checkBoxItems.Clear();      
      foreach (var folder in EnvironmentHelper.ConfigurationPackageFolders)
      {
        this.Append(folder, args.Product);
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

      DoSearch(this.SearchTextBox.Text = string.Empty);
    }

    /// <summary>
    /// The append.
    /// </summary>
    /// <param name="folder">
    /// The file packages. 
    /// </param>
    private void Append(string folder, Product product)
    {
      var ver = product.Version;
      IEnumerable<string> files = FileSystem.Local.Directory.GetFiles(folder, "*.zip", SearchOption.AllDirectories).Where(f => !f.ContainsIgnoreCase("- Sitecore") || (f.ContainsIgnoreCase("- Sitecore " + ver) || (f.ContainsIgnoreCase("- Sitecore " + ver.Substring(0, 3) + " ") || (f.ContainsIgnoreCase("- Sitecore " + ver[0] + " ")))));

      var productsToAdd = files.Select(f => new ProductInCheckbox(Product.GetFilePackageProduct(f))).ToList();
      foreach (ProductInCheckbox productInCheckbox in productsToAdd)
      {
        this.checkBoxItems.Add(productInCheckbox);
      }
    }

    /// <summary>
    /// Modules the selected.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data. 
    /// </param>
    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      this.filePackages.SelectedIndex = -1;
    }

    /// <summary>
    /// Users the control loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void UserControlLoaded(object sender, RoutedEventArgs e)
    {
    }

    private void SelectAddedPackage(string packageName)
    {
      var package = checkBoxItems.FirstOrDefault(item => item.Name.EqualsIgnoreCase(packageName));
      if (package != null) package.IsChecked = true;
    }

    #region Search Implementation

    private void SearchTextBoxKeyPressed(object sender, KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, "e");

        if (e.Handled)
          return;
        e.Handled = true;
        Key key = e.Key;

        if (key == Key.Escape)
        {
          this.SearchTextBox.Text = string.Empty;
        }

        DoSearch(this.SearchTextBox.Text);
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError(ex.Message, true, ex, this);
      }
    }

    private void Search(object sender, RoutedEventArgs e)
    {
      DoSearch(this.SearchTextBox.Text);
    }

    private void DoSearch(string filter)
    {
      this.checkBoxItems = new ObservableCollection<ProductInCheckbox>(this.unfilteredCheckBoxItems.Where(product => product.Name.ContainsIgnoreCase(filter) || product.Value.SearchToken.ContainsIgnoreCase(filter)));
      this.filePackages.ItemsSource = this.checkBoxItems;
    }

    #endregion

    #endregion
  }
}