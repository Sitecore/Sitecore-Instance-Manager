namespace SIM.Tool.Windows.UserControls.Install.Modules
{
  using System.Linq;
  using SIM.Products;

  #region

  #endregion

  public class ProductInCheckbox : DataObjectBase
  {
    #region Fields

    private string name { get; }

    private readonly Product[] _Products;

    private bool _IsChecked;

    private Product _Value;

    #endregion

    #region Constructors

    public ProductInCheckbox(Product value)
    {
      this.Value = value;
      this._Products = null;
    }

    public ProductInCheckbox(string name, Product[] products)
    {
      this.name = name;
      this._Products = products;
      this.Value = products.First();
    }

    #endregion

    #region Properties

    public bool IsChecked
    {
      get
      {
        return this._IsChecked;
      }

      set
      {
        this._IsChecked = value;
        this.NotifyPropertyChanged("IsChecked");
      }
    }

    public string Name
    {
      get
      {
        return this.name ?? this.Value.Label ?? this.Value.Name;
      }
    }

    public Product[] Scope
    {
      get
      {
        return this._Products;
      }
    }

    public Product Value
    {
      get
      {
        return this._Value;
      }

      set
      {
        this._Value = value;
        this.NotifyPropertyChanged("Value");
        this.NotifyPropertyChanged("ValueIndex");
      }
    }

    public int ValueIndex
    {
      get
      {
        var i = 0;
        foreach (var product in this.Scope)
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
        this._Value = this.Scope[value];
        this.NotifyPropertyChanged("ValueIndex");
      }
    }

    #endregion

    #region Public methods

    public override string ToString()
    {
      return this.Value.Name;
    }

    #endregion
  }
}