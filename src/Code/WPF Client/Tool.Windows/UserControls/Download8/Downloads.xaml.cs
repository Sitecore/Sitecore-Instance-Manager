#region Usings



#endregion

namespace SIM.Tool.Windows.UserControls.Download8
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using SIM.Base;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;

  #region



  #endregion

  /// <summary>
  ///   Interaction logic for FilePackages.xaml
  /// </summary>
  public partial class Downloads : IWizardStep, ICustomButton, IFlowControl
  {
    #region Fields

    /// <summary>
    ///   The check box items.
    /// </summary>
    private readonly List<ProductDownload8InCheckbox> checkBoxItems = new List<ProductDownload8InCheckbox>();

    #endregion

    #region Constructors

    public Downloads()
    {
      this.InitializeComponent();
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
        return "Open Folder";
      }
    }

    #endregion

    #endregion

    #region Public Methods

    #region ICustomButton Members

    public void CustomButtonClick()
    {
      WindowHelper.OpenFolder(ProfileManager.Profile.LocalRepository);
    }

    #endregion

    #region IFlowControl Members

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      bool canMoveNext = args.Products.Count > 0;
      if(!canMoveNext)
      {
        WindowHelper.HandleError("You didn't select any download, please select one to go further", false);
      }

      WindowHelper.LongRunningTask(() => this.PrepareData(args), "Sitecore Versions Downloader", Window.GetWindow(this), "Preparing for downloading");

      return canMoveNext;
    }

    
    private void PrepareData(DownloadWizardArgs args)
    {
      try
      {
        var links = this.GetLinks(args);
        args.Links = links;
      }
      catch (Exception ex)
      {
        Log.Error("Error while preparing data", this, ex);
      }
    }

    private ReadOnlyCollection<Uri> GetLinks(DownloadWizardArgs args)
    {
      return new ReadOnlyCollection<Uri>(args.Products.SelectMany(product => product.Value).ToArray());
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
      var args = (DownloadWizardArgs)wizardArgs;
      var selected = this.checkBoxItems.Where(mm => mm.IsChecked);
      args.Products.AddRange(selected);

      return true;
    }

    public WizardArgs WizardArgs
    {
      get;
      set;
    }

    public static WebBrowser WebBrowser
    {
      get;
      private set;
    }

    #endregion

    #endregion

    #region Methods

    public void InitializeStep(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      this.checkBoxItems.Clear();
      this.Append(args.Records);

      foreach (var product in args.Products)
      {
        var selectedPRoduct = product;
        ProductDownload8InCheckbox checkBoxItem = this.checkBoxItems.SingleOrDefault(cbi => cbi.Value.Equals(selectedPRoduct));
        if (checkBoxItem != null)
        {
          checkBoxItem.IsChecked = true;
        }
      }

      this.filePackages.DataContext = this.checkBoxItems;
    }

    private void Append(IEnumerable<string> records)
    {
      this.checkBoxItems.AddRange(records.Select(f => new ProductDownload8InCheckbox(f)).ToList());
    }

    /// <summary>
    /// Products the selected.
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

    #endregion
  }
}