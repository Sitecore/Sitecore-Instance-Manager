namespace SIM.Tool.Windows.UserControls.Download
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using SIM.Extensions;

  #region

  #endregion

  public class ProductDownloadInCheckbox : DataObjectBase
  {
    #region Fields

    private bool isEnabled { get; }
    private string label { get; }
    private string name { get; }
    private string nameOverride { get; }
    private string revision { get; }
    private string version { get; }
    private bool isChecked;

    private ReadOnlyCollection<Uri> value;

    #endregion

    #region Constructors

    public ProductDownloadInCheckbox([NotNull] IRelease release)
    {
      Assert.ArgumentNotNull(release, nameof(release));

      this.name = "Sitecore CMS";
      this.version = release.Version.MajorMinor;
      this.revision = release.Version.Revision.ToString();
      this.label = release.Label;
      var distribution = release.DefaultDistribution;
      Assert.IsNotNull(distribution, nameof(distribution));

      this.value = new ReadOnlyCollection<Uri>(distribution.Downloads.Where(x => x.StartsWith("http")).Select(x => new Uri(x)).ToArray());
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
      return $"{this.nameOverride ?? this.Name} {this.Version} rev. {this.Revision}{(string.IsNullOrEmpty(this.Label) ? string.Empty : " (" + this.Label + ")")}{(this.IsEnabled ? string.Empty : " - you already have it")}";
    }

    #endregion

    #region Private methods

    private bool CheckAnalyticsProduct(Products.Product product)
    {
      return product.Name.Equals("Sitecore Analytics")
             && product.Revision == this.revision;
    }

    private bool CheckProduct(Products.Product product)
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