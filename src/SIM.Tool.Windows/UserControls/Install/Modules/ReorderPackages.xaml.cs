namespace SIM.Tool.Windows.UserControls.Install.Modules
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Shapes;
  using Microsoft.Win32;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  internal class ReadmeToVisibilityConverter : IValueConverter
  {
    #region Public methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return string.IsNullOrEmpty(((Dictionary<bool, string>)value).Values.FirstOrDefault()) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  internal class ReadmeToStringConverter : IValueConverter
  {
    #region Public methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((Dictionary<bool, string>)value).Values.FirstOrDefault();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  internal class ReadmeToColorConverter : IValueConverter
  {
    #region Public methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((Dictionary<bool, string>)value).Keys.FirstOrDefault() ? "red" : "gray";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  public partial class ReorderPackages : IWizardStep, IFlowControl, ICustomButton
  {
    #region Fields

    private readonly ObservableCollection<Product> actualProducts = new ObservableCollection<Product>();

    #endregion

    #region Constructors

    public ReorderPackages()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Public Methods

    #region IStateControl Members

    #region Public properties

    public WizardArgs WizardArgs { get; set; }

    #endregion

    #region Public methods

    public bool SaveChanges(WizardArgs wizardArgs)
    {
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
      this.actualProducts.Clear();
      args.Modules.ForEach(module => this.actualProducts.Add(module));

      this.modulesList.ItemsSource = this.actualProducts;
      this.selectedProductLabel.DataContext = args.Product;
    }

    #endregion

    #region Private methods

    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      this.modulesList.SelectedIndex = -1;
    }

    #endregion

    #region Drag-n-Drop Implementation

    protected void ModulesListDrop(object sender, DragEventArgs e)
    {
      Product droppedData = e.Data.GetData(typeof(Product)) as Product;
      Product target = ((ListBoxItem)sender).DataContext as Product;

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

    protected void ModulesListPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
      if (sender is ListBoxItem)
      {
        ListBoxItem draggedItem = sender as ListBoxItem;
        DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
        draggedItem.IsSelected = true;
      }
    }

    #endregion

    #endregion

    #region ICustomButton Implementation

    #region Public properties

    public string CustomButtonText
    {
      get
      {
        return "Add Package";
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
      if (openDialog.ShowDialog(Window.GetWindow(this)) == true)
      {
        foreach (string path in openDialog.FileNames)
        {
          var fileName = System.IO.Path.GetFileName(path);

          if (string.IsNullOrEmpty(fileName))
          {
            continue;
          }

          Product product = Product.GetFilePackageProduct(path);
          if (!this.actualProducts.Any(p => p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase)))
          {
            this.actualProducts.Add(product);
          }
        }
      }
    }

    #endregion

    #endregion

    #region Public methods

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

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

    #endregion

    #region Private methods

    private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
    {
      ((Rectangle)sender).Cursor = Cursors.Hand;
    }

    #endregion
  }
}