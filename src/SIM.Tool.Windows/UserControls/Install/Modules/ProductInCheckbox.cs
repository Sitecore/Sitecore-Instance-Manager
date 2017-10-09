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
      Value = value;
      _Products = null;
    }

    public ProductInCheckbox(string name, Product[] products)
    {
      this.name = name;
      _Products = products;
      Value = products.First();
    }

    #endregion

    #region Properties

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

    public string Name
    {
      get
      {
        return name ?? Value.Label ?? Value.Name;
      }
    }

    public Product[] Scope
    {
      get
      {
        return _Products;
      }
    }

    public Product Value
    {
      get
      {
        return _Value;
      }

      set
      {
        _Value = value;
        NotifyPropertyChanged("Value");
        NotifyPropertyChanged("ValueIndex");
      }
    }

    public int ValueIndex
    {
      get
      {
        var i = 0;
        foreach (var product in Scope)
        {
          if (product == Value)
          {
            return i;
          }

          i++;
        }

        return -1;
      }

      set
      {
        _Value = Scope[value];
        NotifyPropertyChanged("ValueIndex");
      }
    }

    #endregion

    #region Public methods

    public override string ToString()
    {
      return Value.Name;
    }

    #endregion
  }
}