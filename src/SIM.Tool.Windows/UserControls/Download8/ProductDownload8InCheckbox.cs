namespace SIM.Tool.Windows.UserControls.Download8
{
  using System;
  using System.Linq;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.InformationService.Client.Model;

  public class ProductDownload8InCheckbox : DataObjectBase
  {
    #region Fields

    private readonly bool isEnabled;
    private readonly string label;
    private readonly string name;
    private readonly string nameOverride;
    private readonly string revision;
    private readonly string version;
    private bool isChecked;

    private Uri value;

    #endregion

    #region Constructors

    public ProductDownload8InCheckbox([NotNull] IRelease release)
    {
      Assert.ArgumentNotNull(release, "release");

      this.name = "Sitecore CMS";
      this.version = release.Version;
      this.revision = release.Revision;
      this.label = release.Label;
      this.value = new Uri(release.Downloads.First(x => x.StartsWith("http")));
      this.isEnabled = !ProductManager.Products.Any(this.CheckProduct);
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

    private bool CheckProduct([CanBeNull] Product product)
    {
      if (product == null)
      {
        return false;
      }

      if (!product.Name.EqualsIgnoreCase(this.name) && !product.OriginalName.EqualsIgnoreCase(this.name))
      {
        return false;
      }

      if (product.Version != this.version)
      {
        return false;
      }

      return product.Revision == this.revision;
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

    public Uri Value
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