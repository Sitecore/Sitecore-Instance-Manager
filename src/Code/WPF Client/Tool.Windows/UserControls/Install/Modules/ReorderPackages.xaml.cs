#region Usings

using Microsoft.Win32;
using SIM.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Product = SIM.Products.Product;

#endregion

namespace SIM.Tool.Windows.UserControls.Install.Modules
{
  using System;
  using System.Globalization;
  using System.Linq;
  using System.Windows.Data;
  using System.Windows.Shapes;
  using Base;
  using Path = System.IO.Path;

  internal class ReadmeToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return string.IsNullOrEmpty(((Dictionary<bool, string>) value).Values.FirstOrDefault()) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

  internal class ReadmeToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (((Dictionary<bool, string>)value).Values.FirstOrDefault());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

  internal class ReadmeToColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((Dictionary<bool, string>)value).Keys.FirstOrDefault() ? "red" : "gray";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

  /// <summary>
  ///   Interaction logic for FilePackages.xaml
  /// </summary>
  public partial class ReorderPackages : IWizardStep, IFlowControl, ICustomButton
  {
    #region Fields

    /// <summary>
    ///   The check box items.
    /// </summary>
    private readonly ObservableCollection<Product> actualProducts = new ObservableCollection<Product>();

    #endregion

    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="FilePackages" /> class.
    /// </summary>
    public ReorderPackages()
    {
      this.InitializeComponent();
    }

    #endregion
    
    #region Public Methods
    
    #region IStateControl Members

    /// <summary>
    ///   Saves the changes.
    /// </summary>
    /// <returns> The changes. </returns>
    public bool SaveChanges(WizardArgs wizardArgs)
    {
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
      this.actualProducts.Clear();      
      args.Modules.ForEach(module => this.actualProducts.Add(module));
      
      this.modulesList.ItemsSource = this.actualProducts;
      this.selectedProductLabel.DataContext = args.Product;
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
      this.modulesList.SelectedIndex = -1;
    }
    
    #region Drag-n-Drop Implementation

    protected void ModulesListPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
      if (sender is ListBoxItem)
      {
        ListBoxItem draggedItem = sender as ListBoxItem;
        DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
        draggedItem.IsSelected = true;
      }
    }

    protected void ModulesListDrop(object sender, DragEventArgs e)
    {
      Product droppedData = e.Data.GetData(typeof(Product)) as Product;
      Product target = ((ListBoxItem)(sender)).DataContext as Product;

      Assert.IsNotNull(droppedData, "[ReorderProducts] Drag-n-drop: droppedData is null");
      Assert.IsNotNull(target, "[ReorderProducts] Drag-n-drop: target is null");
      int removeIndex = this.actualProducts.IndexOf(droppedData);
      int insertIndex = this.actualProducts.IndexOf(target);

      if (removeIndex < insertIndex)
      {
        this.actualProducts.Insert(insertIndex + 1, droppedData);
        this.actualProducts.RemoveAt(removeIndex);
      }
      else
      {
        int removeIndexNext = removeIndex + 1;
        if (this.modulesList.Items.Count + 1 > removeIndexNext)
        {
          this.actualProducts.Insert(insertIndex, droppedData);
          this.actualProducts.RemoveAt(removeIndexNext);
        }
      }
    }

    #endregion

    #endregion

    #region ICustomButton Implementation

    public string CustomButtonText
    {
      get
      {
        return "Add Package";
      }
    }

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

          Product product = Product.GetFilePackageProduct(path);
          if (!this.actualProducts.Any(p => p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase)))
          {
            this.actualProducts.Add(product);
          }
        }
      }
    }

    #endregion

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (InstallModulesWizardArgs)wizardArgs;
      Product product = args.Product;
      Assert.IsNotNull(product, "product");
      IEnumerable<Product> selected = this.actualProducts;
      args.Modules.Clear();
      args.Modules.AddRange(selected);

      if (!(args is InstallWizardArgs) && !args.Modules.Any())
      {
        WindowHelper.HandleError("You haven't chosen any module to install", false);
        return false;
      }
      
      return true;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
    {
      ((Rectangle)sender).Cursor = Cursors.Hand;
    }
  }
}