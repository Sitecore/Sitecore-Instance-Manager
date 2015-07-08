#region Usings

using System;
using System.Collections.ObjectModel;
using System.Linq;
using SIM.Base;
using SIM.Products;

#endregion

namespace SIM.Tool.Windows.UserControls.Download
{
  #region

  

  #endregion

  /// <summary>
  ///   The product in checkbox.
  /// </summary>
  public class ProductDownloadInCheckbox : DataObjectBase
  {
    #region Fields

    /// <summary>
    ///   The name.
    /// </summary>
    private readonly string name;

    /// <summary>
    ///   The is checked.
    /// </summary>
    private bool isChecked;

    /// <summary>
    ///   The value.
    /// </summary>
    private ReadOnlyCollection<Uri> value;

    private readonly bool isEnabled;
    private readonly string label;
    private readonly string revision;
    private readonly string version;
    private readonly string nameOverride;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductInCheckbox"/> class.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>         
    public ProductDownloadInCheckbox(string record)
    {
      // record = Sitecore CMS|6.6.0|130214|Update-4|http://sdn.sitecore.net/downloads/Sitecore660rev130214.download|http://sdn.sitecore.net/downloads/dms660rev130214.download
      var arr = record.Split('|');
      this.name = arr[0];
      this.version = arr[1];
      this.revision = arr[2];
      this.label = arr[3];
      this.value = new ReadOnlyCollection<Uri>(arr.Skip(4).Select(url => new Uri(url)).ToArray());
      this.isEnabled = !ProductManager.Products.Any(CheckProduct);
      if(!this.isEnabled && this.name.EqualsIgnoreCase("Sitecore CMS") && !ProductManager.Products.Any(CheckAnalyticsProduct) && this.value.Count > 1)
      {
        this.isEnabled = true;
        this.nameOverride = "Sitecore Analytics";
      }
    }

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

    /// <summary>
    ///   Gets or sets a value indicating whether IsChecked.
    /// </summary>
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

    /// <summary>
    ///   Gets Name.
    /// </summary>
    public string Name
    {
      get
      {
        return this.name;
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

    protected string Label
    {
      get
      {
        return this.label;
      }
    }

    /// <summary>
    ///   Gets or sets Value.
    /// </summary>
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

    public override string ToString()
    {
      return string.Format("{0} {1} rev. {2}{3}{4}", this.nameOverride ?? this.Name, this.Version, this.Revision, string.IsNullOrEmpty(this.Label) ? string.Empty : " (" + this.Label + ")", this.IsEnabled ? string.Empty : " - you already have it");
    }
  }
}