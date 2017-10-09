namespace SIM.Tool.Windows.UserControls.Download8
{
  using System;
  using System.Linq;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using SIM.Extensions;

  public class ProductDownload8InCheckbox : DataObjectBase
  {
    #region Fields

    private bool isEnabled { get; }
    private string label { get; }
    private string name { get; }
    private string NameOverride { get; }
    private string revision { get; }
    private string version { get; }
    private bool _IsChecked;

    private Uri _Value;

    #endregion

    #region Constructors

    public ProductDownload8InCheckbox([NotNull] IRelease release)
    {
      Assert.ArgumentNotNull(release, nameof(release));

      name = "Sitecore CMS";
      version = release.Version.MajorMinor;
      revision = release.Version.Revision.ToString();
      label = release.Label;
      var distribution = release.DefaultDistribution;
      Assert.IsNotNull(distribution, nameof(distribution));

      _Value = new Uri(distribution.Downloads.First(x => x.StartsWith("http")));
      isEnabled = !ProductManager.Products.Any(CheckProduct);
    }

    #endregion

    #region Public methods

    public override string ToString()
    {
      return $"{NameOverride ?? Name} {Version} rev. {Revision}{(string.IsNullOrEmpty(Label) ? string.Empty : $" ({Label})")}{(IsEnabled ? string.Empty : " - you already have it")}";
    }

    #endregion

    #region Private methods

    private bool CheckAnalyticsProduct(Products.Product product)
    {
      return product.Name.Equals("Sitecore Analytics")
             && product.Revision == revision;
    }

    private bool CheckProduct([CanBeNull] Products.Product product)
    {
      if (product == null)
      {
        return false;
      }

      if (!product.Name.EqualsIgnoreCase(name) && !product.OriginalName.EqualsIgnoreCase(name))
      {
        return false;
      }

      if (product.Version != version)
      {
        return false;
      }

      return product.Revision == revision;
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

    public Uri Value
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