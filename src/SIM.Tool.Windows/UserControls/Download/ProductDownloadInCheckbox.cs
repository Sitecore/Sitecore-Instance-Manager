namespace SIM.Tool.Windows.UserControls.Download
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.InformationService.Client.Model;

  #region

  #endregion

  public class ProductDownloadInCheckbox : DataObjectBase
  {
    #region Fields

    private readonly bool isEnabled;
    private readonly string label;
    private readonly string name;
    private readonly string nameOverride;
    private readonly string revision;
    private readonly string version;
    private bool isChecked;

    private ReadOnlyCollection<Uri> value;

    #endregion

    #region Constructors

    public ProductDownloadInCheckbox([NotNull] IRelease release)
    {
      Assert.ArgumentNotNull(release, "release");

      this.name = "Sitecore CMS";
      this.version = release.Version;
      this.revision = release.Revision;
      this.label = release.Label;
      this.value = new ReadOnlyCollection<Uri>(release.Downloads.Where(x => x.StartsWith("http")).Select(x => new Uri(x)).ToArray());
      this.isEnabled = !ProductManager.Products.Any(this.CheckProduct);
      if (!this.isEnabled && this.name.EqualsIgnoreCase("Sitecore CMS") && !ProductManager.Products.Any(this.CheckAnalyticsProduct) && this.value.Count > 1)
      {
        this.isEnabled = true;
        this.nameOverride = "Sitecore Analytics";
      }
    }

    #endregion

    #region Public methods

    public override string ToString()
    {
      return string.Format("{0} {1} rev. {2}{3}{4}", this.nameOverride ?? this.Name, this.Version, this.Revision, string.IsNullOrEmpty(this.Label) ? string.Empty : " (" + this.Label + ")", this.IsEnabled ? string.Empty : " - you already have it");
    }

    #endregion

    #region Private methods

    private bool CheckAnalyticsProduct(Product product)
    {
      return product.Name.Equals("Sitecore Analytics")
             && product.Revision == this.revision;
    }

    private bool CheckProduct(Product product)
    {
      return (product.Name.EqualsIgnoreCase(this.name) || product.OriginalName.EqualsIgnoreCase(this.name))
             && product.Version == this.version
             && product.Revision == this.revision;
    }

    #endregion

    #region Properties

    #region Public properties

    public bool IsChecked
    {
      get
      {
        return this.isChecked;
      }

      set
      {
        this.isChecked = value;
        this.NotifyPropertyChanged("IsChecked");
      }
    }

    public bool IsEnabled
    {
      get
      {
        return this.isEnabled;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public ReadOnlyCollection<Uri> Value
    {
      get
      {
        return this.value;
      }

      set
      {
        this.value = value;
        this.NotifyPropertyChanged("Value");
      }
    }

    #endregion

    #region Protected properties

    protected string Label
    {
      get
      {
        return this.label;
      }
    }

    protected string Revision
    {
      get
      {
        return this.revision;
      }
    }

    protected string Version
    {
      get
      {
        return this.version;
      }
    }

    #endregion

    #endregion
  }
}