#region Usings

using System.Linq;
using SIM.Base;
using SIM.Products;

#endregion

namespace SIM.Tool.Windows.UserControls.Install.Modules
{
  #region

  

  #endregion

  /// <summary>
  ///   The product in checkbox.
  /// </summary>
  public class ProductInCheckbox : DataObjectBase
  {
    #region Fields

    /// <summary>
    ///   The name.
    /// </summary>
    private readonly string name;

    /// <summary>
    ///   The products.
    /// </summary>
    private readonly Product[] products;

    /// <summary>
    ///   The is checked.
    /// </summary>
    private bool isChecked;

    /// <summary>
    ///   The value.
    /// </summary>
    private Product value;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductInCheckbox"/> class.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    public ProductInCheckbox(Product value)
    {
      this.Value = value;
      this.products = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductInCheckbox"/> class.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <param name="products">
    /// The products. 
    /// </param>
    public ProductInCheckbox(string name, Product[] products)
    {
      this.name = name;
      this.products = products;
      this.Value = products.First();
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

    /// <summary>
    ///   Gets Name.
    /// </summary>
    public string Name
    {
      get
      {
        return this.name ?? this.Value.Label ?? this.Value.Name;
      }
    }

    /// <summary>
    ///   Gets Scope.
    /// </summary>
    public Product[] Scope
    {
      get
      {
        return this.products;
      }
    }

    /// <summary>
    ///   Gets or sets Value.
    /// </summary>
    public Product Value
    {
      get
      {
        return this.value;
      }

      set
      {
        this.value = value;
        this.NotifyPropertyChanged("Value");
        this.NotifyPropertyChanged("ValueIndex");
      }
    }

    /// <summary>
    ///   Gets or sets ValueIndex.
    /// </summary>
    public int ValueIndex
    {
      get
      {
        int i = 0;
        foreach (Product product in this.Scope)
        {
          if (product == this.Value)
          {
            return i;
          }

          i++;
        }

        return -1;
      }

      set
      {
        this.value = this.Scope[value];
        this.NotifyPropertyChanged("ValueIndex");
      }
    }

    #endregion

    public override string ToString()
    {
      return this.Value.Name;
    }
  }
}