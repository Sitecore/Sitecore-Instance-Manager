namespace SIM.Tool.Windows.UserControls.Download
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Pipelines.Download;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.Diagnostics.InformationService.Client.Model;
  using SIM.Core;

  #region

  #endregion

  public partial class Downloads : IWizardStep, ICustomButton, IFlowControl
  {
    #region Fields

    private readonly List<ProductDownloadInCheckbox> checkBoxItems = new List<ProductDownloadInCheckbox>();

    #endregion

    #region Constructors

    public Downloads()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Properties

    public string CustomButtonText
    {
      get
      {
        return "Open Folder";
      }
    }

    #endregion

    #region Public Methods

    #region ICustomButton Members

    public void CustomButtonClick()
    {
      CoreApp.OpenFolder(ProfileManager.Profile.LocalRepository);
    }

    #endregion

    #region IFlowControl Members

    #region Public methods

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      bool canMoveNext = args.Products.Count > 0;
      if (!canMoveNext)
      {
        WindowHelper.HandleError("You didn't select any download, please select one to go further", false);
      }

      WindowHelper.LongRunningTask(() => this.PrepareData(args), "Sitecore Versions Downloader", Window.GetWindow(this), "Preparing for downloading");

      return canMoveNext;
    }

    #endregion

    #region Private methods

    private void CheckFileSize(UriBasedCollection<long> fileSizes, Uri url, string cookies)
    {
      Assert.IsNotNull(url, "url");

      try
      {
        using (var response = WebRequestHelper.RequestAndGetResponse(url, 60000, 60000, cookies))
        {
          var fileSize = response.ContentLength;
          fileSizes[url] = fileSize;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error while downloading {0}", url.ToString());
      }
    }


    private ReadOnlyCollection<Uri> GetLinks(DownloadWizardArgs args)
    {
      return new ReadOnlyCollection<Uri>(args.Products.SelectMany(product => product.Value).ToArray());
    }

    private UriBasedCollection<long> GetSizes(ReadOnlyCollection<Uri> urls, string cookies)
    {
      UriBasedCollection<long> sizes = new UriBasedCollection<long>();
      int parallelDownloadsNumber = WindowsSettings.AppDownloaderParallelThreads.Value;

      for (int i = 0; i < urls.Count; i += parallelDownloadsNumber)
      {
        int remains = urls.Count - i;
        var tasks = urls
          .Skip(i)
          .Take(Math.Min(parallelDownloadsNumber, remains))
          .Select(url => Task.Factory.StartNew(() => this.CheckFileSize(sizes, url, cookies)))
          .ToArray();

        Task.WaitAll(tasks, remains * 60000);
      }

      return sizes;
    }

    private void PrepareData(DownloadWizardArgs args)
    {
      try
      {
        var links = this.GetLinks(args);
        args.Links = links;
        var sizes = this.GetSizes(links, args.Cookies);
        Assert.IsTrue(sizes.Count == args.Links.Count, "The length of the sizes array differs from links count");
        Assert.IsTrue(sizes.All(s => s.Value > 0), "Some SDN packages are said to have 0 length");
        args.Sizes = sizes;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error while preparing data");
      }
    }

    #endregion

    #endregion

    #region IStateControl Members

    #region Public properties

    public static WebBrowser WebBrowser { get; private set; }
    public WizardArgs WizardArgs { get; set; }

    #endregion

    #region Public methods

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      var selected = this.checkBoxItems.Where(mm => mm.IsChecked);
      args.Products.AddRange(selected);

      return true;
    }

    #endregion

    #endregion

    #endregion

    #region Methods

    #region Public methods

    public void InitializeStep(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      this.checkBoxItems.Clear();
      this.Append(args.Releases);

      foreach (var product in args.Products)
      {
        var selectedPRoduct = product;
        ProductDownloadInCheckbox checkBoxItem = this.checkBoxItems.SingleOrDefault(cbi => cbi.Value.Equals(selectedPRoduct));
        if (checkBoxItem != null)
        {
          checkBoxItem.IsChecked = true;
        }
      }

      this.filePackages.DataContext = this.checkBoxItems;
    }

    #endregion

    #region Private methods

    private void Append(IEnumerable<IRelease> records)
    {
      this.checkBoxItems.AddRange(records.Select(r => new ProductDownloadInCheckbox(r)).ToList());
    }

    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      this.filePackages.SelectedIndex = -1;
    }

    private void UserControlLoaded(object sender, RoutedEventArgs e)
    {
    }

    #endregion

    #endregion
  }
}