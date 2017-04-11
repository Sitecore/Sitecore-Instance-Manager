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
    private string NameOverride { get; }
    private string revision { get; }
    private string version { get; }
    private bool _IsChecked;

    private ReadOnlyCollection<Uri> _Value;

    #endregion

    #region Constructors

    public ProductDownloadInCheckbox([NotNull] IRelease release)
    {
      Assert.ArgumentNotNull(release, nameof(release));

      name = "Sitecore CMS";
      version = release.Version.MajorMinor;
      revision = release.Version.Revision.ToString();
      label = release.Label;
      var distribution = release.DefaultDistribution;
      Assert.IsNotNull(distribution, nameof(distribution));

      _Value = new ReadOnlyCollection<Uri>(distribution.Downloads.Where(x => x.StartsWith("http")).Select(x => new Uri(x)).ToArray());
      isEnabled = !ProductManager.Products.Any(CheckProduct);
      if (!isEnabled && name.EqualsIgnoreCase("Sitecore CMS") && !ProductManager.Products.Any(CheckAnalyticsProduct) && _Value.Count > 1)
      {
        isEnabled = true;
        NameOverride = "Sitecore Analytics";
      }
    }

    #endregion

    #region Public methods

    public override string ToString()
    {
      return $"{NameOverride ?? Name} {Version} rev. {Revision}{(string.IsNullOrEmpty(Label) ? string.Empty : " (" + Label + ")")}{(IsEnabled ? string.Empty : " - you already have it")}";
    }

    #endregion

    #region Private methods

    private bool CheckAnalyticsProduct(Products.Product product)
    {
      return product.Name.Equals("Sitecore Analytics")
             && product.Revision == revision;
    }

    private bool CheckProduct(Products.Product product)
    {
      return (product.Name.EqualsIgnoreCase(name) || product.OriginalName.EqualsIgnoreCase(name))
             && product.Version == version
             && product.Revision == revision;
    }

    #endregion

    #region Properties

    #region Public properties

    public bool IsChecked
    {
      get
      {
        return _IsChecked;
      }

      set
      {
        _IsChecked = value;
        NotifyPropertyChanged("IsChecked");
      }
    }

    public bool IsEnabled
    {
      get
      {
        return isEnabled;
      }
    }

    public string Name
    {
      get
      {
        return name;
      }
    }

    public ReadOnlyCollection<Uri> Value
    {
      get
      {
        return _Value;
      }

      set
      {
        _Value = value;
        NotifyPropertyChanged("Value");
      }
    }

    #endregion

    #region Protected properties

    protected string Label
    {
      get
      {
        return label;
      }
    }

    protected string Revision
    {
      get
      {
        return revision;
      }
    }

    protected string Version
    {
      get
      {
        return version;
      }
    }

    #endregion

    #endregion
  }
}